using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Enums;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Objects.Models.Socket;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV1
{
    /// <summary>
    /// Spot streams
    /// </summary>
    public interface ICoinExSocketClientSpotApi : ISocketApiClient, IDisposable
    {
        /// <summary>
        /// Pings the server
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/048ping" /></para>
        /// </summary>
        /// <returns>Success if server responded, error otherwise</returns>
        Task<CallResult> PingAsync();

        /// <summary>
        /// Gets the server time
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/049time" /></para>
        /// </summary>
        /// <returns>The server time</returns>
        Task<CallResult<DateTime>> GetServerTimeAsync();

        /// <summary>
        /// Get the symbol ticker
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/053state" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the state for</param>
        /// <param name="cyclePeriod">The period to get data over, specified in seconds. i.e. one minute = 60, one day = 86400</param>
        /// <returns>Symbol state</returns>
        Task<CallResult<CoinExSocketSymbolState>> GetTickerAsync(string symbol, int cyclePeriod);

        /// <summary>
        /// Subscribe to symbol ticker updates for a specific symbol
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/053state" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to receive updates for</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<CoinExSocketSymbolState>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to ticker updates for all symbols
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/053state" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToAllTickerUpdatesAsync(Action<DataEvent<IEnumerable<CoinExSocketSymbolState>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Get an order book
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/044depth" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the order book for</param>
        /// <param name="limit">The limit of results returned, 5, 10 or 20</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <returns>Order book of a symbol</returns>
        Task<CallResult<CoinExSocketOrderBook>> GetOrderBookAsync(string symbol, int limit, int mergeDepth);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/044depth" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to receive updates for</param>
        /// <param name="limit">The limit of results to receive in a update</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="diffUpdates">Set to true to get snapshot first, then diff updates</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int limit, int mergeDepth, Action<DataEvent<CoinExSocketOrderBook>> onMessage, bool diffUpdates = false, CancellationToken ct = default);

        /// <summary>
        /// Gets the latest trades on a symbol
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/045deals" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the trades for</param>
        /// <param name="limit">The limit of trades</param>
        /// <param name="fromId">Return trades since this id</param>
        /// <returns>List of trades</returns>
        Task<CallResult<IEnumerable<CoinExSocketSymbolTrade>>> GetTradeHistoryAsync(string symbol, int? limit = null, int? fromId = null);

        /// <summary>
        /// Subscribe to symbol trade updates for a symbol
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/045deals" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to receive updates from</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<CoinExSocketSymbolTrade>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Gets symbol kline data
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/046kline" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to get the data for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <returns></returns>
        Task<CallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval);

        /// <summary>
        /// Get balances of assets. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/051asset" /></para>
        /// </summary>
        /// <param name="assets">The assets to get the balances for, empty for all</param>
        /// <returns>Dictionary of assets and their balances</returns>
        Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(IEnumerable<string>? assets = null);

        /// <summary>
        /// Subscribe to updates of your balances, Receives updates whenever the balance for an asset changes
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/051asset" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<IEnumerable<CoinExBalance>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of open orders for a symbol
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/052order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol to get open orders for</param>
        /// <param name="side">Order side</param>
        /// <param name="offset">The offset in the list</param>
        /// <param name="limit">The limit of results</param>
        /// <returns>List of open orders</returns>
        Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOpenOrdersAsync(string symbol, OrderSide? side = null, int? offset = null, int? limit = null);

        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/052order" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/052order" /></para>
        /// </summary>
        /// <param name="symbols">The symbols to receive order updates from</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default);

    }
}