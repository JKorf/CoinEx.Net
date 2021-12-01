using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Enums;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Objects.Models.Socket;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace CoinEx.Net.Interfaces.Clients.Socket
{
    /// <summary>
    /// Interface for the CoinEx socket client
    /// </summary>
    public interface ICoinExSocketClient : ISocketClient
    {
        public ICoinExSocketClientSpotMarket SpotStreams { get; }
    }
}