﻿using System.Collections.Generic;
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
        /// Gets a list of symbols active on CoinEx
        /// </summary>
        /// <returns>List of symbol names</returns>
        WebCallResult<IEnumerable<string>> GetSymbols(CancellationToken ct = default);

        /// <summary>
        /// Gets a list of symbols active on CoinEx
        /// </summary>
        /// <returns>List of symbol names</returns>
        Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the state of a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the symbol</returns>
        WebCallResult<CoinExSymbolState> GetSymbolState(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Gets the state of a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the symbol</returns>
        Task<WebCallResult<CoinExSymbolState>> GetSymbolStateAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Gets the states of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all symbols</returns>
        WebCallResult<CoinExSymbolStatesList> GetSymbolStates(CancellationToken ct = default);

        /// <summary>
        /// Gets the states of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all symbols</returns>
        Task<WebCallResult<CoinExSymbolStatesList>> GetSymbolStatesAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order book for a symbol</returns>
        WebCallResult<CoinExOrderBook> GetOrderBook(string symbol, int mergeDepth, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order book for a symbol</returns>
        Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int mergeDepth, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the latest trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="fromId">The id from which on to return trades</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        WebCallResult<IEnumerable<CoinExSymbolTrade>> GetSymbolTrades(string symbol, long? fromId = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the latest trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="fromId">The id from which on to return trades</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        Task<WebCallResult<IEnumerable<CoinExSymbolTrade>>> GetSymbolTradesAsync(string symbol, long? fromId = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves kline data for a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a symbol</returns>
        WebCallResult<IEnumerable<CoinExKline>> GetKlines(string symbol, KlineInterval interval, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves kline data for a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a symbol</returns>
        Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, CancellationToken ct = default);


        /// <summary>
        /// Retrieves market data for the exchange
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        WebCallResult<Dictionary<string, CoinExMarket>>GetMarketInfo(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Retrieves market data for the exchange
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        Task<WebCallResult<Dictionary<string, CoinExMarket>>> GetMarketInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Retrieves market data for the exchange
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        WebCallResult<Dictionary<string, CoinExMarket>> GetMarketInfo(CancellationToken ct = default);

        /// <summary>
        /// Retrieves market data for the exchange
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        Task<WebCallResult<Dictionary<string, CoinExMarket>>> GetMarketInfoAsync(CancellationToken ct = default);
       
        
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
        /// Retrieves a list of deposits. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        WebCallResult<IEnumerable<CoinExDeposit>> GetDepositHistory(string? coin = null, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of deposits. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExDeposit>>> GetDepositHistoryAsync(string? coin = null, int? page = null, int? limit = null, CancellationToken ct = default);


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
        /// <param name="localTransfer">Is it a local transfer between users or onchain</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The withdrawal object</returns>
        WebCallResult<CoinExWithdrawal> Withdraw(string coin, string coinAddress, bool localTransfer, decimal amount, CancellationToken ct = default);

        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="localTransfer">Is it a local transfer between users or onchain</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The withdrawal object</returns>
        Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string coin, string coinAddress, bool localTransfer, decimal amount, CancellationToken ct = default);

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
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        WebCallResult<CoinExOrder> PlaceLimitOrder(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default);

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
        Task<WebCallResult<CoinExOrder>> PlaceLimitOrderAsync(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        WebCallResult<CoinExOrder> PlaceMarketOrder(string symbol, TransactionType type, decimal amount, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order, specified in the base asset. For example on a ETHBTC symbol the value should be how much BTC should be spend to buy ETH</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        Task<WebCallResult<CoinExOrder>> PlaceMarketOrderAsync(string symbol, TransactionType type, decimal amount, string? sourceId = null, CancellationToken ct = default);

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
        WebCallResult<CoinExOrder> PlaceImmediateOrCancelOrder(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default);

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
        Task<WebCallResult<CoinExOrder>> PlaceImmediateOrCancelOrderAsync(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of open orders for a symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        WebCallResult<CoinExPagedResult<CoinExOrder>> GetOpenOrders(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of open orders for a symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of executed orders for a symbol in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a symbol</returns>
        WebCallResult<CoinExPagedResult<CoinExOrder>> GetExecutedOrders(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of executed orders for a symbol in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetExecutedOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        WebCallResult<CoinExOrder> GetOrderStatus(long orderId, string symbol, CancellationToken ct = default);

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        Task<WebCallResult<CoinExOrder>> GetOrderStatusAsync(long orderId, string symbol, CancellationToken ct = default);

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        WebCallResult<CoinExPagedResult<CoinExOrderTrade>> GetExecutedOrderDetails(long orderId, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrderTrade>>> GetExecutedOrderDetailsAsync(long orderId, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of trades you executed on a specific symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        WebCallResult<CoinExPagedResult<CoinExOrderTradeExtended>> GetTrades(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of trades you executed on a specific symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrderTradeExtended>>> GetTradesAsync(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the canceled order</returns>
        WebCallResult<CoinExOrder> CancelOrder(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the canceled order</returns>
        Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default);

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
