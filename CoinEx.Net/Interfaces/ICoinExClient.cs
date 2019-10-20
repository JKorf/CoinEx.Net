using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net.Interfaces
{
    /// <summary>
    /// Interface for the CoinEx client
    /// </summary>
    public interface ICoinExClient: IRestClient
    {
        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);

        /// <summary>
        /// Gets a list of markets active on CoinEx
        /// </summary>
        /// <returns>List of market names</returns>
        WebCallResult<IEnumerable<string>> GetMarketList(CancellationToken ct = default);

        /// <summary>
        /// Gets a list of markets active on CoinEx
        /// </summary>
        /// <returns>List of market names</returns>
        Task<WebCallResult<IEnumerable<string>>> GetMarketListAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the state of a specific market
        /// </summary>
        /// <param name="market">The market to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the market</returns>
        WebCallResult<CoinExMarketState> GetMarketState(string market, CancellationToken ct = default);

        /// <summary>
        /// Gets the state of a specific market
        /// </summary>
        /// <param name="market">The market to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the market</returns>
        Task<WebCallResult<CoinExMarketState>> GetMarketStateAsync(string market, CancellationToken ct = default);

        /// <summary>
        /// Gets the states of all markets
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all markets</returns>
        WebCallResult<CoinExMarketStatesList> GetMarketStates(CancellationToken ct = default);

        /// <summary>
        /// Gets the states of all markets
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all markets</returns>
        Task<WebCallResult<CoinExMarketStatesList>> GetMarketStatesAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the depth data for a market
        /// </summary>
        /// <param name="market">The market to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Depth data for a market</returns>
        WebCallResult<CoinExMarketDepth> GetMarketDepth(string market, int mergeDepth, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the depth data for a market
        /// </summary>
        /// <param name="market">The market to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Depth data for a market</returns>
        Task<WebCallResult<CoinExMarketDepth>> GetMarketDepthAsync(string market, int mergeDepth, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the latest transactions for a market
        /// </summary>
        /// <param name="market">The market to retrieve data for</param>
        /// <param name="fromId">The id from which on to return transactions</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of transactions for a market</returns>
        WebCallResult<IEnumerable<CoinExMarketTransaction>> GetLatestTransactions(string market, long? fromId = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the latest transactions for a market
        /// </summary>
        /// <param name="market">The market to retrieve data for</param>
        /// <param name="fromId">The id from which on to return transactions</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of transactions for a market</returns>
        Task<WebCallResult<IEnumerable<CoinExMarketTransaction>>> GetLatestTransactionsAsync(string market, long? fromId = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves kline data for a specific market
        /// </summary>
        /// <param name="market">The market to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a market</returns>
        WebCallResult<IEnumerable<CoinExKline>> GetKlines(string market, KlineInterval interval, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves kline data for a specific market
        /// </summary>
        /// <param name="market">The market to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a market</returns>
        Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string market, KlineInterval interval, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of balances</returns>
        WebCallResult<Dictionary<string, CoinExBalance>> GetBalances(CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of balances</returns>
        Task<WebCallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        WebCallResult<IEnumerable<CoinExWithdrawal>> GetWithdrawalHistory(string? coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The withdrawal object</returns>
        WebCallResult<CoinExWithdrawal> Withdraw(string coin, string coinAddress, decimal amount, CancellationToken ct = default);

        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The withdrawal object</returns>
        Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string coin, string coinAddress, decimal amount, CancellationToken ct = default);

        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if successful, false otherwise</returns>
        WebCallResult<bool> CancelWithdrawal(long coinWithdrawId, CancellationToken ct = default);

        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<WebCallResult<bool>> CancelWithdrawalAsync(long coinWithdrawId, CancellationToken ct = default);

        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        WebCallResult<CoinExOrder> PlaceLimitOrder(string market, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        Task<WebCallResult<CoinExOrder>> PlaceLimitOrderAsync(string market, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        WebCallResult<CoinExOrder> PlaceMarketOrder(string market, TransactionType type, decimal amount, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order, specified in the base asset. For example on a ETHBTC market the value should be how much BTC should be spend to buy ETH</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        Task<WebCallResult<CoinExOrder>> PlaceMarketOrderAsync(string market, TransactionType type, decimal amount, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Places an order which should be filled immediately up on placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        WebCallResult<CoinExOrder> PlaceImmediateOrCancelOrder(string market, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Places an order which should be filled immediately up on placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="market">The market to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder>> PlaceImmediateOrCancelOrderAsync(string market, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of open orders for a market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a market</returns>
        WebCallResult<CoinExPagedResult<CoinExOrder>> GetOpenOrders(string market, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of open orders for a market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a market</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string market, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of executed orders for a market in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a market</returns>
        WebCallResult<CoinExPagedResult<CoinExOrder>> GetExecutedOrders(string market, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of executed orders for a market in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a market</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetExecutedOrdersAsync(string market, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="market">The market the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        WebCallResult<CoinExOrder> GetOrderStatus(long orderId, string market, CancellationToken ct = default);

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="market">The market the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        Task<WebCallResult<CoinExOrder>> GetOrderStatusAsync(long orderId, string market, CancellationToken ct = default);

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        WebCallResult<CoinExPagedResult<CoinExOrderTransaction>> GetExecutedOrderDetails(long orderId, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrderTransaction>>> GetExecutedOrderDetailsAsync(long orderId, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of transactions you executed on a specific market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve transactions for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of transaction for a market</returns>
        WebCallResult<CoinExPagedResult<CoinExOrderTransactionExtended>> GetExecutedTransactions(string market, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of transactions you executed on a specific market. Requires API credentials
        /// </summary>
        /// <param name="market">The market to retrieve transactions for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of transaction for a market</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrderTransactionExtended>>> GetExecutedTransactionsAsync(string market, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="market">The market the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the canceled order</returns>
        WebCallResult<CoinExOrder> CancelOrder(string market, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="market">The market the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the canceled order</returns>
        Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string market, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Mining difficulty</returns>
        WebCallResult<CoinExMiningDifficulty> GetMiningDifficulty(CancellationToken ct = default);

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Mining difficulty</returns>
        Task<WebCallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync(CancellationToken ct = default);
    }
}