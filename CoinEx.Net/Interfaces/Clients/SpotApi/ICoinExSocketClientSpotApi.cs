using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Enums;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Objects.Models.Socket;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;

namespace CoinEx.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Spot streams
    /// </summary>
    public interface ICoinExSocketClientSpotApi : ISocketApiClient, IDisposable
    {
        /// <summary>
        /// Subscribe to symbol ticker updates for the specified symbols. Note that only one ticker subscription can be active at the same time; new ticker subscription will override the old subscriptions.
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market" /></para>
        /// </summary>
        /// <param name="symbols">The symbols to subscribe</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<IEnumerable<CoinExTicker>>> onMessage, CancellationToken ct = default);
        
        /// <summary>
        /// Subscribe to symbol ticker updates for all symbols. Note that only one ticker subscription can be active at the same time; new ticker subscription will override the old subscriptions.
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(Action<DataEvent<IEnumerable<CoinExTicker>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-depth" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="depth">Order book depth, 5, 10, 20 or 50</param>
        /// <param name="mergeLevel">The merge level, 0.00000000001 up to 1000, 0 for no merging</param>
        /// <param name="fullBookUpdates">Whether updates should provide full update or only updates</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<Objects.Models.V2.CoinExOrderBook>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to order book updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-depth" /></para>
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <param name="depth">Order book depth, 5, 10, 20 or 50</param>
        /// <param name="mergeLevel">The merge level, 0.00000000001 up to 1000, 0 for no merging</param>
        /// <param name="fullBookUpdates">Whether updates should provide full update or only updates</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(IEnumerable<string> symbols, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<Objects.Models.V2.CoinExOrderBook>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to live trade updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<CoinExTrade>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to live trade updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-deals" /></para>
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<IEnumerable<CoinExTrade>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to live trade updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-deals" /></para>
        /// </summary>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(Action<DataEvent<IEnumerable<CoinExTrade>>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to index price updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-index" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToIndexPriceUpdatesAsync(string symbol, Action<DataEvent<CoinExIndexPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to index price updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-index" /></para>
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToIndexPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExIndexPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to book price updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-bbo" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBookPriceUpdatesAsync(string symbol, Action<DataEvent<CoinExBookPriceUpdate>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// Subscribe to book price updates
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/ws/market-bbo" /></para>
        /// </summary>
        /// <param name="symbols">Symbols</param>
        /// <param name="onMessage">Data handler</param>
        /// <param name="ct">Cancellation token for closing this subscription</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        Task<CallResult<UpdateSubscription>> SubscribeToBookPriceUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExBookPriceUpdate>> onMessage, CancellationToken ct = default);
    }
}