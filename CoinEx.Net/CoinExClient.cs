using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CryptoExchange.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CoinEx.Net.Interfaces;
using Newtonsoft.Json.Linq;

namespace CoinEx.Net
{
    public class CoinExClient : RestClient, ICoinExClient
    {
        #region fields
        private static CoinExClientOptions defaultOptions = new CoinExClientOptions();
        private static CoinExClientOptions DefaultOptions => defaultOptions.Copy();

        private string userAgent;

        private const string MarketListEndpoint = "/market/list";
        private const string MarketStatisticsEndpoint = "/market/ticker";
        private const string MarketStatisticsListEndpoint = "/market/ticker/all";
        private const string MarketDepthEndpoint = "/market/depth";
        private const string MarketDealsEndpoint = "/market/deals";
        private const string MarketKlinesEndpoint = "/market/kline";

        private const string AccountInfoEndpoint = "/balance/info";
        private const string WithdrawalHistoryEndpoint = "/balance/coin/withdraw";
        private const string WithdrawEndpoint = "/balance/coin/withdraw";
        private const string CancelWithdrawalEndpoint = "/balance/coin/withdraw";

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
        public CoinExClient(CoinExClientOptions options): base(options, options.ApiCredentials == null ? null : new CoinExAuthenticationProvider(options.ApiCredentials))
        {
            Configure(options);
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
        /// Gets a list of markets active on CoinEx
        /// </summary>
        /// <returns>List of market names</returns>
        public CallResult<string[]> GetMarketList() => GetMarketListAsync().Result;
        /// <summary>
        /// Gets a list of markets active on CoinEx
        /// </summary>
        /// <returns>List of market names</returns>
        public async Task<CallResult<string[]>> GetMarketListAsync()
        {
            return await Execute<string[]>(GetUrl(MarketListEndpoint)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the state of a specific market
        /// </summary>
        /// <param name="market">The market to retrieve state for</param>
        /// <returns>The state of the market</returns>
        public CallResult<CoinExMarketState> GetMarketState(string market) => GetMarketStateAsync(market).Result;
        /// <summary>
        /// Gets the state of a specific market
        /// </summary>
        /// <param name="market">The market to retrieve state for</param>
        /// <returns>The state of the market</returns>
        public async Task<CallResult<CoinExMarketState>> GetMarketStateAsync(string market)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", market }
            };

            return await Execute<CoinExMarketState>(GetUrl(MarketStatisticsEndpoint), parameters:parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the states of all markets
        /// </summary>
        /// <returns>List of states for all markets</returns>
        public CallResult<CoinExMarketStatesList> GetMarketStates() => GetMarketStatesAsync().Result;
        /// <summary>
        /// Gets the states of all markets
        /// </summary>
        /// <returns>List of states for all markets</returns>
        public async Task<CallResult<CoinExMarketStatesList>> GetMarketStatesAsync()
        {
            return await Execute<CoinExMarketStatesList>(GetUrl(MarketStatisticsListEndpoint)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the depth data for a market
        /// </summary>
        /// <param name="market">The market to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <returns>Depth data for a market</returns>
        public CallResult<CoinExMarketDepth> GetMarketDepth(string market, int mergeDepth, int? limit = null) => GetMarketDepthAsync(market, mergeDepth, limit).Result;
        /// <summary>
        /// Gets the depth data for a market
        /// </summary>
        /// <param name="market">The market to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <returns>Depth data for a market</returns>
        public async Task<CallResult<CoinExMarketDepth>> GetMarketDepthAsync(string market, int mergeDepth, int? limit = null)
        {
            if (mergeDepth < 0 || mergeDepth > 8)
                return new CallResult<CoinExMarketDepth>(null, new ArgumentError("Merge depth needs to be between 0 - 8"));

            if (limit.HasValue && limit != 5 && limit != 10 && limit != 20)
                return new CallResult<CoinExMarketDepth>(null, new ArgumentError("Limit should be 5 / 10 / 20"));
            
            var parameters = new Dictionary<string, object>
            {
                { "market", market },
                { "merge", CoinExHelpers.MergeDepthIntToString(mergeDepth) }
            };
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<CoinExMarketDepth>(GetUrl(MarketDepthEndpoint), parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the latest transactions for a market
        /// </summary>
        /// <param name="market">The market to retrieve data for</param>
        /// <param name="fromId">The id from which on to return transactions</param>
        /// <returns>List of transactions for a market</returns>
        public CallResult<CoinExMarketTransaction[]> GetLatestTransactions(string market, long? fromId = null) => GetLatestTransactionsAsync(market, lastId).Result;
        /// <summary>
        /// Gets the latest transactions for a market
        /// </summary>
        /// <param name="market">The market to retrieve data for</param>
        /// <param name="fromId">The id from which on to return transactions</param>
        /// <returns>List of transactions for a market</returns>
        public async Task<CallResult<CoinExMarketTransaction[]>> GetLatestTransactionsAsync(string market, long? fromId = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", market }
            };
            parameters.AddOptionalParameter("last_id", lastId);

            return await Execute<CoinExMarketTransaction[]>(GetUrl(MarketDealsEndpoint), parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves kline data for a specific market
        /// </summary>
        /// <param name="market">The market to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <returns>List of klines for a market</returns>
        public CallResult<CoinExKline[]> GetKlines(string market, KlineInterval interval, int? limit = null) => GetKlinesAsync(market, interval, limit).Result;
        /// <summary>
        /// Retrieves kline data for a specific market
        /// </summary>
        /// <param name="market">The market to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <returns>List of klines for a market</returns>
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

        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <returns>List of balances</returns>
        public CallResult<Dictionary<string, CoinExBalance>> GetBalances() => GetBalancesAsync().Result;
        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <returns>List of balances</returns>
        public async Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync()
        {
            return await Execute<Dictionary<string, CoinExBalance>>(GetUrl(AccountInfoEndpoint), true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <returns></returns>
        public CallResult<CoinExWithdrawal[]> GetWithdrawalHistory(string coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null) => GetWithdrawalHistoryAsync(coin, coinWithdrawId, page, limit).Result;
        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <returns></returns>
        public async Task<CallResult<CoinExWithdrawal[]>> GetWithdrawalHistoryAsync(string coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin", coin);
            parameters.AddOptionalParameter("coin_withdraw_id", coinWithdrawId);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<CoinExWithdrawal[]>(GetUrl(WithdrawalHistoryEndpoint), true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <returns>The withdrawal object</returns>
        public CallResult<CoinExWithdrawal> Withdraw(string coin, string coinAddress, decimal amount) => WithdrawAsync(coin, coinAddress, amount).Result;
        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <returns>The withdrawal object</returns>
        public async Task<CallResult<CoinExWithdrawal>> WithdrawAsync(string coin, string coinAddress, decimal amount)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "coin_type", coin },
                { "coin_address", coinAddress },
                { "actual_amount", amount.ToString(CultureInfo.InvariantCulture) },
            };

            return await Execute<CoinExWithdrawal>(GetUrl(WithdrawEndpoint), method: Constants.PostMethod, signed: true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <returns>True if successful, false otherwise</returns>
        public CallResult<bool> CancelWithdrawal(long coinWithdrawId) => CancelWithdrawalAsync(coinWithdrawId).Result;
        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<CallResult<bool>> CancelWithdrawalAsync(long coinWithdrawId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "coin_withdraw_id", coinWithdrawId },
            };

            var result = await Execute<object>(GetUrl(CancelWithdrawalEndpoint), method: Constants.DeleteMethod, signed: true, parameters: parameters).ConfigureAwait(false);
            if (!result.Success)
                return new CallResult<bool>(false, result.Error);
            return new CallResult<bool>(true, null);
        }

        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns>Details of the order that was placed</returns>
        public CallResult<CoinExOrder> PlaceLimitOrder(string market, TransactionType type, decimal amount, decimal price, string sourceId = null) => PlaceLimitOrderAsync(market, type, amount, price, sourceId).Result;
        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns>Details of the order that was placed</returns>
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

            return await Execute<CoinExOrder>(GetUrl(PlaceLimitOrderEndpoint), method: Constants.PostMethod, signed: true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns>Details of the order that was placed</returns>
        public CallResult<CoinExOrder> PlaceMarketOrder(string market, TransactionType type, decimal amount, string sourceId = null) => PlaceMarketOrderAsync(market, type, amount, sourceId).Result;
        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns>Details of the order that was placed</returns>
        public async Task<CallResult<CoinExOrder>> PlaceMarketOrderAsync(string market, TransactionType type, decimal amount, string sourceId = null)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            return await Execute<CoinExOrder>(GetUrl(PlaceMarketOrderEndpoint), method: Constants.PostMethod, signed: true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Places an order which should be filled immediately uppon placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns></returns>
        public CallResult<CoinExOrder> PlaceImmediateOrCancelOrder(string market, TransactionType type, decimal amount, decimal price, string sourceId = null) => PlaceImmediateOrCancelOrderAsync(market, type, amount, price, sourceId).Result;
        /// <summary>
        /// Places an order which should be filled immediately uppon placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns></returns>
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

            return await Execute<CoinExOrder>(GetUrl(PlaceImmediateOrCancelOrderEndpoint), method: Constants.PostMethod, signed: true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of open orders for a market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of open orders for a market</returns>
        public CallResult<CoinExPagedResult<CoinExOrder>> GetOpenOrders(string market, int page, int limit) => GetOpenOrdersAsync(market, page, limit).Result;
        /// <summary>
        /// Retrieves a list of open orders for a market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of open orders for a market</returns>
        public async Task<CallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string market, int page, int limit)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "page", page },
                { "limit", limit },
            };

            return await ExecutePaged<CoinExOrder>(GetUrl(OpenOrdersEndpoint), true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of executed orders for a market in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of executed orders for a market</returns>
        public CallResult<CoinExPagedResult<CoinExOrder>> GetExecutedOrders(string market, int page, int limit) => GetExecutedOrdersAsync(market, page, limit).Result;
        /// <summary>
        /// Retrieves a list of executed orders for a market in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of executed orders for a market</returns>
        public async Task<CallResult<CoinExPagedResult<CoinExOrder>>> GetExecutedOrdersAsync(string market, int page, int limit)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "page", page },
                { "limit", limit },
            };

            return await ExecutePaged<CoinExOrder>(GetUrl(FinishedOrdersEndpoint), true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="market">The market the order is for</param>
        /// <returns>Details of the order</returns>
        public CallResult<CoinExOrder> GetOrderStatus(long orderId, string market) => GetOrderStatusAsync(orderId, market).Result;
        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="market">The market the order is for</param>
        /// <returns>Details of the order</returns>
        public async Task<CallResult<CoinExOrder>> GetOrderStatusAsync(long orderId, string market)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "id", orderId },
            };

            return await Execute<CoinExOrder>(GetUrl(OrderStatusEndpoint), true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>Details of an executed order</returns>
        public CallResult<CoinExPagedResult<CoinExOrderTransaction>> GetExecutedOrderDetails(long orderId, int page, int limit) => GetExecutedOrderDetailsAsync(orderId, page, limit).Result;
        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>Details of an executed order</returns>
        public async Task<CallResult<CoinExPagedResult<CoinExOrderTransaction>>> GetExecutedOrderDetailsAsync(long orderId, int page, int limit)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "id", orderId },
                { "page", page },
                { "limit", limit },
            };

            return await ExecutePaged<CoinExOrderTransaction>(GetUrl(OrderDetailsEndpoint), true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of transactions you executed on a specific market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve transactions for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of transaction for a market</returns>
        public CallResult<CoinExPagedResult<CoinExOrderTransactionExtended>> GetExecutedTransactions(string market, int page, int limit) => GetExecutedTransactionsAsync(market, page, limit).Result;
        /// <summary>
        /// Gets a list of transactions you executed on a specific market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve transactions for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of transaction for a market</returns>
        public async Task<CallResult<CoinExPagedResult<CoinExOrderTransactionExtended>>> GetExecutedTransactionsAsync(string market, int page, int limit)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "page", page },
                { "limit", limit },
            };

            return await ExecutePaged<CoinExOrderTransactionExtended>(GetUrl(UserTransactionsEndpoint), true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="market">The market the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns>Details of the canceled order</returns>
        public CallResult<CoinExOrder> CancelOrder(string market, long orderId) => CancelOrderAsync(market, orderId).Result;
        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="market">The market the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns>Details of the canceled order</returns>
        public async Task<CallResult<CoinExOrder>> CancelOrderAsync(string market, long orderId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "market", market },
                { "id", orderId }
            };

            return await Execute<CoinExOrder>(GetUrl(CancelOrderEndpoint), method: Constants.DeleteMethod, signed: true, parameters: parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <returns>Mining difficulty</returns>
        public CallResult<CoinExMiningDifficulty> GetMiningDifficulty() => GetMiningDifficultyAsync().Result;
        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <returns>Mining difficulty</returns>
        public async Task<CallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync()
        {
            return await Execute<CoinExMiningDifficulty>(GetUrl(MiningDifficultyEndpoint), true).ConfigureAwait(false);
        }
        #endregion

        #region private

        protected override bool IsErrorResponse(JToken data)
        {
            return data["code"] != null && (int)data["code"] != 0;
        }

        protected override Error ParseErrorResponse(JToken error)
        {
            if (error["code"] == null || error["message"] == null)
                return new ServerError(error.ToString());

            return new ServerError((int)error["code"], (string)error["message"]);
        }

        private async Task<CallResult<T>> Execute<T>(Uri uri, bool signed = false, string method = Constants.GetMethod, Dictionary<string, object> parameters = null) where T : class
        {
            return GetResult(await ExecuteRequest<CoinExApiResult<T>>(uri, method, parameters, signed).ConfigureAwait(false));
        }

        private async Task<CallResult<CoinExPagedResult<T>>> ExecutePaged<T>(Uri uri, bool signed = false, string method = Constants.GetMethod, Dictionary<string, object> parameters = null) where T : class
        {
            return GetResult(await ExecuteRequest<CoinExApiResult<CoinExPagedResult<T>>>(uri, method, parameters, signed).ConfigureAwait(false));
        }

        private static CallResult<T> GetResult<T>(CallResult<CoinExApiResult<T>> result) where T : class
        {
            if (result.Error != null || result.Data == null)
                return new CallResult<T>(null, result.Error);

            return new CallResult<T>(result.Data.Data, null);
        }

        private Uri GetUrl(string endpoint)
        {
            return new Uri(BaseAddress + endpoint);
        }

        private void Configure(CoinExClientOptions options)
        {
            userAgent = options.UserAgent;
        }
        #endregion
        #endregion
    }
}
