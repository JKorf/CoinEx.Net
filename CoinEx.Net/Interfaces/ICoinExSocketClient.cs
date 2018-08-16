using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using CryptoExchange.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.RateLimiter;

namespace CoinEx.Net.Interfaces
{
    public interface ICoinExSocketClient
    {
        IWebsocketFactory SocketFactory { get; set; }
        IRequestFactory RequestFactory { get; set; }

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.PingAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<long> Ping();

        /// <summary>
        /// Pings the server
        /// </summary>
        /// <returns>True if server responded, false otherwise</returns>
        Task<CallResult<long>> PingAsync();

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.GetServerTimeAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<DateTime> GetServerTime();

        /// <summary>
        /// Gets the server time
        /// </summary>
        /// <returns>The server time</returns>
        Task<CallResult<DateTime>> GetServerTimeAsync();

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.GetMarketStateAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExSocketMarketState> GetMarketState(string market, int cyclePeriod);

        /// <summary>
        /// Get the market state
        /// </summary>
        /// <param name="market">The market to get the state for</param>
        /// <param name="cyclePeriod">The period to get data over, specified in seconds. i.e. one minute = 60, one day = 86400</param>
        /// <returns>Market state</returns>
        Task<CallResult<CoinExSocketMarketState>> GetMarketStateAsync(string market, int cyclePeriod);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.GetMarketDepthAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExSocketMarketDepth> GetMarketDepth(string market, int limit, int mergeDepth);

        /// <summary>
        /// Get a market depth overview
        /// </summary>
        /// <param name="market">The market to get depth for</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <returns>Depth overview for a market</returns>
        Task<CallResult<CoinExSocketMarketDepth>> GetMarketDepthAsync(string market, int limit, int mergeDepth);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.GetMarketTransactionsAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExSocketMarketTransaction[]> GetMarketTransactions(string market, int limit, int? lastId = null);

        /// <summary>
        /// Gets the latest transactions on a market
        /// </summary>
        /// <param name="market">The market to get the transactions for</param>
        /// <param name="limit">The limit of transactions</param>
        /// <param name="lastId">Return transactions since this id</param>
        /// <returns>List of transactions</returns>
        Task<CallResult<CoinExSocketMarketTransaction[]>> GetMarketTransactionsAsync(string market, int limit, int? lastId = null);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.GetMarketKlinesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExKline> GetMarketKlines(string market, KlineInterval interval);

        /// <summary>
        /// Gets market kline data
        /// </summary>
        /// <param name="market">The market to get the data for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <returns></returns>
        Task<CallResult<CoinExKline>> GetMarketKlinesAsync(string market, KlineInterval interval);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.GetBalancesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<Dictionary<string, CoinExBalance>> GetBalances(params string[] coins);

        /// <summary>
        /// Get balances of coins. Requires API credentials
        /// </summary>
        /// <param name="coins">The coins to get the balances for, empty for all</param>
        /// <returns>Dictionary of coins and their balances</returns>
        Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(params string[] coins);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.GetOpenOrdersAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExSocketPagedResult<CoinExSocketOrder>> GetOpenOrders(string market, TransactionType type, int offset, int limit);

        /// <summary>
        /// Gets a list of open orders for a market
        /// </summary>
        /// <param name="market">Market to get open orders for</param>
        /// <param name="type">The type of orders to get</param>
        /// <param name="offset">The offset in the list</param>
        /// <param name="limit">The limit of results</param>
        /// <returns>List of open orders</returns>
        Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOpenOrdersAsync(string market, TransactionType type, int offset, int limit);

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToMarketStateUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExStreamSubscription> SubscribeToMarketStateUpdates(string market, Action<string, CoinExSocketMarketState> onMessage);

        /// <summary>
        /// Subscribe to market state updates for a specific market
        /// </summary>
        /// <param name="market">Market to receive updates for</param>
        /// <param name="onMessage">Datahandler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketState]: the market state update</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="CoinExSocketClient.UnsubscribeFromStream"/> method</returns>
        Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketStateUpdatesAsync(string market, Action<string, CoinExSocketMarketState> onMessage);

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToMarketStateUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExStreamSubscription> SubscribeToMarketStateUpdates(Action<Dictionary<string, CoinExSocketMarketState>> onMessage);

        /// <summary>
        /// Subscribe to market state updates for all markets
        /// </summary>
        /// <param name="onMessage">Datahandler, receives a dictionary of market name -> market state</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="CoinExSocketClient.UnsubscribeFromStream"/> method</returns>
        Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketStateUpdatesAsync(Action<Dictionary<string, CoinExSocketMarketState>> onMessage);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.SubscribeToMarketDepthUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExStreamSubscription> SubscribeToMarketDepthUpdates(string market, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage);

        /// <summary>
        /// Subscribe to market depth updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="limit">The limit of results to receive in a update</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="onMessage">Datahandler, receives Param 1[string]: the market name, Param 2[bool]: whether this is a full update, or an update based on the last send data, Param 3[CoinExSocketMarketDepth]: the update data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="CoinExSocketClient.UnsubscribeFromStream"/> method</returns>
        Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketDepthUpdatesAsync(string market, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.SubscribeToMarketTransactionUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExStreamSubscription> SubscribeToMarketTransactionUpdates(string market, Action<string, CoinExSocketMarketTransaction[]> onMessage);

        /// <summary>
        /// Subscribe to market transaction updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates from</param>
        /// <param name="onMessage">Datahandler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketTransaction[]]: list of transactions</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="CoinExSocketClient.UnsubscribeFromStream"/> method</returns>
        Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketTransactionUpdatesAsync(string market, Action<string, CoinExSocketMarketTransaction[]> onMessage);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.SubscribeToMarketKlineUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExStreamSubscription> SubscribeToMarketKlineUpdates(string market, KlineInterval interval, Action<string, CoinExKline[]> onMessage);

        /// <summary>
        /// Subscribe to kline updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="interval">The interval of the candle to receive updates for</param>
        /// <param name="onMessage">Datahandler, receives Param 1[string]: the market name, Param 2[CoinExKline[]]: list of klines updated klines</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="CoinExSocketClient.UnsubscribeFromStream"/> method</returns>
        Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketKlineUpdatesAsync(string market, KlineInterval interval, Action<string, CoinExKline[]> onMessage);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.SubscribeToBalanceUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExStreamSubscription> SubscribeToBalanceUpdates(Action<Dictionary<string, CoinExBalance>> onMessage);

        /// <summary>
        /// Subscribe to updates of your balances, Receives updates whenever the balance for a coin changes
        /// </summary>
        /// <param name="onMessage">Datahandler, receives a dictionary of ciub name -> balance</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="CoinExSocketClient.UnsubscribeFromStream"/> method</returns>
        Task<CallResult<CoinExStreamSubscription>> SubscribeToBalanceUpdatesAsync(Action<Dictionary<string, CoinExBalance>> onMessage);

        /// <summary>
        /// Synchronized version of the <see cref="CoinExSocketClient.SubscribeToOrderUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        CallResult<CoinExStreamSubscription> SubscribeToOrderUpdates(string[] markets, Action<UpdateType, CoinExSocketOrder> onMessage);

        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// </summary>
        /// <param name="markets">The markets to receive order updates from</param>
        /// <param name="onMessage">Datahandler, receives Param 1[UpdateType]: the type of update, Param 2[CoinExSocketOrder]: the order that was updated</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="CoinExSocketClient.UnsubscribeFromStream"/> method</returns>
        Task<CallResult<CoinExStreamSubscription>> SubscribeToOrderUpdatesAsync(string[] markets, Action<UpdateType, CoinExSocketOrder> onMessage);

        /// <summary>
        /// Unsubscribes from a stream
        /// </summary>
        /// <param name="streamSubscription">The stream subscription received by subscribing</param>
        Task UnsubscribeFromStream(CoinExStreamSubscription streamSubscription);

        /// <summary>
        /// Unsubscribes from all streams
        /// </summary>
        Task UnsubscribeAllStreams();

        /// <summary>
        /// Dispose this instance
        /// </summary>
        void Dispose();

        void AddRateLimiter(IRateLimiter limiter);
        void RemoveRateLimiters();
    }
}