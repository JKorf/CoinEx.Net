using CoinEx.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the CoinEx websocket API
    /// </summary>
    public interface ICoinExSocketClient : ISocketClient
    {
        /// <summary>
        /// Spot streams
        /// </summary>
        public ICoinExSocketClientSpotStreams SpotStreams { get; }
    }
}