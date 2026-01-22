using CoinEx.Net.Clients.MessageHandlers;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Objects.Options;
using CoinEx.Net.Objects.Sockets.V2;
using CoinEx.Net.Objects.Sockets.V2.Queries;
using CoinEx.Net.Objects.Sockets.V2.Subscriptions;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.SpotApiV2
{
    /// <inheritdoc cref="ICoinExSocketClientSpotApi" />
    internal partial class CoinExSocketClientSpotApi : SocketApiClient, ICoinExSocketClientSpotApi
    {
        #region fields
        /// <inheritdoc />
        public new CoinExSocketOptions ClientOptions => (CoinExSocketOptions)base.ClientOptions;

        protected override ErrorMapping ErrorMapping => CoinExErrors.SocketErrorMapping;

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
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(CoinExExchange._serializerContext));

        public override ISocketMessageHandler CreateMessageConverter(WebSocketMessageType messageType) => new CoinExSocketSpotMessageHandler();

        /// <inheritdoc />
        public override ReadOnlySpan<byte> PreprocessStreamMessage(SocketConnection connection, WebSocketMessageType type, ReadOnlySpan<byte> data)
        {
            if (type == WebSocketMessageType.Binary)
                return data.DecompressGzip();

            return data;
        }

        #region public

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToSystemNoticeUpdatesAsync(Action<DataEvent<CoinExMaintenance[]>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExMaintenance[]>>((receiveTime, originalData, invocations, data) =>
            {
                onMessage(
                    new DataEvent<CoinExMaintenance[]>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                    );
            });

            var subscription = new CoinExSubscription<CoinExMaintenance[]>(_logger, this, "notice", null, new Dictionary<string, object>
            {
                { "channels", new int[] { 101 } }
            }, internalHandler, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(Action<DataEvent<CoinExTicker[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExTickerSubscription(_logger, this, [], new Dictionary<string, object>
            {
                { "market_list", new string[] { } }
            }, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExTicker[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExTickerSubscription(_logger, this, symbols.ToArray(), new Dictionary<string, object>
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
            var subscription = new CoinExOrderBookSubscription(_logger, this, symbols.ToArray(), new Dictionary<string, object>
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
            var subscription = new CoinExTradesSubscription(_logger, this, symbols.ToArray(), new Dictionary<string, object>
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
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExIndexPriceUpdate>>((receiveTime, originalData, invocations, data) =>
            {
                onMessage(
                    new DataEvent<CoinExIndexPriceUpdate>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithSymbol(data.Data.Symbol)
                    );
            });

            var subscription = new CoinExSubscription<CoinExIndexPriceUpdate>(_logger, this, "index", symbols.ToArray(), new Dictionary<string, object>
            {
                { "market_list", symbols.ToArray() }
            }, internalHandler);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToBookPriceUpdatesAsync(string symbol, Action<DataEvent<CoinExBookPriceUpdate>> onMessage, CancellationToken ct = default)
            => SubscribeToBookPriceUpdatesAsync(new[] { symbol }, onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBookPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExBookPriceUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExBookPriceUpdate>>((receiveTime, originalData, invocations, data) =>
            {
                UpdateTimeOffset(data.Data.UpdateTime);

                onMessage(
                    new DataEvent<CoinExBookPriceUpdate>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithSymbol(data.Data.Symbol)
                        .WithDataTimestamp(data.Data.UpdateTime, GetTimeOffset())
                    );
            });

            var subscription = new CoinExSubscription<CoinExBookPriceUpdate>(_logger, this, "bbo", symbols.ToArray(), new Dictionary<string, object>
            {
                { "market_list", symbols.ToArray() }
            }, internalHandler);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExOrderUpdate>>((receiveTime, originalData, invocations, data) =>
            {
                if (data.Data.Order.UpdateTime != null)
                    UpdateTimeOffset(data.Data.Order.UpdateTime.Value);

                onMessage(
                    new DataEvent<CoinExOrderUpdate>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithSymbol(data.Data.Order.Symbol)
                        .WithDataTimestamp(data.Data.Order.UpdateTime, GetTimeOffset())
                    );
            });

            var subscription = new CoinExSubscription<CoinExOrderUpdate>(_logger, this, "order", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "market_list", Array.Empty<string>() }
            }, internalHandler, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToStopOrderUpdatesAsync(Action<DataEvent<CoinExStopOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExStopOrderUpdate>>((receiveTime, originalData, invocations, data) =>
            {
                if (data.Data.Order.UpdateTime != null)
                    UpdateTimeOffset(data.Data.Order.UpdateTime.Value);

                onMessage(
                    new DataEvent<CoinExStopOrderUpdate>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithSymbol(data.Data.Order.Symbol)
                        .WithDataTimestamp(data.Data.Order.UpdateTime, GetTimeOffset())
                    );
            });
            var subscription = new CoinExSubscription<CoinExStopOrderUpdate>(_logger, this, "stop", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "market_list", Array.Empty<string>() }
            }, internalHandler, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(Action<DataEvent<CoinExUserTrade>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExUserTrade>>((receiveTime, originalData, invocations, data) =>
            {
                UpdateTimeOffset(data.Data.CreateTime);

                onMessage(
                    new DataEvent<CoinExUserTrade>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithSymbol(data.Data.Symbol)
                        .WithDataTimestamp(data.Data.CreateTime, GetTimeOffset())
                    );
            });

            var subscription = new CoinExSubscription<CoinExUserTrade>(_logger, this, "user_deals", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "market_list", Array.Empty<string>() }
            }, internalHandler, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<CoinExBalanceUpdate[]>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExBalanceUpdateWrapper>>((receiveTime, originalData, invocations, data) =>
            {
                var timestamp = data.Data.Balances.Max(x => x.UpdateTime);
                UpdateTimeOffset(timestamp);

                onMessage(
                    new DataEvent<CoinExBalanceUpdate[]>(CoinExExchange.ExchangeName, data.Data.Balances, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithDataTimestamp(timestamp, GetTimeOffset())
                    );
            });

            var subscription = new CoinExSubscription<CoinExBalanceUpdateWrapper>(_logger, this, "balance", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "ccy_list", Array.Empty<string>() }
            }, internalHandler, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #endregion
    }
}
