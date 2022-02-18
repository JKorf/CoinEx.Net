using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CryptoExchange.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using CryptoExchange.Net.Logging;
using CoinEx.Net.Interfaces.Clients.SpotApi;

namespace CoinEx.Net.Clients.SpotApi
{
    /// <inheritdoc cref="ICoinExSocketClientSpotStreams" />
    public class CoinExSocketClientSpotStreams : SocketApiClient, ICoinExSocketClientSpotStreams
    {
        #region fields
        private readonly CoinExSocketClient _baseClient;
        private readonly CoinExSocketClientOptions _options;
        private readonly Log _log;

        private const string ServerSubject = "server";
        private const string StateSubject = "state";
        private const string DepthSubject = "depth";
        private const string TransactionSubject = "deals";
        private const string KlineSubject = "kline";
        private const string BalanceSubject = "asset";
        private const string OrderSubject = "order";

        private const string SubscribeAction = "subscribe";
        private const string QueryAction = "query";
        private const string ServerTimeAction = "time";
        private const string PingAction = "ping";
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExSocketClient with default options
        /// </summary>
        public CoinExSocketClientSpotStreams(Log log, CoinExSocketClient baseClient, CoinExSocketClientOptions options)
            : base(options, options.SpotStreamsOptions)
        {
            _log = log;
            _options = options;
            _baseClient = baseClient;
        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new CoinExAuthenticationProvider(credentials, _options.NonceProvider ?? new CoinExNonceProvider());

        #region methods
        #region public

        /// <inheritdoc />
        public async Task<CallResult<bool>> PingAsync()
        {
            var result = await _baseClient.QueryInternalAsync<string>(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), ServerSubject, PingAction), false).ConfigureAwait(false);
            return result.As(result.Success);
        }

        /// <inheritdoc />
        public async Task<CallResult<DateTime>> GetServerTimeAsync()
        {
            var result = await _baseClient.QueryInternalAsync<long>(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), ServerSubject, ServerTimeAction), false).ConfigureAwait(false);
            if (!result)
                return new CallResult<DateTime>(result.Error!);
            return new CallResult<DateTime>(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result.Data));
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketSymbolState>> GetTickerAsync(string symbol, int cyclePeriod)
        {
            symbol.ValidateCoinExSymbol();
            return await _baseClient.QueryInternalAsync<CoinExSocketSymbolState>(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), StateSubject, QueryAction, symbol, cyclePeriod), false).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketOrderBook>> GetOrderBookAsync(string symbol, int limit, int mergeDepth)
        {
            symbol.ValidateCoinExSymbol();
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit.ValidateIntValues(nameof(limit), 5, 10, 20);

            return await _baseClient.QueryInternalAsync<CoinExSocketOrderBook>(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), DepthSubject, QueryAction, symbol, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth)), false).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<CoinExSocketSymbolTrade>>> GetTradeHistoryAsync(string symbol, int limit, int? fromId = null)
        {
            symbol.ValidateCoinExSymbol();

            return await _baseClient.QueryInternalAsync<IEnumerable<CoinExSocketSymbolTrade>>(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), TransactionSubject, QueryAction, symbol, limit, fromId ?? 0), false).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExKline>> GetKlinesAsync(string symbol, KlineInterval interval)
        {
            symbol.ValidateCoinExSymbol();

            return await _baseClient.QueryInternalAsync<CoinExKline>(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), KlineSubject, QueryAction, symbol, interval.ToSeconds()), false).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(IEnumerable<string> assets)
        {
            return await _baseClient.QueryInternalAsync<Dictionary<string, CoinExBalance>>(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), BalanceSubject, QueryAction, assets.ToArray()), true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOpenOrdersAsync(string symbol, OrderSide side, int offset, int limit)
        {
            symbol.ValidateCoinExSymbol();
            return await _baseClient.QueryInternalAsync<CoinExSocketPagedResult<CoinExSocketOrder>>(this,
                new CoinExSocketRequest(_baseClient.NextIdInternal(), OrderSubject, QueryAction, symbol, int.Parse(JsonConvert.SerializeObject(side, new OrderSideIntConverter(false))), offset, limit), true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<CoinExSocketSymbolState>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                var desResult = _baseClient.DeserializeInternal<Dictionary<string, CoinExSocketSymbolState>>(data.Data[0]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid state update: " + desResult.Error);
                    return;
                }
                var result = desResult.Data.First().Value;
                result.Symbol = symbol;

                onMessage(data.As(result, symbol));
            });

            return await _baseClient.SubscribeInternalAsync(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), StateSubject, SubscribeAction, symbol), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToAllTickerUpdatesAsync(Action<DataEvent<IEnumerable<CoinExSocketSymbolState>>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                var desResult = _baseClient.DeserializeInternal<Dictionary<string, CoinExSocketSymbolState>>(data.Data[0]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid state update: " + desResult.Error);
                    return;
                }

                foreach (var item in desResult.Data)
                    item.Value.Symbol = item.Key;

                onMessage(data.As(desResult.Data.Select(d => d.Value)));
            });

            return await _baseClient.SubscribeInternalAsync(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), StateSubject, SubscribeAction), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int limit, int mergeDepth, Action<DataEvent<CoinExSocketOrderBook>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit.ValidateIntValues(nameof(limit), 5, 10, 20);

            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length != 3)
                {
                    _log.Write(LogLevel.Warning, $"Received unexpected data format for depth update. Expected 3 objects, received {data.Data.Length}. Data: " + data);
                    return;
                }

                var fullUpdate = (bool)data.Data[0];
                var desResult = _baseClient.DeserializeInternal<CoinExSocketOrderBook>(data.Data[1]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid depth update: " + desResult.Error);
                    return;
                }

                desResult.Data.FullUpdate = fullUpdate;
                onMessage(data.As(desResult.Data, symbol));
            });

            return await _baseClient.SubscribeInternalAsync(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), DepthSubject, SubscribeAction, symbol, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth)), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<CoinExSocketSymbolTrade>>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length != 2 && data.Data.Length != 3)
                {
                    // Sometimes an extra True is send as 3rd parameter?
                    _log.Write(LogLevel.Warning, $"Received unexpected data format for trade update. Expected 2 objects, received {data.Data.Length}. Data: {data.OriginalData}");
                    return;
                }

                var desResult = _baseClient.DeserializeInternal<IEnumerable<CoinExSocketSymbolTrade>>(data.Data[1]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid trade update: " + desResult.Error);
                    return;
                }

                onMessage(data.As(desResult.Data, symbol));
            });

            return await _baseClient.SubscribeInternalAsync(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), TransactionSubject, SubscribeAction, symbol), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<CoinExKline>>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length > 2)
                {
                    _log.Write(LogLevel.Warning, $"Received unexpected data format for kline update. Expected 1 or 2 objects, received {data.Data.Length}. Data: [{string.Join(",", data.Data.Select(s => s.ToString()))}]");
                    return;
                }

                var desResult = _baseClient.DeserializeInternal<IEnumerable<CoinExKline>>(new JArray(data.Data));
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid kline update: " + desResult.Error);
                    return;
                }

                onMessage(data.As(desResult.Data, symbol));
            });

            return await _baseClient.SubscribeInternalAsync(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), KlineSubject, SubscribeAction, symbol, interval.ToSeconds()), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<IEnumerable<CoinExBalance>>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length != 1)
                {
                    if (data.Data.Length != 2 || data.Data.Length == 2 && data.Data[1].ToString().Trim() != "0")
                    {
                        _log.Write(LogLevel.Warning, $"Received unexpected data format for balance update. Expected 1 objects, received {data.Data.Length}. Data: [{string.Join(",", data.Data.Select(s => s.ToString()))}]");
                        return;
                    }
                }

                var desResult = _baseClient.DeserializeInternal<Dictionary<string, CoinExBalance>>(data.Data[0]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid balance update: " + desResult.Error);
                    return;
                }

                foreach (var item in desResult.Data)
                    item.Value.Asset = item.Key;

                onMessage(data.As<IEnumerable<CoinExBalance>>(desResult.Data.Values, null));
            });

            return await _baseClient.SubscribeInternalAsync(this, new CoinExSocketRequest(_baseClient.NextIdInternal(), BalanceSubject, SubscribeAction), null, true, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default)
            => SubscribeToOrderUpdatesAsync(Array.Empty<string>(), onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length != 2)
                {
                    _log.Write(LogLevel.Warning, $"Received unexpected data format for order update. Expected 2 objects, received {data.Data.Length}. Data: [{string.Join(",", data.Data.Select(s => s.ToString()))}]");
                    return;
                }

                var updateResult = JsonConvert.DeserializeObject<UpdateType>(data.Data[0].ToString(), new UpdateTypeConverter(false));
                var desResult = _baseClient.DeserializeInternal<CoinExSocketOrder>(data.Data[1]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid order update: " + desResult.Error);
                    return;
                }

                var result = new CoinExSocketOrderUpdate()
                {
                    UpdateType = updateResult,
                    Order = desResult.Data
                };
                onMessage(data.As(result, result.Order.Symbol));
            });

            var request = new CoinExSocketRequest(_baseClient.NextIdInternal(), OrderSubject, SubscribeAction, symbols.ToArray());
            return await _baseClient.SubscribeInternalAsync(this, request, null, true, internalHandler, ct).ConfigureAwait(false);
        }
        #endregion

        #endregion
    }
}
