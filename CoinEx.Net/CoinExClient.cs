using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinEx.Net
{
    public class CoinExClient : ExchangeClient
    {
        #region fields
        private static CoinExClientOptions defaultOptions = new CoinExClientOptions();

        private string baseAddress;
        private string userAgent;

        private const string MarketListEndpoint = "/market/list";
        private const string MarketStatisticsEndpoint = "/market/ticker";
        private const string MarketStatisticsListEndpoint = "/market/ticker/all";
        private const string MarketDepthEndpoint = "/market/depth";
        private const string MarketDealsEndpoint = "/market/deals";
        private const string MarketKlinesEndpoint = "/market/kline";

        private const string AccountInfoEndpoint = "/balance/info";
        private const string WithdrawalHistoryEndpoint = "/balance/coin/withdraw";

        private const string PlaceLimitOrderEndpoint = "/order/limit";
        private const string PlaceMarketOrderEndpoint = "/order/market";
        private const string PlaceImmediateOrCancelOrderEndpoint = "/order/ioc";

        private const string FinishedOrdersEndpoint = "/order/finished";
        private const string OpenOrdersEndpoint = "/order/pending";
        private const string OrderStatusEndpoint = "/order/status";
        private const string OrderDetailsEndpoint = "/order/deals";
        private const string UserTransactionsEndpoint = "/order/user/deals";
        private const string CancelOrderEndpoint = "/order/pending";
        private const string MiningDifficultyEndpoint = "/order/mining/difficulty";

        #endregion

        #region ctor
        public CoinExClient() : this(defaultOptions)
        {
        }

        public CoinExClient(CoinExClientOptions options): base(options, options.ApiCredentials == null ? null : new CoinExAuthenticationProvider(options.ApiCredentials))
        {
            Configure(options);
        }
        #endregion

        #region methods
        public static void SetDefaultOptions(CoinExClientOptions options)
        {
            defaultOptions = options;
        }

        public CallResult<string[]> GetMarketList() => GetMarketListAsync().Result;
        public async Task<CallResult<string[]>> GetMarketListAsync()
        {
            return await Execute<string[]>(GetUrl(MarketListEndpoint)).ConfigureAwait(false);
        }

        public CallResult<CoinExMarketState> GetMarketStatistics(string market) => GetMarketStatisticsAsync(market).Result;
        public async Task<CallResult<CoinExMarketState>> GetMarketStatisticsAsync(string market)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", market }
            };

            return await Execute<CoinExMarketState>(GetUrl(MarketStatisticsEndpoint), parameters:parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExMarketStatesList> GetMarketStates() => GetMarketStatesAsync().Result;
        public async Task<CallResult<CoinExMarketStatesList>> GetMarketStatesAsync()
        {
            return await Execute<CoinExMarketStatesList>(GetUrl(MarketStatisticsListEndpoint)).ConfigureAwait(false);
        }

        public CallResult<CoinExMarketDepth> GetMarketDepth(string market, int mergeDepth, int? limit = null) => GetMarketDepthAsync(market, mergeDepth, limit).Result;
        public async Task<CallResult<CoinExMarketDepth>> GetMarketDepthAsync(string market, int mergeDepth, int? limit = null)
        {
            if (mergeDepth < 0 || mergeDepth > 8)
                return new CallResult<CoinExMarketDepth>(null, new ArgumentError("Merge depth needs to be between 0 - 8"));

            if (limit.HasValue && limit != 5 && limit != 10 && limit != 20)
                return new CallResult<CoinExMarketDepth>(null, new ArgumentError("Limit should be 5 / 10 / 20"));

            string merge = "0";
            if (mergeDepth != 8)
            {
                merge += ".";
                for (int i = 0; i < 7 - mergeDepth; i++)
                    merge += "0";
                merge += "1";
            }

            var parameters = new Dictionary<string, object>
            {
                { "market", market },
                { "merge", merge }
            };
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<CoinExMarketDepth>(GetUrl(MarketDepthEndpoint), parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExMarketTransaction[]> GetLatestTransactionData(string market, long? lastId = null) => GetLatestTransactionDataAsync(market, lastId).Result;
        public async Task<CallResult<CoinExMarketTransaction[]>> GetLatestTransactionDataAsync(string market, long? lastId = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", market }
            };
            parameters.AddOptionalParameter("last_id", lastId);

            return await Execute<CoinExMarketTransaction[]>(GetUrl(MarketDealsEndpoint), parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExKline[]> GetKlines(string market, KlineInterval interval, int? limit = null) => GetKlinesAsync(market, interval, limit).Result;
        public async Task<CallResult<CoinExKline[]>> GetKlinesAsync(string market, KlineInterval interval, int? limit = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", market },
                { "type", JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)) },
            };
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<CoinExKline[]>(GetUrl(MarketKlinesEndpoint), parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<Dictionary<string, CoinExBalance>> GetBalances() => GetBalancesAsync().Result;
        public async Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync()
        {
            return await Execute<Dictionary<string, CoinExBalance>>(GetUrl(AccountInfoEndpoint), signed: true).ConfigureAwait(false);
        }

        public CallResult<Dictionary<string, CoinExWithdrawal[]>> GetWitdrawalHistory(string coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null) => GetWitdrawalHistoryAsync(coin, coinWithdrawId, page, limit).Result;
        public async Task<CallResult<Dictionary<string, CoinExWithdrawal[]>>> GetWitdrawalHistoryAsync(string coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin", coin);
            parameters.AddOptionalParameter("coin_withdraw_id", coinWithdrawId);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<Dictionary<string, CoinExWithdrawal[]>>(GetUrl(WithdrawalHistoryEndpoint), signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExOrder> PlaceLimitOrder(string market, TransactionType type, decimal amount, decimal price, string sourceId = null) => PlaceLimitOrderAsync(market, type, amount, price, sourceId).Result;
        public async Task<CallResult<CoinExOrder>> PlaceLimitOrderAsync(string market, TransactionType type, decimal amount, decimal price, string sourceId = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            return await Execute<CoinExOrder>(GetUrl(PlaceLimitOrderEndpoint), method: "POST", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExOrder> PlaceMarketOrder(string market, TransactionType type, decimal amount, string sourceId = null) => PlaceMarketOrderAsync(market, type, amount, sourceId).Result;
        public async Task<CallResult<CoinExOrder>> PlaceMarketOrderAsync(string market, TransactionType type, decimal amount, string sourceId = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            return await Execute<CoinExOrder>(GetUrl(PlaceMarketOrderEndpoint), method: "POST", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExOrder> PlaceImmediateOrCancelOrder(string market, TransactionType type, decimal amount, decimal price, string sourceId = null) => PlaceImmediateOrCancelOrderAsync(market, type, amount, price, sourceId).Result;
        public async Task<CallResult<CoinExOrder>> PlaceImmediateOrCancelOrderAsync(string market, TransactionType type, decimal amount, decimal price, string sourceId = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            return await Execute<CoinExOrder>(GetUrl(PlaceImmediateOrCancelOrderEndpoint), method: "POST", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExPagedResult<CoinExOrder[]>> GetOpenOrders(string market, int page, int limit) => GetOpenOrdersAsync(market, page, limit).Result;
        public async Task<CallResult<CoinExPagedResult<CoinExOrder[]>>> GetOpenOrdersAsync(string market, int page, int limit)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "page", page },
                { "limit", limit },
            };

            return await ExecutePaged<CoinExOrder[]>(GetUrl(OpenOrdersEndpoint), method: "GET", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExPagedResult<CoinExOrder[]>> GetExecutedOrders(string market, int page, int limit) => GetExecutedOrdersAsync(market, page, limit).Result;
        public async Task<CallResult<CoinExPagedResult<CoinExOrder[]>>> GetExecutedOrdersAsync(string market, int page, int limit)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "page", page },
                { "limit", limit },
            };

            return await ExecutePaged<CoinExOrder[]>(GetUrl(FinishedOrdersEndpoint), method: "GET", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExOrder> GetOrderStatus(long orderId, string market) => GetOrderStatusAsync(orderId, market).Result;
        public async Task<CallResult<CoinExOrder>> GetOrderStatusAsync(long orderId, string market)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "id", orderId },
            };

            return await Execute<CoinExOrder>(GetUrl(OrderStatusEndpoint), method: "GET", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExPagedResult<CoinExOrderTransaction[]>> GetExecutedOrderDetails(long orderId, int page, int limit) => GetExecutedOrderDetailsAsync(orderId, page, limit).Result;
        public async Task<CallResult<CoinExPagedResult<CoinExOrderTransaction[]>>> GetExecutedOrderDetailsAsync(long orderId, int page, int limit)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", orderId },
                { "page", page },
                { "limit", limit },
            };

            return await ExecutePaged<CoinExOrderTransaction[]>(GetUrl(OrderDetailsEndpoint), method: "GET", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExPagedResult<CoinExOrderTransactionExtended[]>> GetTransactions(string market, int page, int limit) => GetTransactionsAsync(market, page, limit).Result;
        public async Task<CallResult<CoinExPagedResult<CoinExOrderTransactionExtended[]>>> GetTransactionsAsync(string market, int page, int limit)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "page", page },
                { "limit", limit },
            };

            return await ExecutePaged<CoinExOrderTransactionExtended[]>(GetUrl(UserTransactionsEndpoint), method: "GET", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExOrder> CancelOrder(string market, long orderId) => CancelOrderAsync(market, orderId).Result;
        public async Task<CallResult<CoinExOrder>> CancelOrderAsync(string market, long orderId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "id", orderId }
            };

            return await Execute<CoinExOrder>(GetUrl(CancelOrderEndpoint), method: "DELETE", signed: true, parameters: parameters).ConfigureAwait(false);
        }

        public CallResult<CoinExMiningDifficulty> GetMiningDifficulty() => GetMiningDifficultyAsync().Result;
        public async Task<CallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync()
        {
            return await Execute<CoinExMiningDifficulty>(GetUrl(MiningDifficultyEndpoint), signed: true).ConfigureAwait(false);
        }

        protected override IRequest ConstructRequest(Uri uri, string method, Dictionary<string, object> parameters, bool signed)
        {
            var uriString = uri.ToString();
            var paramString = GetParamString(signed, parameters);

            if (method == "GET" || method == "DELETE")
                uriString += "?" + paramString;

            var request = RequestFactory.Create(uriString);
            request.Method = method;
            request.ContentType = "application/json; charset=utf-8";

            if (signed)
            {
                request.Headers.Add("authorization", GetSignData(paramString));
            }

            if (parameters != null && method == "POST")
            {
                var stringData = JsonConvert.SerializeObject(parameters);
                var data = Encoding.UTF8.GetBytes(stringData);
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream().Result)
                    stream.Write(data, 0, data.Length);
            }

            return request;
        }

        private string GetParamString(bool signed, Dictionary<string, object> parameters)
        {
            if (!signed && (parameters == null || parameters.Count == 0))
                return "";

            if (signed)
            {
                if (parameters == null)
                    parameters = new Dictionary<string, object>();

                parameters.Add("access_id", authProvider.Credentials.Key.GetString());
                parameters.Add("tonce", GetTimestamp());
                parameters = parameters.OrderBy(p => p.Key).ToDictionary(k => k.Key, v => v.Value);
            }

            return string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
        }

        private string GetSignData(string paramString)
        {
            string signString = paramString + "&secret_key=" + authProvider.Credentials.Secret.GetString();
            return authProvider.Sign(signString);            
        }

        private async Task<CallResult<T>> Execute<T>(Uri uri, bool signed = false, string method = "GET", Dictionary<string, object> parameters = null) where T : class
        {
            return GetResult(await ExecuteRequest<CoinExApiResult<T>>(uri, method, parameters, signed).ConfigureAwait(false));
        }

        private async Task<CallResult<CoinExPagedResult<T>>> ExecutePaged<T>(Uri uri, bool signed = false, string method = "GET", Dictionary<string, object> parameters = null) where T : class
        {
            return GetResult(await ExecuteRequest<CoinExApiResult<CoinExPagedResult<T>>>(uri, method, parameters, signed).ConfigureAwait(false));
        }

        private static CallResult<T> GetResult<T>(CallResult<CoinExApiResult<T>> result) where T : class
        {
            if (result.Error != null || result.Data == null)
                return new CallResult<T>(null, result.Error);

            var error = result.Data.Code != 0;
            return new CallResult<T>(error ? null : result.Data.Data, error ? new ServerError(result.Data.Code, result.Data.Message) : null);
        }

        private Uri GetUrl(string endpoint)
        {
            return new Uri(baseAddress + endpoint);
        }

        private long GetTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private void Configure(CoinExClientOptions options)
        {
            base.Configure(options);
            baseAddress = options.BaseAddress;
            userAgent = options.UserAgent;
        }
        #endregion
    }
}
