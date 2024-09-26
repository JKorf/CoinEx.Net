using CryptoExchange.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Authentication;
using CoinEx.Net.Enums;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Objects.Models;
using CryptoExchange.Net.CommonObjects;
using System.Globalization;
using CryptoExchange.Net.Interfaces.CommonClients;
using Microsoft.Extensions.Logging;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Clients;
using CoinEx.Net.Interfaces.Clients.SpotApiV1;
using CryptoExchange.Net.SharedApis;

namespace CoinEx.Net.Clients.SpotApiV1
{
    /// <inheritdoc cref="ICoinExRestClientSpotApi" />
    internal class CoinExRestClientSpotApi : RestApiClient, ICoinExRestClientSpotApi, ISpotClient
    {
        #region fields
        /// <inheritdoc />
        public new CoinExRestOptions ClientOptions => (CoinExRestOptions)base.ClientOptions;

        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<OrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client. Note that this does not trigger when using CancelAllOrdersAsync
        /// </summary>
        public event Action<OrderId>? OnOrderCanceled;
        #endregion

        /// <inheritdoc />
        public string ExchangeName => "CoinEx";

        #region Api clients
        /// <inheritdoc />
        public ICoinExRestClientSpotApiAccount Account { get; }
        /// <inheritdoc />
        public ICoinExRestClientSpotApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public ICoinExRestClientSpotApiTrading Trading { get; }
        #endregion

        internal readonly string _brokerId;

        #region ctor
        internal CoinExRestClientSpotApi(ILogger logger, HttpClient? httpClient, CoinExRestOptions options) :
            base(logger, httpClient, options.Environment.RestBaseAddress, options, options.SpotOptions)
        {
            Account = new CoinExRestClientSpotApiAccount(this);
            ExchangeData = new CoinExRestClientSpotApiExchangeData(this);
            Trading = new CoinExRestClientSpotApiTrading(this);

            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;

            _brokerId = !string.IsNullOrEmpty(options.BrokerId) ? options.BrokerId! : "147866029";

        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new CoinExAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new CoinExNonceProvider());

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null) => $"{baseAsset.ToUpperInvariant()}{quoteAsset.ToUpperInvariant()}";

        #region methods
        #region private
        internal async Task<WebCallResult<T>> Execute<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            var result = await SendRequestAsync<CoinExApiResult<T>>(uri, method, ct, parameters, signed, requestWeight: 0).ConfigureAwait(false);
            if (!result)
                return result.As<T>(default);

            if (result.Data.Code != 0)
                return result.AsError<T>(new ServerError(result.Data.Code, result.Data.Message!));

            return result.As(result.Data.Data);
        }

        internal async Task<WebCallResult> Execute(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false)
        {
            var result = await SendRequestAsync<CoinExApiResult>(uri, method, ct, parameters, signed, requestWeight: 0).ConfigureAwait(false);
            if (!result)
                return result.AsDataless();

            if (result.Data.Code != 0)
                return result.AsDatalessError(new ServerError(result.Data.Code, result.Data.Message!));

            return result.AsDataless();
        }

        internal async Task<WebCallResult<CoinExPagedResult<T>>> ExecutePaged<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            var result = await SendRequestAsync<CoinExApiResult<CoinExPagedResult<T>>>(uri, method, ct, parameters, signed, requestWeight: 0).ConfigureAwait(false);
            if (!result)
                return result.As<CoinExPagedResult<T>>(default);

            if (result.Data.Code != 0)
                return result.AsError<CoinExPagedResult<T>>(new ServerError(result.Data.Code, result.Data.Message!));

            return result.As(result.Data.Data);
        }

        internal Uri GetUrl(string endpoint)
        {
            return new Uri(BaseAddress.AppendPath("v1").AppendPath(endpoint));
        }

        internal void InvokeOrderPlaced(OrderId id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(OrderId id)
        {
            OnOrderCanceled?.Invoke(id);
        }

        #endregion
        #endregion

        #region common interface

        /// <summary>
        /// Get the name of a symbol for CoinEx based on the base and quote asset
        /// </summary>
        /// <param name="baseAsset"></param>
        /// <param name="quoteAsset"></param>
        /// <returns></returns>
        public string GetSymbolName(string baseAsset, string quoteAsset) => (baseAsset + quoteAsset).ToUpperInvariant();

        async Task<WebCallResult<IEnumerable<Symbol>>> IBaseRestClient.GetSymbolsAsync(CancellationToken ct)
        {
            var symbols = await ExchangeData.GetSymbolInfoAsync(ct: ct).ConfigureAwait(false);
            if (!symbols)
                return symbols.As<IEnumerable<Symbol>>(null);

            return symbols.As(symbols.Data.Select(d => new Symbol
            {
                SourceObject = d,
                Name = d.Key,
                MinTradeQuantity = d.Value.MinQuantity,
                PriceDecimals = d.Value.PricingDecimal,
                QuantityDecimals = d.Value.TradingDecimal
            }));
        }

        async Task<WebCallResult<Ticker>> IBaseRestClient.GetTickerAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for CoinEx " + nameof(ISpotClient.GetTickerAsync), nameof(symbol));

            var tickers = await ExchangeData.GetTickerAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!tickers)
                return tickers.As<Ticker>(null);

            return tickers.As(new Ticker
            {
                SourceObject = tickers.Data,
                Symbol = symbol,
                HighPrice = tickers.Data.Ticker.HighPrice,
                LowPrice = tickers.Data.Ticker.LowPrice,
                LastPrice = tickers.Data.Ticker.LastPrice,
                Price24H = tickers.Data.Ticker.OpenPrice,
                Volume = tickers.Data.Ticker.Volume
            });
        }

        async Task<WebCallResult<IEnumerable<Ticker>>> IBaseRestClient.GetTickersAsync(CancellationToken ct)
        {
            var tickers = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!tickers)
                return tickers.As<IEnumerable<Ticker>>(null);

            return tickers.As(tickers.Data.Tickers.Select(t =>
                new Ticker
                {
                    SourceObject = t,
                    Symbol = t.Key,
                    HighPrice = t.Value.HighPrice,
                    LowPrice = t.Value.LowPrice,
                    LastPrice = t.Value.LastPrice,
                    Price24H = t.Value.OpenPrice,
                    Volume = t.Value.Volume
                }));
        }

        async Task<WebCallResult<IEnumerable<Kline>>> IBaseRestClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime, DateTime? endTime, int? limit, CancellationToken ct)
        {
            if (startTime != null || endTime != null)
                throw new ArgumentException("CoinEx does not support time based klines requesting", startTime != null ? nameof(startTime) : nameof(endTime));

            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for CoinEx " + nameof(ISpotClient.GetKlinesAsync), nameof(symbol));

            var klines = await ExchangeData.GetKlinesAsync(symbol, GetKlineIntervalFromTimespan(timespan), limit, ct: ct).ConfigureAwait(false);
            if (!klines)
                return klines.As<IEnumerable<Kline>>(null);

            return klines.As(klines.Data.Select(t =>
                new Kline
                {
                    SourceObject = t,
                    OpenPrice = t.OpenPrice,
                    LowPrice = t.LowPrice,
                    OpenTime = t.OpenTime,
                    Volume = t.Volume,
                    ClosePrice = t.ClosePrice,
                    HighPrice = t.HighPrice
                }));
        }

        async Task<WebCallResult<OrderBook>> IBaseRestClient.GetOrderBookAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for CoinEx " + nameof(ISpotClient.GetOrderBookAsync), nameof(symbol));

            var book = await ExchangeData.GetOrderBookAsync(symbol, 0, ct: ct).ConfigureAwait(false);
            if (!book)
                return book.As<OrderBook>(null);

            return book.As(new OrderBook
            {
                SourceObject = book.Data,
                Asks = book.Data.Asks.Select(a => new OrderBookEntry { Price = a.Price, Quantity = a.Quantity }),
                Bids = book.Data.Bids.Select(b => new OrderBookEntry { Price = b.Price, Quantity = b.Quantity })
            });
        }

        async Task<WebCallResult<IEnumerable<Trade>>> IBaseRestClient.GetRecentTradesAsync(string symbol, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for CoinEx " + nameof(ISpotClient.GetRecentTradesAsync), nameof(symbol));

            var trades = await ExchangeData.GetTradeHistoryAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!trades)
                return trades.As<IEnumerable<Trade>>(null);

            return trades.As(trades.Data.Select(t =>
                new Trade
                {
                    SourceObject = t,
                    Price = t.Price,
                    Quantity = t.Quantity,
                    Symbol = symbol,
                    Timestamp = t.Timestamp
                }));
        }

        async Task<WebCallResult<OrderId>> ISpotClient.PlaceOrderAsync(string symbol, CommonOrderSide side, CommonOrderType type, decimal quantity, decimal? price, string? accountId, string? clientOrderId, CancellationToken ct)
        {
            if (price == null && type == CommonOrderType.Limit)
                throw new ArgumentException("Price parameter null while placing a limit order", nameof(price));

            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for CoinEx " + nameof(ISpotClient.PlaceOrderAsync), nameof(symbol));

            var result = await Trading.PlaceOrderAsync(
                symbol,
                side == CommonOrderSide.Sell ? OrderSide.Sell : OrderSide.Buy,
                type == CommonOrderType.Limit ? OrderType.Limit : OrderType.Market,
                quantity,
                price,
                clientOrderId: clientOrderId,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<OrderId>(null);

            return result.As(new OrderId { SourceObject = result.Data, Id = result.Data.Id.ToString(CultureInfo.InvariantCulture) });
        }

        async Task<WebCallResult<Order>> IBaseRestClient.GetOrderAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for CoinEx " + nameof(ISpotClient.GetOrderAsync), nameof(symbol));

            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid order id for CoinEx {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            var order = await Trading.GetOrderAsync(symbol!, id, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.As<Order>(null);

            return order.As(new Order
            {
                SourceObject = order.Data,
                Id = order.Data.Id.ToString(CultureInfo.InvariantCulture),
                Price = order.Data.Price,
                Quantity = order.Data.Quantity,
                QuantityFilled = order.Data.QuantityFilled,
                Timestamp = order.Data.CreateTime,
                Symbol = order.Data.Symbol,
                Side = order.Data.Side == OrderSide.Buy ? CommonOrderSide.Buy: CommonOrderSide.Sell,
                Status = order.Data.Status == OrderStatus.Canceled ? CommonOrderStatus.Canceled: order.Data.Status == OrderStatus.Executed ? CommonOrderStatus.Filled: CommonOrderStatus.Active,
                Type = order.Data.OrderType == OrderType.Market ? CommonOrderType.Market: order.Data.OrderType == OrderType.Limit ? CommonOrderType.Limit: CommonOrderType.Other
            });
        }

        async Task<WebCallResult<IEnumerable<UserTrade>>> IBaseRestClient.GetOrderTradesAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid order id for CoinEx {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            var result = await Trading.GetOrderTradesAsync(id, 1, 100, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<UserTrade>>(null);

            return result.As(result.Data.Data.Select(d =>
                new UserTrade
                {
                    SourceObject = d,
                    Id = d.Id.ToString(CultureInfo.InvariantCulture),
                    Price = d.Price,
                    Quantity = d.Quantity,
                    Fee = d.Fee,
                    FeeAsset = d.FeeAsset,
                    OrderId = d.OrderId?.ToString(CultureInfo.InvariantCulture),
                    Symbol = symbol ?? string.Empty,
                    Timestamp = d.Timestamp
                }));
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetOpenOrdersAsync(string? symbol, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(ISpotClient.GetOpenOrdersAsync)}");

            var openOrders = await Trading.GetOpenOrdersAsync(symbol!, 1, 100, ct: ct).ConfigureAwait(false);
            if (!openOrders)
                return openOrders.As<IEnumerable<Order>>(null);

            return openOrders.As(openOrders.Data.Data.Select(o => new Order
            {
                SourceObject = o,
                Id = o.Id.ToString(CultureInfo.InvariantCulture),
                Price = o.Price,
                Quantity = o.Quantity,
                QuantityFilled = o.QuantityFilled,
                Timestamp = o.CreateTime,
                Symbol = o.Symbol,
                Side = o.Side == OrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                Status = o.Status == OrderStatus.Canceled ? CommonOrderStatus.Canceled : o.Status == OrderStatus.Executed ? CommonOrderStatus.Filled : CommonOrderStatus.Active,
                Type = o.OrderType == OrderType.Market ? CommonOrderType.Market : o.OrderType == OrderType.Limit ? CommonOrderType.Limit : CommonOrderType.Other
            }));
        }

        async Task<WebCallResult<IEnumerable<Order>>> IBaseRestClient.GetClosedOrdersAsync(string? symbol, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for CoinEx " + nameof(ISpotClient.GetClosedOrdersAsync), nameof(symbol));

            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(ISpotClient.GetClosedOrdersAsync)}");

            var result = await Trading.GetClosedOrdersAsync(symbol!, 1, 100, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<IEnumerable<Order>>(null);

            return result.As(result.Data.Data.Select(o => new Order
            {
                SourceObject = o,
                Id = o.Id.ToString(CultureInfo.InvariantCulture),
                Price = o.Price,
                Quantity = o.Quantity,
                QuantityFilled = o.QuantityFilled,
                Timestamp = o.CreateTime,
                Symbol = o.Symbol,
                Side = o.Side == OrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                Status = o.Status == OrderStatus.Canceled ? CommonOrderStatus.Canceled : o.Status == OrderStatus.Executed ? CommonOrderStatus.Filled : CommonOrderStatus.Active,
                Type = o.OrderType == OrderType.Market ? CommonOrderType.Market : o.OrderType == OrderType.Limit ? CommonOrderType.Limit : CommonOrderType.Other
            }));
        }

        async Task<WebCallResult<OrderId>> IBaseRestClient.CancelOrderAsync(string orderId, string? symbol, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException(nameof(symbol) + " required for CoinEx " + nameof(ISpotClient.CancelOrderAsync), nameof(symbol));

            if (!long.TryParse(orderId, out var id))
                throw new ArgumentException($"Invalid order id for CoinEx {nameof(ISpotClient.GetOrderAsync)}", nameof(orderId));

            var result = await Trading.CancelOrderAsync(symbol!, id, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.As<OrderId>(null);

            return result.As(new OrderId
            {
                SourceObject = result.Data,
                Id = result.Data.Id.ToString(CultureInfo.InvariantCulture)
            });
        }

        async Task<WebCallResult<IEnumerable<Balance>>> IBaseRestClient.GetBalancesAsync(string? accountId, CancellationToken ct)
        {
            var balances = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!balances)
                return balances.As<IEnumerable<Balance>>(null);

            return balances.As(balances.Data.Select(d => new Balance
            {
                SourceObject = d,
                Asset = d.Key,
                Available = d.Value.Available,
                Total = d.Value.Frozen + d.Value.Available
            }));
        }

        private static KlineInterval GetKlineIntervalFromTimespan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.FromMinutes(1)) return KlineInterval.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(3)) return KlineInterval.ThreeMinutes;
            if (timeSpan == TimeSpan.FromMinutes(5)) return KlineInterval.FiveMinutes;
            if (timeSpan == TimeSpan.FromMinutes(15)) return KlineInterval.FiveMinutes;
            if (timeSpan == TimeSpan.FromMinutes(30)) return KlineInterval.ThirtyMinutes;
            if (timeSpan == TimeSpan.FromHours(1)) return KlineInterval.OneHour;
            if (timeSpan == TimeSpan.FromHours(2)) return KlineInterval.TwoHours;
            if (timeSpan == TimeSpan.FromHours(4)) return KlineInterval.FourHours;
            if (timeSpan == TimeSpan.FromHours(6)) return KlineInterval.SixHours;
            if (timeSpan == TimeSpan.FromHours(12)) return KlineInterval.TwelveHours;
            if (timeSpan == TimeSpan.FromDays(1)) return KlineInterval.OneDay;
            if (timeSpan == TimeSpan.FromDays(3)) return KlineInterval.ThreeDays;
            if (timeSpan == TimeSpan.FromDays(7)) return KlineInterval.OneWeek;

            throw new ArgumentException("Unsupported timespan for CoinEx Klines, check supported intervals using CoinEx.Net.Objects.KlineInterval");
        }
        #endregion

        /// <inheritdoc />
        protected override Error ParseErrorResponse(int httpStatusCode, IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, IMessageAccessor accessor)
        {
            if (!accessor.IsJson)
                return new ServerError(accessor.GetOriginalString());

            var code = accessor.GetValue<int?>(MessagePath.Get().Property("code"));
            var msg = accessor.GetValue<string>(MessagePath.Get().Property("message"));
            if (msg == null)
                return new ServerError(accessor.GetOriginalString());

            if (code == null)
                return new ServerError(msg);

            return new ServerError(code.Value, msg);
        }

        /// <inheritdoc />
        public override TimeSyncInfo? GetTimeSyncInfo() => null;

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset() => null;

        /// <inheritdoc />
        public ISpotClient CommonSpotClient => this;
    }
}
