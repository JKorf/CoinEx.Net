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

namespace CoinEx.Net
{
    /// <summary>
    /// Client for the CoinEx REST API
    /// </summary>
    public class CoinExClient : RestClient, ICoinExClient, IExchangeClient
    {
        #region fields
        private static CoinExClientOptions defaultOptions = new CoinExClientOptions();
        private static CoinExClientOptions DefaultOptions => defaultOptions.Copy<CoinExClientOptions>();

        private const string AssetConfigEndpoint = "common/asset/config";
        private const string CurrencyRateEndpoint = "common/currency/rate";

        private const string MarketListEndpoint = "market/list";
        private const string MarketStatisticsEndpoint = "market/ticker";
        private const string MarketStatisticsListEndpoint = "market/ticker/all";
        private const string MarketDepthEndpoint = "market/depth";
        private const string MarketDealsEndpoint = "market/deals";
        private const string MarketKlinesEndpoint = "market/kline";
        private const string MarketInfoEndpoint = "market/info";

        private const string AccountInfoEndpoint = "balance/info";
        private const string WithdrawalHistoryEndpoint = "balance/coin/withdraw";
        private const string DepositHistoryEndpoint = "balance/coin/deposit";
        private const string WithdrawEndpoint = "balance/coin/withdraw";
        private const string CancelWithdrawalEndpoint = "balance/coin/withdraw";
        private const string DepositAddressEndpoint = "balance/deposit/address/";

        private const string PlaceLimitOrderEndpoint = "order/limit";
        private const string PlaceMarketOrderEndpoint = "order/market";
        private const string PlaceStopLimitOrderEndpoint = "order/stop/limit";
        private const string PlaceStopMarketOrderEndpoint = "order/stop/market";
        private const string PlaceImmediateOrCancelOrderEndpoint = "order/ioc";

        private const string FinishedOrdersEndpoint = "order/finished";
        private const string OpenOrdersEndpoint = "order/pending";
        private const string OpenStopOrdersEndpoint = "order/stop/pending";
        private const string OrderStatusEndpoint = "order/status";
        private const string OrderDetailsEndpoint = "order/deals";
        private const string UserTransactionsEndpoint = "order/user/deals";
        private const string CancelOrderEndpoint = "order/pending";
        private const string CancelStopOrderEndpoint = "order/stop/pending";
        private const string MiningDifficultyEndpoint = "order/mining/difficulty";

        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client. Note that this does not trigger when using CancelAllOrdersAsync
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderCanceled;
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExClient with default options
        /// </summary>
        public CoinExClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of CoinExClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public CoinExClient(CoinExClientOptions options): base("CoinEx", options, options.ApiCredentials == null ? null : new CoinExAuthenticationProvider(options.ApiCredentials, options.NonceProvider))
        {
            manualParseError = true;
            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;
        }
        #endregion

        #region methods
        #region public
        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(CoinExClientOptions options)
        {
            defaultOptions = options;
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

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await Execute<IEnumerable<string>>(GetUrl(MarketListEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, decimal>>> GetCurrencyRateAsync(CancellationToken ct = default)
        {
            return await Execute<Dictionary<string, decimal>>(GetUrl(CurrencyRateEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, CoinExAssetConfig>>> GetAssetConfigAsync(string? assetType = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", assetType);

            return await Execute< Dictionary<string, CoinExAssetConfig>>(GetUrl(AssetConfigEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExSymbolState>> GetTickerAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };

            return await Execute<CoinExSymbolState>(GetUrl(MarketStatisticsEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExSymbolStatesList>> GetTickersAsync(CancellationToken ct = default)
        {
            var data = await Execute<CoinExSymbolStatesList>(GetUrl(MarketStatisticsListEndpoint), HttpMethod.Get, ct)
                .ConfigureAwait(false);
            if (!data)
                return data;

            foreach (var item in data.Data.Tickers)
                item.Value.Symbol = item.Key;
            return data;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int mergeDepth, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit?.ValidateIntValues(nameof(limit), 5, 10, 20);

            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "merge", CoinExHelpers.MergeDepthIntToString(mergeDepth) }
            };
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<CoinExOrderBook>(GetUrl(MarketDepthEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExSymbolTrade>>> GetTradeHistoryAsync(string symbol, long? fromId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };
            parameters.AddOptionalParameter("last_id", fromId);

            return await Execute<IEnumerable<CoinExSymbolTrade>>(GetUrl(MarketDealsEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };
            return await Execute<Dictionary<string, CoinExSymbol>>(GetUrl(MarketInfoEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(CancellationToken ct = default)
        {
            return await Execute<Dictionary<string, CoinExSymbol>>(GetUrl(MarketInfoEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)) }
            };
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<IEnumerable<CoinExKline>>(GetUrl(MarketKlinesEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            var result = await Execute<Dictionary<string, CoinExBalance>>(GetUrl(AccountInfoEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
            if (result)
            {
                foreach (var b in result.Data)
                    b.Value.Symbol = b.Key;
            }

            return result;
        }

        /// <inheritdoc />
		public async Task<WebCallResult<CoinExPagedResult<CoinExDeposit>>> GetDepositHistoryAsync(string? asset = null, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", asset);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await ExecutePaged<CoinExDeposit>(GetUrl(DepositHistoryEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? asset = null, long? withdrawId = null, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", asset);
            parameters.AddOptionalParameter("coin_withdraw_id", withdrawId);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<CoinExPagedResult<CoinExWithdrawal>>(GetUrl(WithdrawalHistoryEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string asset, string address, bool localTransfer, decimal quantity, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            address.ValidateNotNull(nameof(address));
            var parameters = new Dictionary<string, object>
            {
                { "coin_type", asset },
                { "coin_address", address },
                { "transfer_method", localTransfer ? "local": "onchain" },
                { "actual_amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };

            return await Execute<CoinExWithdrawal>(GetUrl(WithdrawEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<bool>> CancelWithdrawalAsync(long withdrawId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "coin_withdraw_id", withdrawId }
            };

            var result = await Execute<object>(GetUrl(CancelWithdrawalEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
            return result.As(result.Success);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string? smartContractName = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("smart_contract_name", smartContractName);

            return await Execute<CoinExDepositAddress>(GetUrl(DepositAddressEndpoint + asset), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> PlaceOrderAsync(
            string symbol, 
            OrderType type,
            OrderSide side, 
            decimal quantity, 
            
            decimal? price = null, 
            decimal? stopPrice = null,
            bool? immediateOrCancel = null,
            OrderOption? orderOption = null,
            string? clientId = null, 
            string? sourceId = null, 
            CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();

            var endpoint = "";
            if (type == OrderType.Limit)
                endpoint = PlaceLimitOrderEndpoint;
            else if (type == OrderType.Market)
                endpoint = PlaceMarketOrderEndpoint;
            else if (type == OrderType.StopLimit)
                endpoint = PlaceStopLimitOrderEndpoint;
            else if (type == OrderType.StopMarket)
                endpoint = PlaceStopMarketOrderEndpoint;

            if (immediateOrCancel == true)
            {
                if (type != OrderType.Limit)
                    throw new ArgumentException("ImmediateOrCancel only valid for limit orders");

                endpoint = PlaceImmediateOrCancelOrderEndpoint;
            }

            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("price", price?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("option", orderOption.HasValue ? JsonConvert.SerializeObject(orderOption, new OrderOptionConverter(false)) : null);
            parameters.AddOptionalParameter("stopPrice", stopPrice?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("client_id", clientId);
            parameters.AddOptionalParameter("source_id", sourceId);

            var result = await Execute<CoinExOrder>(GetUrl(endpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                OnOrderPlaced?.Invoke(result.Data);

            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrder>(GetUrl(OpenOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenStopOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrder>(GetUrl(OpenStopOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetExecutedOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrder>(GetUrl(FinishedOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> GetOrderAsync(long orderId, string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "id", orderId }
            };

            return await Execute<CoinExOrder>(GetUrl(OrderStatusEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrderTrade>>> GetOrderTradesAsync(long orderId, int page, int limit, CancellationToken ct = default)
        {
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "id", orderId },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrderTrade>(GetUrl(OrderDetailsEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrderTradeExtended>>> GetUserTradesAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrderTradeExtended>(GetUrl(UserTransactionsEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "id", orderId }
            };

            var result = await Execute<CoinExOrder>(GetUrl(CancelOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
            if (result)
                OnOrderCanceled?.Invoke(result.Data);
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult> CancelAllOrdersAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
            };

            return await Execute(GetUrl(CancelOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> CancelAllStopOrdersAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
            };

            return await Execute(GetUrl(CancelStopOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync(CancellationToken ct = default)
        {
            return await Execute<CoinExMiningDifficulty>(GetUrl(MiningDifficultyEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
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

        private async Task<WebCallResult<T>> Execute<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<T>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }
        private async Task<WebCallResult> Execute(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) 
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<object>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        private async Task<WebCallResult<CoinExPagedResult<T>>> ExecutePaged<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
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
        private Uri GetUrl(string endpoint)
        {
            return new Uri(BaseAddress + endpoint);
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
            var symbols = await GetSymbolInfoAsync().ConfigureAwait(false);
            return symbols.As<IEnumerable<ICommonSymbol>>(symbols.Data?.Select(d => d.Value));
        }

        async Task<WebCallResult<ICommonTicker>> IExchangeClient.GetTickerAsync(string symbol)
        {
            var tickers = await GetTickerAsync(symbol).ConfigureAwait(false);
            return tickers.As<ICommonTicker>(tickers.Data?.Ticker);
        }

        async Task<WebCallResult<IEnumerable<ICommonTicker>>> IExchangeClient.GetTickersAsync()
        {
            var tickers = await GetTickersAsync().ConfigureAwait(false);
            return tickers.As<IEnumerable<ICommonTicker>>(tickers.Data?.Tickers.Select(d => d.Value));
        }

        async Task<WebCallResult<IEnumerable<ICommonKline>>> IExchangeClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if(startTime != null || endTime != null)
                return WebCallResult<IEnumerable<ICommonKline>>.CreateErrorResult(new ArgumentError($"CoinEx does not support the {nameof(startTime)}/{nameof(endTime)} parameters for the method {nameof(IExchangeClient.GetKlinesAsync)}"));

            var klines = await GetKlinesAsync(symbol, GetKlineIntervalFromTimespan(timespan), limit).ConfigureAwait(false);
            return klines.As<IEnumerable<ICommonKline>>(klines.Data);
        }

        async Task<WebCallResult<ICommonOrderBook>> IExchangeClient.GetOrderBookAsync(string symbol)
        {
            var book = await GetOrderBookAsync(symbol, 0).ConfigureAwait(false);
            return book.As<ICommonOrderBook>(book.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonRecentTrade>>> IExchangeClient.GetRecentTradesAsync(string symbol)
        {
            var trades = await GetTradeHistoryAsync(symbol).ConfigureAwait(false);
            return trades.As<IEnumerable<ICommonRecentTrade>>(trades.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.PlaceOrderAsync(string symbol, IExchangeClient.OrderSide side, IExchangeClient.OrderType type, decimal quantity, decimal? price = null, string? accountId = null)
        {
            if(price == null && type == IExchangeClient.OrderType.Limit)
                return WebCallResult<ICommonOrderId>.CreateErrorResult(new ArgumentError($"Price parameter null while placing a limit order"));

            var result = await PlaceOrderAsync(
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

            var order = await GetOrderAsync(long.Parse(orderId), symbol!).ConfigureAwait(false);
            return order.As<ICommonOrder>(order.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonTrade>>> IExchangeClient.GetTradesAsync(string orderId, string? symbol = null)
        {
            var result = await GetOrderTradesAsync(long.Parse(orderId), 1, 100).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonTrade>>(result.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetOpenOrdersAsync(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(IExchangeClient.GetOpenOrdersAsync)}");

            var openOrders = await GetOpenOrdersAsync(symbol!, 1, 100).ConfigureAwait(false);
            return openOrders.As<IEnumerable<ICommonOrder>>(openOrders.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetClosedOrdersAsync(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(IExchangeClient.GetClosedOrdersAsync)}");

            var result = await GetExecutedOrdersAsync(symbol!, 1, 100).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonOrder>>(result.Data?.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.CancelOrderAsync(string orderId, string? symbol)
        {
            if (symbol == null)
                return WebCallResult<ICommonOrderId>.CreateErrorResult(new ArgumentError(nameof(symbol) + " required for CoinEx " + nameof(IExchangeClient.CancelOrderAsync)));

            var result = await CancelOrderAsync(symbol, long.Parse(orderId)).ConfigureAwait(false);
            return result.As<ICommonOrderId>(result.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonBalance>>> IExchangeClient.GetBalancesAsync(string? accountId = null)
        {
            var balances = await GetBalancesAsync().ConfigureAwait(false);
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
