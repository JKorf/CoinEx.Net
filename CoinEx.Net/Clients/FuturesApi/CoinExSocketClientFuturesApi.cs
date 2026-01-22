using CoinEx.Net.Clients.MessageHandlers;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
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

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc cref="ICoinExSocketClientFuturesApi" />
    internal partial class CoinExSocketClientFuturesApi : SocketApiClient, ICoinExSocketClientFuturesApi
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
        internal CoinExSocketClientFuturesApi(ILogger logger, CoinExSocketOptions options)
            : base(logger, options.Environment.SocketBaseAddress, options, options.FuturesOptions)
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

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => CoinExExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

        public override ISocketMessageHandler CreateMessageConverter(WebSocketMessageType messageType) => new CoinExSocketFuturesMessageHandler();

        #region methods

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(CoinExExchange._serializerContext));

        public ICoinExSocketClientFuturesApiShared SharedClient => this;

        /// <inheritdoc />
        public override ReadOnlySpan<byte> PreprocessStreamMessage(SocketConnection connection, WebSocketMessageType type, ReadOnlySpan<byte> data)
        {
            if (type == WebSocketMessageType.Binary)
                return data.DecompressGzip();

            return data;
        }

        #region public

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExFuturesTickerUpdate[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExFuturesTickerSubscription(_logger, this, symbols.ToArray(), new Dictionary<string, object>
            {
                { "market_list", symbols.ToArray() }
            }, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(Action<DataEvent<CoinExFuturesTickerUpdate[]>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExFuturesTickerSubscription(_logger, this, [], new Dictionary<string, object>
            {
                { "market_list", new object[] { } }
            }, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<CoinExOrderBook>> onMessage, CancellationToken ct = default)
            => SubscribeToOrderBookUpdatesAsync(new[] { symbol }, depth, mergeLevel, fullBookUpdates, onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(IEnumerable<string> symbols, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<CoinExOrderBook>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExOrderBook>>((receiveTime, originalData, invocations, data) =>
            {
                if (data.Data.UpdateTime != null)
                    UpdateTimeOffset(data.Data.UpdateTime.Value);

                onMessage(
                    new DataEvent<CoinExOrderBook>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithSymbol(data.Data.Symbol)
                        .WithDataTimestamp(data.Data.Data.UpdateTime, GetTimeOffset())
                    );
            });

            var subscription = new CoinExSubscription<CoinExOrderBook>(_logger, this, "depth", symbols.ToArray(), new Dictionary<string, object>
            {
                { "market_list", symbols.Select(x => new object[] { x, depth, mergeLevel ?? "0", fullBookUpdates }).ToArray() }
            }, internalHandler);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
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
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
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
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
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
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExFuturesOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExFuturesOrderUpdate>>((receiveTime, originalData, invocations, data) =>
            {
                if (data.Data.Order.UpdateTime != null)
                    UpdateTimeOffset(data.Data.Order.UpdateTime.Value);

                onMessage(
                    new DataEvent<CoinExFuturesOrderUpdate>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithSymbol(data.Data.Order.Symbol)
                        .WithDataTimestamp(data.Data.Order.UpdateTime, GetTimeOffset())
                    );
            });
            var subscription = new CoinExSubscription<CoinExFuturesOrderUpdate>(_logger, this, "order", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "market_list", Array.Empty<string>() }
            }, internalHandler, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
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
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
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
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<CoinExFuturesBalance[]>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExFuturesBalanceUpdate>>((receiveTime, originalData, invocations, data) =>
            {
                onMessage(
                    new DataEvent<CoinExFuturesBalance[]>(CoinExExchange.ExchangeName, data.Data.Balances, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                    );
            });
            var subscription = new CoinExSubscription<CoinExFuturesBalanceUpdate>(_logger, this, "balance", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "ccy_list", Array.Empty<string>() }
            }, internalHandler, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(Action<DataEvent<CoinExPositionUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DateTime, string?, int, CoinExSocketUpdate<CoinExPositionUpdate>>((receiveTime, originalData, invocations, data) =>
            {
                if (data.Data.Position.UpdateTime != null)
                    UpdateTimeOffset(data.Data.Position.UpdateTime.Value);

                onMessage(
                    new DataEvent<CoinExPositionUpdate>(CoinExExchange.ExchangeName, data.Data, receiveTime, originalData)
                        .WithUpdateType(SocketUpdateType.Update)
                        .WithStreamId(data.Method)
                        .WithSymbol(data.Data.Position.Symbol)
                        .WithDataTimestamp(data.Data.Position.UpdateTime, GetTimeOffset())
                    );
            });
            var subscription = new CoinExSubscription<CoinExPositionUpdate>(_logger, this, "position", Array.Empty<string>(), new Dictionary<string, object>
            {
                { "market_list", Array.Empty<string>() }
            }, internalHandler, true);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/futures"), subscription, ct).ConfigureAwait(false);
        }
        #endregion

        #endregion
    }
}
