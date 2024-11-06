using CoinEx.Net.Converters;
using CryptoExchange.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;
using CoinEx.Net.Enums;
using System.Threading;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Objects.Models.Socket;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Objects.Sockets;
using CoinEx.Net.Objects.Sockets.Queries;
using CoinEx.Net.Objects.Sockets;
using CoinEx.Net.Objects.Sockets.Subscriptions.Deals;
using CoinEx.Net.Objects.Sockets.Subscriptions.Balance;
using CoinEx.Net.Objects.Sockets.Subscriptions.Depth;
using CoinEx.Net.Objects.Sockets.Subscriptions.State;
using CoinEx.Net.Objects.Sockets.Subscriptions.Orders;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Clients;
using CoinEx.Net.Interfaces.Clients.SpotApiV1;
using CryptoExchange.Net.SharedApis;

namespace CoinEx.Net.Clients.SpotApiV1
{
    /// <inheritdoc cref="ICoinExSocketClientSpotApi" />
    internal class CoinExSocketClientSpotApi : SocketApiClient, ICoinExSocketClientSpotApi
    {
        #region fields
        /// <inheritdoc />
        public new CoinExSocketOptions ClientOptions => (CoinExSocketOptions)base.ClientOptions;

        private static readonly MessagePath _idPath = MessagePath.Get().Property("id");
        private static readonly MessagePath _methodPath = MessagePath.Get().Property("method");
        private static readonly MessagePath _symbolPathDeals = MessagePath.Get().Property("params").Index(0);
        private static readonly MessagePath _symbolPathDepth = MessagePath.Get().Property("params").Index(2);
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExSocketClient with default options
        /// </summary>
        internal CoinExSocketClientSpotApi(ILogger logger, CoinExSocketOptions options)
            : base(logger, options.Environment.SocketBaseAddress, options, options.SpotOptions)
        {
            RegisterPeriodicQuery("Ping", TimeSpan.FromMinutes(1), q => (new CoinExQuery<string>("server.ping", new object[] { })), null);
        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new CoinExAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new CoinExNonceProvider());

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null) => $"{baseAsset.ToUpperInvariant()}{quoteAsset.ToUpperInvariant()}";

        #region methods

        /// <inheritdoc />
        public override string? GetListenerIdentifier(IMessageAccessor messageAccessor)
        {
            var id = messageAccessor.GetValue<string>(_idPath);
            if (id != null)
                return id;

            var method = messageAccessor.GetValue<string>(_methodPath);
            if (string.Equals(method, "deals.update", StringComparison.Ordinal))
            {
                var symbol = messageAccessor.GetValue<string>(_symbolPathDeals);
                return method + symbol;
            }

            if (string.Equals(method, "depth.update", StringComparison.Ordinal))
            {
                var symbol = messageAccessor.GetValue<string>(_symbolPathDepth);
                return method + symbol;
            }

            return method;
        }

        /// <inheritdoc />
        protected override Task<Query?> GetAuthenticationRequestAsync(SocketConnection connection)
        {
            var authProvider = (CoinExAuthenticationProvider)AuthenticationProvider!;
            var authParams = authProvider.GetSocketAuthParameters();
            return Task.FromResult<Query?>(new CoinExQuery<CoinExSubscriptionStatus>("server.sign", authParams, false));
        }

        #region public

        /// <inheritdoc />
        public async Task<CallResult> PingAsync()
        {
            var query = await QueryAsync(new CoinExQuery<string>("server.ping", new object[] { })).ConfigureAwait(false);
            return query.AsDataless();
        }

        /// <inheritdoc />
        public async Task<CallResult<DateTime>> GetServerTimeAsync()
        {
            var query = await QueryAsync(new CoinExQuery<long>("server.time", new object[] { })).ConfigureAwait(false);
            return query.As(DateTimeConverter.ConvertFromSeconds(query.Data.Result));
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketSymbolState>> GetTickerAsync(string symbol, int cyclePeriod)
        {
            var query = await QueryAsync(new CoinExQuery<CoinExSocketSymbolState>("state.query", new object[] { symbol, cyclePeriod })).ConfigureAwait(false);
            return query.As<CoinExSocketSymbolState>(query.Data?.Result);
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketOrderBook>> GetOrderBookAsync(string symbol, int limit, int mergeDepth)
        {
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit.ValidateIntValues(nameof(limit), 5, 10, 20);

            var query = await QueryAsync(new CoinExQuery<CoinExSocketOrderBook>("depth.query", new object[] { symbol, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth) }, false)).ConfigureAwait(false);
            return query.As<CoinExSocketOrderBook>(query.Data?.Result);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<CoinExSocketSymbolTrade>>> GetTradeHistoryAsync(string symbol, int? limit = null, int? fromId = null)
        {
            var query = await QueryAsync(new CoinExQuery<IEnumerable<CoinExSocketSymbolTrade>>("deals.query", new object[] { symbol, limit ?? 10, fromId ?? 0 }, false)).ConfigureAwait(false);
            return query.As<IEnumerable<CoinExSocketSymbolTrade>>(query.Data?.Result);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval)
        {
            var startTime = DateTimeConverter.ConvertToSeconds(DateTime.UtcNow.AddDays(-1));
            var endTime = DateTimeConverter.ConvertToSeconds(DateTime.UtcNow);
            var query = await QueryAsync(new CoinExQuery<IEnumerable<CoinExKline>>("kline.query", new object[] { symbol, startTime, endTime, interval.ToSeconds() }, false)).ConfigureAwait(false);
            return query.As<IEnumerable<CoinExKline>>(query.Data?.Result);
        }

        /// <inheritdoc />
        public async Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(IEnumerable<string>? assets = null)
        {
            var query = await QueryAsync(new CoinExQuery<Dictionary<string, CoinExBalance>>("asset.query", assets?.Any() == true ? assets.ToArray() : Array.Empty<object>(), true)).ConfigureAwait(false);
            return query.As<Dictionary<string, CoinExBalance>>(query.Data?.Result);
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOpenOrdersAsync(string symbol, OrderSide? side = null, int? offset = null, int? limit = null)
        {
            var query = await QueryAsync(new CoinExQuery<CoinExSocketPagedResult<CoinExSocketOrder>>("order.query", new object[] { symbol, int.Parse(JsonConvert.SerializeObject(side ?? OrderSide.Either, new OrderSideIntConverter(false))), offset ?? 0, limit ?? 10 }, true)).ConfigureAwait(false);
            return query.As<CoinExSocketPagedResult<CoinExSocketOrder>>(query.Data?.Result);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<CoinExSocketSymbolState>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExStateSubscription(_logger, symbol, new object[] { symbol }, x => onMessage(x.As(x.Data.Single())));
            return await SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToAllTickerUpdatesAsync(Action<DataEvent<IEnumerable<CoinExSocketSymbolState>>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExStateSubscription(_logger, null, new object[] { }, onMessage);
            return await SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int limit, int mergeDepth, Action<DataEvent<CoinExSocketOrderBook>> onMessage, bool diffUpdates = false, CancellationToken ct = default)
        {
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit.ValidateIntValues(nameof(limit), 5, 10, 20);

            var subscription = new CoinExDepthSubscription(_logger, symbol, new object[] { symbol, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth), diffUpdates }, onMessage);
            return await SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<CoinExSocketSymbolTrade>>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExDealsSubscription(_logger, symbol, new object[] { symbol }, onMessage);
            return await SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<IEnumerable<CoinExBalance>>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExBalanceSubscription(_logger, onMessage);
            return await SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default)
            => SubscribeToOrderUpdatesAsync(Array.Empty<string>(), onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExOrderSubscription(_logger, symbols, onMessage);
            return await SubscribeAsync(subscription, ct).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
