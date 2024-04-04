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

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// Futures streams
    /// </summary>
    public interface ICoinExSocketClientFuturesApi : ISocketApiClient, IDisposable
    {
    }
}