using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// Futures streams
    /// </summary>
    public interface ICoinExSocketClientFuturesApi : ISocketApiClient, IDisposable
    {
        /// <summary>
        /// Get the shared socket subscription client. This interface is shared with other exchanges to allow for a common implementation for different exchanges.
        /// </summary>
        ICoinExSocketClientFuturesApiShared SharedClient { get; }

        /// <summary>
        /// Subscribe to symbol ticker updates for all symbols. Note that only one ticker subscription can be active at the same time; new ticker subscription will override the old subscriptions.
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-state" /></para>
        /// </summary>
        /// <param name="symbols">The symbols to subscribe, for example `ETHUSDT`</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExFuturesTickerUpdate[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to symbol ticker updates for all symbols. Note that only one ticker subscription can be active at the same time; new ticker subscription will override the old subscriptions.
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-state" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(Action<DataEvent<CoinExFuturesTickerUpdate[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-depth" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="depth">Order book depth, 5, 10, 20 or 50</param>
        /// <param name="mergeLevel">The merge level, 0.00000000001 up to 1000, 0 for no merging</param>
        /// <param name="fullBookUpdates">Whether updates should provide full update or only updates</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<Objects.Models.V2.CoinExOrderBook>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-depth" /></para>
        /// </summary>
        /// <param name="symbols">Symbols, for example `ETHUSDT`</param>
        /// <param name="depth">Order book depth, 5, 10, 20 or 50</param>
        /// <param name="mergeLevel">The merge level, 0.00000000001 up to 1000, 0 for no merging</param>
        /// <param name="fullBookUpdates">Whether updates should provide full update or only updates</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(IEnumerable<string> symbols, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<Objects.Models.V2.CoinExOrderBook>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to live trade updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<CoinExTrade[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to live trade updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-deals" /></para>
        /// </summary>
        /// <param name="symbols">Symbols, for example `ETHUSDT`</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExTrade[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to live trade updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-deals" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(Action<DataEvent<CoinExTrade[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to index price updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-index" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToIndexPriceUpdatesAsync(string symbol, Action<DataEvent<CoinExIndexPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to index price updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-index" /></para>
        /// </summary>
        /// <param name="symbols">Symbols, for example `ETHUSDT`</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToIndexPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExIndexPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to book price updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-bbo" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBookPriceUpdatesAsync(string symbol, Action<DataEvent<CoinExBookPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to book price updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/ws/market-bbo" /></para>
        /// </summary>
        /// <param name="symbols">Symbols, for example `ETHUSDT`</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBookPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExBookPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user order updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/ws/user-order" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExFuturesOrderUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user stop order updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/ws/user-stop-order" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToStopOrderUpdatesAsync(Action<DataEvent<CoinExStopOrderUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user trade updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/deal/ws/user-deals" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToUserTradeUpdatesAsync(Action<DataEvent<CoinExUserTrade>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user balance updates
        /// <para><a href="https://docs.coinex.com/api/v2/assets/balance/ws/futures_balance" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<CoinExFuturesBalance[]>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to user position updates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/ws/user-position" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(Action<DataEvent<CoinExPositionUpdate>> onMessage, CancellationToken ct = default);
    }
}
