using CoinEx.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Interface for the CoinEx socket client
    /// </summary>
    public interface ICoinExSocketClient : ISocketClient
    {
        public ICoinExSocketClientSpotStreams SpotStreams { get; }
    }
}