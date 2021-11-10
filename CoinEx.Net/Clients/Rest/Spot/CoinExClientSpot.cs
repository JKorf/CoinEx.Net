using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CryptoExchange.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ExchangeInterfaces;
using Newtonsoft.Json.Linq;
using CryptoExchange.Net.Interfaces;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.Rest.Spot;

namespace CoinEx.Net.Clients.Rest.Spot
{
    /// <summary>
    /// Client for the CoinEx REST API
    /// </summary>
    public class CoinExClientSpot : RestClient, ICoinExClientSpot, IExchangeClient
    {
        #region fields
        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client. Note that this does not trigger when using CancelAllOrdersAsync
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderCanceled;
        #endregion

        #region Subclients
        public ICoinExClientSpotAccount Account { get; }
        public ICoinExClientSpotExchangeData ExchangeData { get; }
        public ICoinExClientSpotTrading Trading { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExClient with default options
        /// </summary>
        public CoinExClientSpot() : this(CoinExClientSpotOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of CoinExClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public CoinExClientSpot(CoinExClientSpotOptions options): base("CoinEx", options, options.ApiCredentials == null ? null : new CoinExAuthenticationProvider(options.ApiCredentials, options.NonceProvider))
        {
            manualParseError = true;
            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;

            Account = new CoinExClientSpotAccount(this);
            ExchangeData = new CoinExClientSpotExchangeData(this);
            Trading = new CoinExClientSpotTrading(this);
        }
        #endregion

        #region methods
        #region public
        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(CoinExClientSpotOptions options)
        {
            CoinExClientSpotOptions.Default = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        /// <param name="nonceProvider">Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that</param>
        public void SetApiCredentials(string apiKey, string apiSecret, INonceProvider? nonceProvider = null)
        {
            SetAuthenticationProvider(new CoinExAuthenticationProvider(new ApiCredentials(apiKey, apiSecret), nonceProvider));
        }
        #endregion

        #region private

        /// <inheritdoc />
        protected override Task<ServerError?> TryParseErrorAsync(JToken data)
        {
            if (data["code"] != null && data["message"] != null)
            {
                if (data["code"]!.Value<int>() != 0)
                {
                    return Task.FromResult((ServerError?)ParseErrorResponse(data));
                }
            }

            return Task.FromResult((ServerError?) null);
        }

        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (error["code"] == null || error["message"] == null)
                return new ServerError(error.ToString());

            return new ServerError((int)error["code"]!, (string)error["message"]!);
        }

        internal async Task<WebCallResult<T>> Execute<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<T>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }
        internal async Task<WebCallResult> Execute(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) 
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<object>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        internal async Task<WebCallResult<CoinExPagedResult<T>>> ExecutePaged<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<CoinExPagedResult<T>>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        private static WebCallResult<T> GetResult<T>(WebCallResult<CoinExApiResult<T>> result) where T : class
        {
            if (result.Error != null || result.Data == null)
                return WebCallResult<T>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error ?? new UnknownError("No data received"));

            return result.As(result.Data.Data);
        }

        private static WebCallResult GetResult(WebCallResult<CoinExApiResult<object>> result) 
        {
            if (result.Error != null || result.Data == null)
                return WebCallResult.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error ?? new UnknownError("No data received"));

            return new WebCallResult(result.ResponseStatusCode, result.ResponseHeaders, null);
        }
        internal Uri GetUrl(string endpoint)
        {
            return new Uri(ClientOptions.BaseAddress + endpoint);
        }

        internal void InvokeOrderPlaced(ICommonOrderId id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(ICommonOrderId id)
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
#pragma warning disable 1066
        async Task<WebCallResult<IEnumerable<ICommonSymbol>>> IExchangeClient.GetSymbolsAsync()
        {
            var symbols = await ExchangeData.GetSymbolInfoAsync().ConfigureAwait(false);
            return symbols.As<IEnumerable<ICommonSymbol>>(symbols.Data?.Select(d => d.Value));
        }

        async Task<WebCallResult<ICommonTicker>> IExchangeClient.GetTickerAsync(string symbol)
        {
            var tickers = await ExchangeData.GetTickerAsync(symbol).ConfigureAwait(false);
            return tickers.As<ICommonTicker>(tickers.Data?.Ticker);
        }

        async Task<WebCallResult<IEnumerable<ICommonTicker>>> IExchangeClient.GetTickersAsync()
        {
            var tickers = await ExchangeData.GetTickersAsync().ConfigureAwait(false);
            return tickers.As<IEnumerable<ICommonTicker>>(tickers.Data?.Tickers.Select(d => d.Value));
        }

        async Task<WebCallResult<IEnumerable<ICommonKline>>> IExchangeClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if(startTime != null || endTime != null)
                return WebCallResult<IEnumerable<ICommonKline>>.CreateErrorResult(new ArgumentError($"CoinEx does not support the {nameof(startTime)}/{nameof(endTime)} parameters for the method {nameof(IExchangeClient.GetKlinesAsync)}"));

            var klines = await ExchangeData.GetKlinesAsync(symbol, GetKlineIntervalFromTimespan(timespan), limit).ConfigureAwait(false);
            return klines.As<IEnumerable<ICommonKline>>(klines.Data);
        }

        async Task<WebCallResult<ICommonOrderBook>> IExchangeClient.GetOrderBookAsync(string symbol)
        {
            var book = await ExchangeData.GetOrderBookAsync(symbol, 0).ConfigureAwait(false);
            return book.As<ICommonOrderBook>(book.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonRecentTrade>>> IExchangeClient.GetRecentTradesAsync(string symbol)
        {
            var trades = await ExchangeData.GetTradeHistoryAsync(symbol).ConfigureAwait(false);
            return trades.As<IEnumerable<ICommonRecentTrade>>(trades.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.PlaceOrderAsync(string symbol, IExchangeClient.OrderSide side, IExchangeClient.OrderType type, decimal quantity, decimal? price = null, string? accountId = null)
        {
            if(price == null && type == IExchangeClient.OrderType.Limit)
                return WebCallResult<ICommonOrderId>.CreateErrorResult(new ArgumentError($"Price parameter null while placing a limit order"));

            var result = await Trading.PlaceOrderAsync(
                symbol,
                type == IExchangeClient.OrderType.Limit ? OrderType.Limit : OrderType.Market,
                side == IExchangeClient.OrderSide.Sell ? OrderSide.Sell : OrderSide.Buy,
                quantity,
                price).ConfigureAwait(false);

            return result.As<ICommonOrderId>(result.Data);
        }

        async Task<WebCallResult<ICommonOrder>> IExchangeClient.GetOrderAsync(string orderId, string? symbol = null)
        {
            if (string.IsNullOrEmpty(symbol))
                return WebCallResult<ICommonOrder>.CreateErrorResult(new ArgumentError($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(IExchangeClient.GetOrderAsync)}"));

            var order = await Trading.GetOrderAsync(long.Parse(orderId), symbol!).ConfigureAwait(false);
            return order.As<ICommonOrder>(order.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonTrade>>> IExchangeClient.GetTradesAsync(string orderId, string? symbol = null)
        {
            var result = await Trading.GetOrderTradesAsync(long.Parse(orderId), 1, 100).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonTrade>>(result.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetOpenOrdersAsync(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(IExchangeClient.GetOpenOrdersAsync)}");

            var openOrders = await Trading.GetOpenOrdersAsync(symbol!, 1, 100).ConfigureAwait(false);
            return openOrders.As<IEnumerable<ICommonOrder>>(openOrders.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetClosedOrdersAsync(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(IExchangeClient.GetClosedOrdersAsync)}");

            var result = await Trading.GetClosedOrdersAsync(symbol!, 1, 100).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonOrder>>(result.Data?.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.CancelOrderAsync(string orderId, string? symbol)
        {
            if (symbol == null)
                return WebCallResult<ICommonOrderId>.CreateErrorResult(new ArgumentError(nameof(symbol) + " required for CoinEx " + nameof(IExchangeClient.CancelOrderAsync)));

            var result = await Trading.CancelOrderAsync(symbol, long.Parse(orderId)).ConfigureAwait(false);
            return result.As<ICommonOrderId>(result.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonBalance>>> IExchangeClient.GetBalancesAsync(string? accountId = null)
        {
            var balances = await Account.GetBalancesAsync().ConfigureAwait(false);
            return balances.As<IEnumerable<ICommonBalance>>(balances.Data?.Select(d => d.Value));
        }
#pragma warning restore 1066

        private static KlineInterval GetKlineIntervalFromTimespan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.FromMinutes(1)) return KlineInterval.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(3)) return KlineInterval.ThreeMinute;
            if (timeSpan == TimeSpan.FromMinutes(5)) return KlineInterval.FiveMinute;
            if (timeSpan == TimeSpan.FromMinutes(15)) return KlineInterval.FiveMinute;
            if (timeSpan == TimeSpan.FromMinutes(30)) return KlineInterval.ThirtyMinute;
            if (timeSpan == TimeSpan.FromHours(1)) return KlineInterval.OneHour;
            if (timeSpan == TimeSpan.FromHours(2)) return KlineInterval.TwoHour;
            if (timeSpan == TimeSpan.FromHours(4)) return KlineInterval.FourHour;
            if (timeSpan == TimeSpan.FromHours(6)) return KlineInterval.SixHour;
            if (timeSpan == TimeSpan.FromHours(12)) return KlineInterval.TwelveHour;
            if (timeSpan == TimeSpan.FromDays(1)) return KlineInterval.OneDay;
            if (timeSpan == TimeSpan.FromDays(3)) return KlineInterval.ThreeDay;
            if (timeSpan == TimeSpan.FromDays(7)) return KlineInterval.OneWeek;

            throw new ArgumentException("Unsupported timespan for CoinEx Klines, check supported intervals using CoinEx.Net.Objects.KlineInterval");
        }
        #endregion
    }
}
