using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace CoinEx.Net.Interfaces
{
    /// <summary>
    /// Interface for the CoinEx socket client
    /// </summary>
    public interface ICoinExSocketClient: ISocketClient
    {
        /// <summary>
        /// Pings the server
        /// </summary>
        /// <returns>True if server responded, false otherwise</returns>
        CallResult<bool> Ping();

        /// <summary>
        /// Pings the server
        /// </summary>
        /// <returns>True if server responded, false otherwise</returns>
        Task<CallResult<bool>> PingAsync();

        /// <summary>
        /// Gets the server time
        /// </summary>
        /// <returns>The server time</returns>
        CallResult<DateTime> GetServerTime();

        /// <summary>
        /// Gets the server time
        /// </summary>
        /// <returns>The server time</returns>
        Task<CallResult<DateTime>> GetServerTimeAsync();

        /// <summary>
        /// Get the market state
        /// </summary>
        /// <param name="symbol">The market to get the state for</param>
        /// <param name="cyclePeriod">The period to get data over, specified in seconds. i.e. one minute = 60, one day = 86400</param>
        /// <returns>Market state</returns>
        CallResult<CoinExSocketMarketState> GetMarketState(string symbol, int cyclePeriod);

        /// <summary>
        /// Get the market state
        /// </summary>
        /// <param name="symbol">The market to get the state for</param>
        /// <param name="cyclePeriod">The period to get data over, specified in seconds. i.e. one minute = 60, one day = 86400</param>
        /// <returns>Market state</returns>
        Task<CallResult<CoinExSocketMarketState>> GetMarketStateAsync(string symbol, int cyclePeriod);

        /// <summary>
        /// Get a market depth overview
        /// </summary>
        /// <param name="symbol">The market to get depth for</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <returns>Depth overview for a market</returns>
        CallResult<CoinExSocketMarketDepth> GetMarketDepth(string symbol, int limit, int mergeDepth);

        /// <summary>
        /// Get a market depth overview
        /// </summary>
        /// <param name="symbol">The market to get depth for</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <returns>Depth overview for a market</returns>
        Task<CallResult<CoinExSocketMarketDepth>> GetMarketDepthAsync(string symbol, int limit, int mergeDepth);

        /// <summary>
        /// Gets the latest transactions on a market
        /// </summary>
        /// <param name="symbol">The market to get the transactions for</param>
        /// <param name="limit">The limit of transactions</param>
        /// <param name="fromId">Return transactions since this id</param>
        /// <returns>List of transactions</returns>
        CallResult<IEnumerable<CoinExSocketMarketTransaction>> GetMarketTransactions(string symbol, int limit, int? fromId = null);

        /// <summary>
        /// Gets the latest transactions on a market
        /// </summary>
        /// <param name="symbol">The market to get the transactions for</param>
        /// <param name="limit">The limit of transactions</param>
        /// <param name="fromId">Return transactions since this id</param>
        /// <returns>List of transactions</returns>
        Task<CallResult<IEnumerable<CoinExSocketMarketTransaction>>> GetMarketTransactionsAsync(string symbol, int limit, int? fromId = null);

        /// <summary>
        /// Gets market kline data
        /// </summary>
        /// <param name="symbol">The market to get the data for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <returns></returns>
        CallResult<CoinExKline> GetMarketKlines(string symbol, KlineInterval interval);

        /// <summary>
        /// Gets market kline data
        /// </summary>
        /// <param name="symbol">The market to get the data for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <returns></returns>
        Task<CallResult<CoinExKline>> GetMarketKlinesAsync(string symbol, KlineInterval interval);

        /// <summary>
        /// Get balances of coins. Requires API credentials
        /// </summary>
        /// <param name="coins">The coins to get the balances for, empty for all</param>
        /// <returns>Dictionary of coins and their balances</returns>
        CallResult<Dictionary<string, CoinExBalance>> GetBalances(params string[] coins);

        /// <summary>
        /// Get balances of coins. Requires API credentials
        /// </summary>
        /// <param name="coins">The coins to get the balances for, empty for all</param>
        /// <returns>Dictionary of coins and their balances</returns>
        Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(params string[] coins);

        /// <summary>
        /// Gets a list of open orders for a market
        /// </summary>
        /// <param name="symbol">Market to get open orders for</param>
        /// <param name="type">The type of orders to get</param>
        /// <param name="offset">The offset in the list</param>
        /// <param name="limit">The limit of results</param>
        /// <returns>List of open orders</returns>
        CallResult<CoinExSocketPagedResult<CoinExSocketOrder>> GetOpenOrders(string symbol, TransactionType type, int offset, int limit);

        /// <summary>
        /// Gets a list of open orders for a market
        /// </summary>
        /// <param name="symbol">Market to get open orders for</param>
        /// <param name="type">The type of orders to get</param>
        /// <param name="offset">The offset in the list</param>
        /// <param name="limit">The limit of results</param>
        /// <returns>List of open orders</returns>
        Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOpenOrdersAsync(string symbol, TransactionType type, int offset, int limit);

        /// <summary>
        /// Subscribe to market state updates for a specific market
        /// </summary>
        /// <param name="symbol">Market to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketState]: the market state update</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToMarketStateUpdates(string symbol, Action<string, CoinExSocketMarketState> onMessage);

        /// <summary>
        /// Subscribe to market state updates for a specific market
        /// </summary>
        /// <param name="symbol">Market to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketState]: the market state update</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToMarketStateUpdatesAsync(string symbol, Action<string, CoinExSocketMarketState> onMessage);

        /// <summary>
        /// Subscribe to market state updates for all markets
        /// </summary>
        /// <param name="onMessage">Data handler, receives a dictionary of market name -> market state</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToMarketStateUpdates(Action<Dictionary<string, CoinExSocketMarketState>> onMessage);

        /// <summary>
        /// Subscribe to market state updates for all markets
        /// </summary>
        /// <param name="onMessage">Data handler, receives a dictionary of market name -> market state</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToMarketStateUpdatesAsync(Action<Dictionary<string, CoinExSocketMarketState>> onMessage);

        /// <summary>
        /// Subscribe to market depth updates for a market
        /// </summary>
        /// <param name="symbol">The market to receive updates for</param>
        /// <param name="limit">The limit of results to receive in a update</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[bool]: whether this is a full update, or an update based on the last send data, Param 3[CoinExSocketMarketDepth]: the update data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToMarketDepthUpdates(string symbol, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage);

        /// <summary>
        /// Subscribe to market depth updates for a market
        /// </summary>
        /// <param name="symbol">The market to receive updates for</param>
        /// <param name="limit">The limit of results to receive in a update</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[bool]: whether this is a full update, or an update based on the last send data, Param 3[CoinExSocketMarketDepth]: the update data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToMarketDepthUpdatesAsync(string symbol, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage);

        /// <summary>
        /// Subscribe to market transaction updates for a market
        /// </summary>
        /// <param name="symbol">The market to receive updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketTransaction[]]: list of transactions</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToMarketTransactionUpdates(string symbol, Action<string, IEnumerable<CoinExSocketMarketTransaction>> onMessage);

        /// <summary>
        /// Subscribe to market transaction updates for a market
        /// </summary>
        /// <param name="symbol">The market to receive updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketTransaction[]]: list of transactions</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToMarketTransactionUpdatesAsync(string symbol, Action<string, IEnumerable<CoinExSocketMarketTransaction>> onMessage);

        /// <summary>
        /// Subscribe to kline updates for a market
        /// </summary>
        /// <param name="symbol">The market to receive updates for</param>
        /// <param name="interval">The interval of the candle to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExKline[]]: list of klines updated klines</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToMarketKlineUpdates(string symbol, KlineInterval interval, Action<string, IEnumerable<CoinExKline>> onMessage);

        /// <summary>
        /// Subscribe to kline updates for a market
        /// </summary>
        /// <param name="symbol">The market to receive updates for</param>
        /// <param name="interval">The interval of the candle to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExKline[]]: list of klines updated klines</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToMarketKlineUpdatesAsync(string symbol, KlineInterval interval, Action<string, IEnumerable<CoinExKline>> onMessage);

        /// <summary>
        /// Subscribe to updates of your balances, Receives updates whenever the balance for a coin changes
        /// </summary>
        /// <param name="onMessage">Data handler, receives a dictionary of coin name -> balance</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToBalanceUpdates(Action<Dictionary<string, CoinExBalance>> onMessage);

        /// <summary>
        /// Subscribe to updates of your balances, Receives updates whenever the balance for a coin changes
        /// </summary>
        /// <param name="onMessage">Data handler, receives a dictionary of coin name -> balance</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<Dictionary<string, CoinExBalance>> onMessage);

        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// </summary>
        /// <param name="markets">The markets to receive order updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[UpdateType]: the type of update, Param 2[CoinExSocketOrder]: the order that was updated</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        CallResult<UpdateSubscription> SubscribeToOrderUpdates(IEnumerable<string> markets, Action<UpdateType, CoinExSocketOrder> onMessage);

        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// </summary>
        /// <param name="markets">The markets to receive order updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[UpdateType]: the type of update, Param 2[CoinExSocketOrder]: the order that was updated</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(IEnumerable<string> markets, Action<UpdateType, CoinExSocketOrder> onMessage);
    }
}