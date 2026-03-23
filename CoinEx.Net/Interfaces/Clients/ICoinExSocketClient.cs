using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects.Options;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the CoinEx websocket API
    /// </summary>
    public interface ICoinExSocketClient : ISocketClient<CoinExCredentials>
    {
        /// <summary>
        /// V2 API Futures streams
        /// </summary>
        /// <see cref="ICoinExSocketClientFuturesApi"/>
        public ICoinExSocketClientFuturesApi FuturesApi { get; }
        /// <summary>
        /// V2 API Spot streams
        /// </summary>
        /// <see cref="ICoinExSocketClientSpotApi"/>
        public ICoinExSocketClientSpotApi SpotApiV2 { get; }
    }
}