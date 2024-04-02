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
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="depth"></param>
        /// <param name="mergeLevel"></param>
        /// <param name="fullBookUpdates"></param>
        /// <param name="onMessage"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<Objects.Models.V2.CoinExOrderBook>> onMessage, CancellationToken ct = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="depth"></param>
        /// <param name="mergeLevel"></param>
        /// <param name="fullBookUpdates"></param>
        /// <param name="onMessage"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(IEnumerable<string> symbols, int depth, string? mergeLevel, bool fullBookUpdates, Action<DataEvent<Objects.Models.V2.CoinExOrderBook>> onMessage, CancellationToken ct = default);
    }
}