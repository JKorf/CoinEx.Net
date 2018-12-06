using System.Collections.Generic;
using System.Threading.Tasks;
using CoinEx.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiter;

namespace CoinEx.Net.Interfaces
{
    public interface ICoinExClient
    {
        /// <summary>
        /// Gets a list of markets active on CoinEx
        /// </summary>
        /// <returns>List of market names</returns>
        CallResult<string[]> GetMarketList();

        /// <summary>
        /// Gets a list of markets active on CoinEx
        /// </summary>
        /// <returns>List of market names</returns>
        Task<CallResult<string[]>> GetMarketListAsync();

        /// <summary>
        /// Gets the state of a specific market
        /// </summary>
        /// <param name="market">The market to retrieve state for</param>
        /// <returns>The state of the market</returns>
        CallResult<CoinExMarketState> GetMarketState(string market);

        /// <summary>
        /// Gets the state of a specific market
        /// </summary>
        /// <param name="market">The market to retrieve state for</param>
        /// <returns>The state of the market</returns>
        Task<CallResult<CoinExMarketState>> GetMarketStateAsync(string market);

        /// <summary>
        /// Gets the states of all markets
        /// </summary>
        /// <returns>List of states for all markets</returns>
        CallResult<CoinExMarketStatesList> GetMarketStates();

        /// <summary>
        /// Gets the states of all markets
        /// </summary>
        /// <returns>List of states for all markets</returns>
        Task<CallResult<CoinExMarketStatesList>> GetMarketStatesAsync();

        /// <summary>
        /// Gets the depth data for a market
        /// </summary>
        /// <param name="market">The market to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <returns>Depth data for a market</returns>
        CallResult<CoinExMarketDepth> GetMarketDepth(string market, int mergeDepth, int? limit = null);

        /// <summary>
        /// Gets the depth data for a market
        /// </summary>
        /// <param name="market">The market to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <returns>Depth data for a market</returns>
        Task<CallResult<CoinExMarketDepth>> GetMarketDepthAsync(string market, int mergeDepth, int? limit = null);

        /// <summary>
        /// Gets the latest transactions for a market
        /// </summary>
        /// <param name="market">The market to retrieve data for</param>
        /// <param name="fromId">The id from which on to return transactions</param>
        /// <returns>List of transactions for a market</returns>
        CallResult<CoinExMarketTransaction[]> GetLatestTransactions(string market, long? fromId = null);

        /// <summary>
        /// Gets the latest transactions for a market
        /// </summary>
        /// <param name="market">The market to retrieve data for</param>
        /// <param name="fromId">The id from which on to return transactions</param>
        /// <returns>List of transactions for a market</returns>
        Task<CallResult<CoinExMarketTransaction[]>> GetLatestTransactionsAsync(string market, long? fromId = null);

        /// <summary>
        /// Retrieves kline data for a specific market
        /// </summary>
        /// <param name="market">The market to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <returns>List of klines for a market</returns>
        CallResult<CoinExKline[]> GetKlines(string market, KlineInterval interval, int? limit = null);

        /// <summary>
        /// Retrieves kline data for a specific market
        /// </summary>
        /// <param name="market">The market to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <returns>List of klines for a market</returns>
        Task<CallResult<CoinExKline[]>> GetKlinesAsync(string market, KlineInterval interval, int? limit = null);

        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <returns>List of balances</returns>
        CallResult<Dictionary<string, CoinExBalance>> GetBalances();

        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <returns>List of balances</returns>
        Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync();

        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <returns></returns>
        CallResult<CoinExWithdrawal[]> GetWithdrawalHistory(string coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null);

        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <returns></returns>
        Task<CallResult<CoinExWithdrawal[]>> GetWithdrawalHistoryAsync(string coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null);

        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <returns>The withdrawal object</returns>
        CallResult<CoinExWithdrawal> Withdraw(string coin, string coinAddress, decimal amount);

        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <returns>The withdrawal object</returns>
        Task<CallResult<CoinExWithdrawal>> WithdrawAsync(string coin, string coinAddress, decimal amount);

        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <returns>True if successful, false otherwise</returns>
        CallResult<bool> CancelWithdrawal(long coinWithdrawId);

        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<CallResult<bool>> CancelWithdrawalAsync(long coinWithdrawId);

        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns>Details of the order that was placed</returns>
        CallResult<CoinExOrder> PlaceLimitOrder(string market, TransactionType type, decimal amount, decimal price, string sourceId = null);

        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns>Details of the order that was placed</returns>
        Task<CallResult<CoinExOrder>> PlaceLimitOrderAsync(string market, TransactionType type, decimal amount, decimal price, string sourceId = null);

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns>Details of the order that was placed</returns>
        CallResult<CoinExOrder> PlaceMarketOrder(string market, TransactionType type, decimal amount, string sourceId = null);

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns>Details of the order that was placed</returns>
        Task<CallResult<CoinExOrder>> PlaceMarketOrderAsync(string market, TransactionType type, decimal amount, string sourceId = null);

        /// <summary>
        /// Places an order which should be filled immediately up on placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns></returns>
        CallResult<CoinExOrder> PlaceImmediateOrCancelOrder(string market, TransactionType type, decimal amount, decimal price, string sourceId = null);

        /// <summary>
        /// Places an order which should be filled immediately up on placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <returns></returns>
        Task<CallResult<CoinExOrder>> PlaceImmediateOrCancelOrderAsync(string market, TransactionType type, decimal amount, decimal price, string sourceId = null);

        /// <summary>
        /// Retrieves a list of open orders for a market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of open orders for a market</returns>
        CallResult<CoinExPagedResult<CoinExOrder>> GetOpenOrders(string market, int page, int limit);

        /// <summary>
        /// Retrieves a list of open orders for a market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of open orders for a market</returns>
        Task<CallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string market, int page, int limit);

        /// <summary>
        /// Retrieves a list of executed orders for a market in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of executed orders for a market</returns>
        CallResult<CoinExPagedResult<CoinExOrder>> GetExecutedOrders(string market, int page, int limit);

        /// <summary>
        /// Retrieves a list of executed orders for a market in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of executed orders for a market</returns>
        Task<CallResult<CoinExPagedResult<CoinExOrder>>> GetExecutedOrdersAsync(string market, int page, int limit);

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="market">The market the order is for</param>
        /// <returns>Details of the order</returns>
        CallResult<CoinExOrder> GetOrderStatus(long orderId, string market);

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="market">The market the order is for</param>
        /// <returns>Details of the order</returns>
        Task<CallResult<CoinExOrder>> GetOrderStatusAsync(long orderId, string market);

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>Details of an executed order</returns>
        CallResult<CoinExPagedResult<CoinExOrderTransaction>> GetExecutedOrderDetails(long orderId, int page, int limit);

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>Details of an executed order</returns>
        Task<CallResult<CoinExPagedResult<CoinExOrderTransaction>>> GetExecutedOrderDetailsAsync(long orderId, int page, int limit);

        /// <summary>
        /// Gets a list of transactions you executed on a specific market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve transactions for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of transaction for a market</returns>
        CallResult<CoinExPagedResult<CoinExOrderTransactionExtended>> GetExecutedTransactions(string market, int page, int limit);

        /// <summary>
        /// Gets a list of transactions you executed on a specific market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve transactions for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <returns>List of transaction for a market</returns>
        Task<CallResult<CoinExPagedResult<CoinExOrderTransactionExtended>>> GetExecutedTransactionsAsync(string market, int page, int limit);

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="market">The market the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns>Details of the canceled order</returns>
        CallResult<CoinExOrder> CancelOrder(string market, long orderId);

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="market">The market the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <returns>Details of the canceled order</returns>
        Task<CallResult<CoinExOrder>> CancelOrderAsync(string market, long orderId);

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <returns>Mining difficulty</returns>
        CallResult<CoinExMiningDifficulty> GetMiningDifficulty();

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <returns>Mining difficulty</returns>
        Task<CallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync();

        /// <summary>
        /// The factory for creating requests. Used for unit testing
        /// </summary>
        IRequestFactory RequestFactory { get; set; }

        /// <summary>
        /// Adds a rate limiter to the client. There are 2 choices, the <see cref="RateLimiterTotal"/> and the <see cref="RateLimiterPerEndpoint"/>.
        /// </summary>
        /// <param name="limiter">The limiter to add</param>
        void AddRateLimiter(IRateLimiter limiter);

        /// <summary>
        /// Removes all rate limiters from this client
        /// </summary>
        void RemoveRateLimiters();

        /// <summary>
        /// Ping to see if the server is reachable
        /// </summary>
        /// <returns>The roundtrip time of the ping request</returns>
        CallResult<long> Ping();

        /// <summary>
        /// Ping to see if the server is reachable
        /// </summary>
        /// <returns>The roundtrip time of the ping request</returns>
        Task<CallResult<long>> PingAsync();

        void Dispose();
    }
}