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
        Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(Action<DataEvent<CoinExTickerUpdateWrapper>> onMessage, CancellationToken ct = default);
    }
}