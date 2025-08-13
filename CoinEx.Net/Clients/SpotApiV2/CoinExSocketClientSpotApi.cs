using CryptoExchange.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;
using System.Threading;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Clients;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Net.WebSockets;
using CoinEx.Net.Objects.Sockets.V2.Subscriptions;
using CoinEx.Net.Objects.Sockets.V2.Queries;
using System.Linq;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Objects.Errors;

namespace CoinEx.Net.Clients.SpotApiV2
{
    /// <inheritdoc cref="ICoinExSocketClientSpotApi" />
    internal partial class CoinExSocketClientSpotApi : SocketApiClient, ICoinExSocketClientSpotApi
    {
        #region fields
        /// <inheritdoc />
        public new CoinExSocketOptions ClientOptions => (CoinExSocketOptions)base.ClientOptions;

        private static readonly MessagePath _idPath = MessagePath.Get().Property("id");
        private static readonly MessagePath _methodPath = MessagePath.Get().Property("method");
        private static readonly MessagePath _symbolPath = MessagePath.Get().Property("data").Property("market");
        protected override ErrorCollection ErrorMapping { get; } = CoinExErrorMapping.SocketErrorMapping;

        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExSocketClient with default options
        /// </summary>
        internal CoinExSocketClientSpotApi(ILogger logger, CoinExSocketOptions options)
            : base(logger, options.Environment.SocketBaseAddress, options, options.SpotOptions)
        {
            KeepAliveInterval = TimeSpan.Zero; // Server doesn't correctly respond to ping frames

            RegisterPeriodicQuery(
                "Ping",
                TimeSpan.FromSeconds(30), 
                q => new CoinExQuery(this, "server.ping", new Dictionary<string, object>()) { RequestTimeout = TimeSpan.FromSeconds(5) },
                (connection, result) =>
                {
                    if (result.Error?.ErrorType == ErrorType.Timeout)
                    {
                        // Ping timeout, reconnect
                        _logger.LogWarning("[Sckt {SocketId}] Ping response timeout, reconnecting", connection.SocketId);
                        _ = connection.TriggerReconnectAsync();
                    }
                });
        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new CoinExV2AuthenticationProvider(credentials);

        public ICoinExSocketClientSpotApiShared SharedClient => this;

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => CoinExExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

        #region methods

        /// <inheritdoc />
        protected override IByteMessageAccessor CreateAccessor(WebSocketMessageType type) => new SystemTextJsonByteMessageAccessor(SerializerOptions.WithConverters(CoinExExchange._serializerContext));
        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(CoinExExchange._serializerContext));


        /// <inheritdoc />
        public override string? GetListenerIdentifier(IMessageAccessor messageAccessor)
        {
            var id = messageAccessor.GetValue<int?>(_idPath);
            if (id != null)
                return id.ToString();

            var method = messageAccessor.GetValue<string>(_methodPath);
            if (!string.Equals(method, "state.update", StringComparison.Ordinal)
                && !string.Equals(method, "deals.update", StringComparison.Ordinal)
                && !string.Equals(method, "user_deals.update", StringComparison.Ordinal))
            {
                var symbol = messageAccessor.GetValue<string>(_symbolPath);
                return method + symbol;
            }

            return method;
        }

        /// <inheritdoc />
        protected override Task<Query?> GetAuthenticationRequestAsync(SocketConnection connection)
        {
            var authProvider = (CoinExV2AuthenticationProvider)AuthenticationProvider!;
            var authParams = authProvider.GetSocketAuthParameters();
            return Task.FromResult<Query?>(new CoinExQuery(this, "server.sign", authParams, false, 0));
        }

        /// <inheritdoc />
        public override ReadOnlyMemory<byte> PreprocessStreamMessage(SocketConnection connection, WebSocketMessageType type, ReadOnlyMemory<byte> data)
        {
            if (type == WebSocketMessageType.Binary)
                return data.DecompressGzip();

            return data;
        }

        #region public

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToSystemNoticeUpdatesAsync(Action<DataEvent<CoinExMaintenance[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExMaintenance[]>(_logger, this, "notice", null, new Dictionary<string, object>
            {
                { "channels", new int[] { 101 } }
            }, onMessage, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(Action<DataEvent<CoinExTicker[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExTickerSubscription(_logger, this, null, new Dictionary<string, object>
            {
                { "market_list", new string[] { } }
            }, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExTicker[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExTickerSubscription(_logger, this, symbols, new Dictionary<string, object>
            {
                { "market_list", symbols.ToArray() }
            }, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<CoinExOrderBook>> onMessage, CancellationToken ct = default)
            => SubscribeToOrderBookUpdatesAsync(new [] { symbol }, depth, mergeLevel, fullBookUpdates, onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(IEnumerable<string> symbols, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<CoinExOrderBook>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExOrderBookSubscription(_logger, this, symbols, new Dictionary<string, object>
            {
                { "market_list", symbols.Select(x => new object[] { x, depth, mergeLevel ?? "0", fullBookUpdates }).ToArray() }
            }, x => onMessage(x.WithSymbol(x.Data.Symbol)));
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<CoinExTrade[]>> onMessage, CancellationToken ct = default)
            => SubscribeToTradeUpdatesAsync(new[] { symbol }, onMessage, ct);

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(Action<DataEvent<CoinExTrade[]>> onMessage, CancellationToken ct = default)
            => SubscribeToTradeUpdatesAsync(new string[] { }, onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExTrade[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExTradesSubscription(_logger, this, symbols, new Dictionary<string, object>
            {
                { "market_list", symbols.ToArray() }
            }, onMessage); 
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToIndexPriceUpdatesAsync(string symbol, Action<DataEvent<CoinExIndexPriceUpdate>> onMessage, CancellationToken ct = default)
            => SubscribeToIndexPriceUpdatesAsync(new[] { symbol }, onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToIndexPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExIndexPriceUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExIndexPriceUpdate>(_logger, this, "index", symbols, new Dictionary<string, object>
            {
                { "market_list", symbols.ToArray() }
            }, x => onMessage(x.WithSymbol(x.Data.Symbol)));
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToBookPriceUpdatesAsync(string symbol, Action<DataEvent<CoinExBookPriceUpdate>> onMessage, CancellationToken ct = default)
            => SubscribeToBookPriceUpdatesAsync(new[] { symbol }, onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBookPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExBookPriceUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExBookPriceUpdate>(_logger, this, "bbo", symbols, new Dictionary<string, object>
            {
                { "market_list", symbols.ToArray() }
            }, x => onMessage(x.WithSymbol(x.Data.Symbol).WithDataTimestamp(x.Data.UpdateTime)));
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExOrderUpdate>(_logger, this, "order", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "market_list", Array.Empty<string>() }
            }, x => onMessage(
                x.WithSymbol(x.Data.Order.Symbol)
                .WithUpdateType(SocketUpdateType.Update)
                .WithDataTimestamp(x.Data.Order.UpdateTime)), true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToStopOrderUpdatesAsync(Action<DataEvent<CoinExStopOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExStopOrderUpdate>(_logger, this, "stop", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "market_list", Array.Empty<string>() }
            }, x => onMessage(
                x.WithSymbol(x.Data.Order.Symbol)
                .WithUpdateType(SocketUpdateType.Update)
                .WithDataTimestamp(x.Data.Order.UpdateTime)), true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(Action<DataEvent<CoinExUserTrade>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExUserTrade>(_logger, this, "user_deals", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "market_list", Array.Empty<string>() }
            }, x => onMessage(
                x.WithSymbol(x.Data.Symbol)
                .WithUpdateType(SocketUpdateType.Update)
                .WithDataTimestamp(x.Data.CreateTime)), true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<CoinExBalanceUpdate[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExBalanceUpdateWrapper>(_logger, this, "balance", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "ccy_list", Array.Empty<string>() }
            }, x => onMessage(
                x.As(x.Data.Balances, "balance", null, SocketUpdateType.Update)
                .WithDataTimestamp(x.Data.Balances.Max(x => x.UpdateTime))), true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #endregion
    }
}
