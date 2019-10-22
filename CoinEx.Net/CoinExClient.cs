using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CryptoExchange.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CoinEx.Net.Interfaces;
using CryptoExchange.Net.Authentication;
using Newtonsoft.Json.Linq;

namespace CoinEx.Net
{
    /// <summary>
    /// Client for the CoinEx REST API
    /// </summary>
    public class CoinExClient : RestClient, ICoinExClient
    {
        #region fields
        private static CoinExClientOptions defaultOptions = new CoinExClientOptions();
        private static CoinExClientOptions DefaultOptions => defaultOptions.Copy<CoinExClientOptions>();
        
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
        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetAuthenticationProvider(new CoinExAuthenticationProvider(new ApiCredentials(apiKey, apiSecret)));
        }

        /// <summary>
        /// Gets a list of symbols active on CoinEx
        /// </summary>
        /// <returns>List of symbol names</returns>
        public WebCallResult<IEnumerable<string>> GetSymbols(CancellationToken ct = default) => GetSymbolsAsync(ct).Result;
        /// <summary>
        /// Gets a list of symbols active on CoinEx
        /// </summary>
        /// <returns>List of symbol names</returns>
        public async Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await Execute<IEnumerable<string>>(GetUrl(MarketListEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the state of a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the symbol</returns>
        public WebCallResult<CoinExSymbolState> GetSymbolState(string symbol, CancellationToken ct = default) => GetSymbolStateAsync(symbol, ct).Result;
        /// <summary>
        /// Gets the state of a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the symbol</returns>
        public async Task<WebCallResult<CoinExSymbolState>> GetSymbolStateAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };

            return await Execute<CoinExSymbolState>(GetUrl(MarketStatisticsEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the states of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all symbols</returns>
        public WebCallResult<CoinExSymbolStatesList> GetSymbolStates(CancellationToken ct = default) => GetSymbolStatesAsync(ct).Result;
        /// <summary>
        /// Gets the states of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all symbols</returns>
        public async Task<WebCallResult<CoinExSymbolStatesList>> GetSymbolStatesAsync(CancellationToken ct = default)
        {
            return await Execute<CoinExSymbolStatesList>(GetUrl(MarketStatisticsListEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order book for a symbol</returns>
        public WebCallResult<CoinExOrderBook> GetOrderBook(string symbol, int mergeDepth, int? limit = null, CancellationToken ct = default) => 
            GetOrderBookAsync(symbol, mergeDepth, limit, ct).Result;
        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order book for a symbol</returns>
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

        /// <summary>
        /// Gets the latest trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="fromId">The id from which on to return trades</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        public WebCallResult<IEnumerable<CoinExSymbolTrade>> GetSymbolTrades(string symbol, long? fromId = null, CancellationToken ct = default) => 
            GetSymbolTradesAsync(symbol, fromId, ct).Result;
        /// <summary>
        /// Gets the latest trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="fromId">The id from which on to return trades</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        public async Task<WebCallResult<IEnumerable<CoinExSymbolTrade>>> GetSymbolTradesAsync(string symbol, long? fromId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };
            parameters.AddOptionalParameter("last_id", fromId);

            return await Execute<IEnumerable<CoinExSymbolTrade>>(GetUrl(MarketDealsEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves kline data for a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a symbol</returns>
        public WebCallResult<IEnumerable<CoinExKline>> GetKlines(string symbol, KlineInterval interval, int? limit = null, CancellationToken ct = default) => GetKlinesAsync(symbol, interval, limit, ct).Result;
        /// <summary>
        /// Retrieves kline data for a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a symbol</returns>
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

        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of balances</returns>
        public WebCallResult<Dictionary<string, CoinExBalance>> GetBalances(CancellationToken ct = default) => GetBalancesAsync(ct).Result;
        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of balances</returns>
        public async Task<WebCallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            return await Execute<Dictionary<string, CoinExBalance>>(GetUrl(AccountInfoEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<IEnumerable<CoinExWithdrawal>> GetWithdrawalHistory(string? coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null, CancellationToken ct = default) =>
            GetWithdrawalHistoryAsync(coin, coinWithdrawId, page, limit, ct).Result;
        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin", coin);
            parameters.AddOptionalParameter("coin_withdraw_id", coinWithdrawId);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<IEnumerable<CoinExWithdrawal>>(GetUrl(WithdrawalHistoryEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The withdrawal object</returns>
        public WebCallResult<CoinExWithdrawal> Withdraw(string coin, string coinAddress, decimal amount, CancellationToken ct = default) => WithdrawAsync(coin, coinAddress, amount, ct).Result;
        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The withdrawal object</returns>
        public async Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string coin, string coinAddress, decimal amount, CancellationToken ct = default)
        {
            coin.ValidateNotNull(nameof(coin));
            coinAddress.ValidateNotNull(nameof(coinAddress));
            var parameters = new Dictionary<string, object>
            {
                { "coin_type", coin },
                { "coin_address", coinAddress },
                { "actual_amount", amount.ToString(CultureInfo.InvariantCulture) }
            };

            return await Execute<CoinExWithdrawal>(GetUrl(WithdrawEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if successful, false otherwise</returns>
        public WebCallResult<bool> CancelWithdrawal(long coinWithdrawId, CancellationToken ct = default) => CancelWithdrawalAsync(coinWithdrawId, ct).Result;
        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<WebCallResult<bool>> CancelWithdrawalAsync(long coinWithdrawId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "coin_withdraw_id", coinWithdrawId }
            };

            var result = await Execute<object>(GetUrl(CancelWithdrawalEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
            return !result ? new WebCallResult<bool>(result.ResponseStatusCode, result.ResponseHeaders, false, result.Error) : new WebCallResult<bool>(result.ResponseStatusCode, result.ResponseHeaders, true, null);
        }

        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        public WebCallResult<CoinExOrder> PlaceLimitOrder(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default) =>
            PlaceLimitOrderAsync(symbol, type, amount, price, sourceId, ct).Result;
        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        public async Task<WebCallResult<CoinExOrder>> PlaceLimitOrderAsync(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            return await Execute<CoinExOrder>(GetUrl(PlaceLimitOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        public WebCallResult<CoinExOrder> PlaceMarketOrder(string symbol, TransactionType type, decimal amount, string? sourceId = null, CancellationToken ct = default) => 
            PlaceMarketOrderAsync(symbol, type, amount, sourceId, ct).Result;
        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order, specified in the base asset. For example on a ETHBTC symbol the value should be how much BTC should be spend to buy ETH</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        public async Task<WebCallResult<CoinExOrder>> PlaceMarketOrderAsync(string symbol, TransactionType type, decimal amount, string? sourceId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            return await Execute<CoinExOrder>(GetUrl(PlaceMarketOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Places an order which should be filled immediately up on placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public WebCallResult<CoinExOrder> PlaceImmediateOrCancelOrder(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default) => 
            PlaceImmediateOrCancelOrderAsync(symbol, type, amount, price, sourceId, ct).Result;
        /// <summary>
        /// Places an order which should be filled immediately up on placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<CoinExOrder>> PlaceImmediateOrCancelOrderAsync(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            return await Execute<CoinExOrder>(GetUrl(PlaceImmediateOrCancelOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of open orders for a symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        public WebCallResult<CoinExPagedResult<CoinExOrder>> GetOpenOrders(string symbol, int page, int limit, CancellationToken ct = default) => 
            GetOpenOrdersAsync(symbol, page, limit, ct).Result;
        /// <summary>
        /// Retrieves a list of open orders for a symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
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

        /// <summary>
        /// Retrieves a list of executed orders for a symbol in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a symbol</returns>
        public WebCallResult<CoinExPagedResult<CoinExOrder>> GetExecutedOrders(string symbol, int page, int limit, CancellationToken ct = default) =>
            GetExecutedOrdersAsync(symbol, page, limit, ct).Result;
        /// <summary>
        /// Retrieves a list of executed orders for a symbol in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a symbol</returns>
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

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        public WebCallResult<CoinExOrder> GetOrderStatus(long orderId, string symbol, CancellationToken ct = default) => GetOrderStatusAsync(orderId, symbol, ct).Result;
        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        public async Task<WebCallResult<CoinExOrder>> GetOrderStatusAsync(long orderId, string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "id", orderId }
            };

            return await Execute<CoinExOrder>(GetUrl(OrderStatusEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        public WebCallResult<CoinExPagedResult<CoinExOrderTransaction>> GetExecutedOrderDetails(long orderId, int page, int limit, CancellationToken ct = default) => 
            GetExecutedOrderDetailsAsync(orderId, page, limit, ct).Result;
        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrderTransaction>>> GetExecutedOrderDetailsAsync(long orderId, int page, int limit, CancellationToken ct = default)
        {
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "id", orderId },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrderTransaction>(GetUrl(OrderDetailsEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of trades you executed on a specific symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        public WebCallResult<CoinExPagedResult<CoinExOrderTransactionExtended>> GetTrades(string symbol, int page, int limit, CancellationToken ct = default) => 
            GetTradesAsync(symbol, page, limit, ct).Result;
        /// <summary>
        /// Gets a list of trades you executed on a specific symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrderTransactionExtended>>> GetTradesAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrderTransactionExtended>(GetUrl(UserTransactionsEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the canceled order</returns>
        public WebCallResult<CoinExOrder> CancelOrder(string symbol, long orderId, CancellationToken ct = default) => CancelOrderAsync(symbol, orderId, ct).Result;
        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the canceled order</returns>
        public async Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "id", orderId }
            };

            return await Execute<CoinExOrder>(GetUrl(CancelOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Mining difficulty</returns>
        public WebCallResult<CoinExMiningDifficulty> GetMiningDifficulty(CancellationToken ct = default) => GetMiningDifficultyAsync(ct).Result;
        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Mining difficulty</returns>
        public async Task<WebCallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync(CancellationToken ct = default)
        {
            return await Execute<CoinExMiningDifficulty>(GetUrl(MiningDifficultyEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }
        #endregion

        #region private
        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (error["code"] == null || error["message"] == null)
                return new ServerError(error.ToString());

            return new ServerError((int)error["code"], (string)error["message"]);
        }

        private async Task<WebCallResult<T>> Execute<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequest<CoinExApiResult<T>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        private async Task<WebCallResult<CoinExPagedResult<T>>> ExecutePaged<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequest<CoinExApiResult<CoinExPagedResult<T>>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        private static WebCallResult<T> GetResult<T>(WebCallResult<CoinExApiResult<T>> result) where T : class
        {
            if (result.Error != null || result.Data == null)
                return WebCallResult<T>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error ?? new UnknownError("No data received"));

            return new WebCallResult<T>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        private Uri GetUrl(string endpoint)
        {
            return new Uri(BaseAddress + endpoint);
        }
        #endregion
        #endregion
    }
}
