using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the CoinEx websocket API
    /// </summary>
    public interface ICoinExSocketClient : ISocketClient
    {
        /// <summary>
        /// V2 API Futures streams
        /// </summary>
        public ICoinExSocketClientFuturesApi FuturesApi { get; }
        /// <summary>
        /// V2 API Spot streams
        /// </summary>
        public SpotApiV2.ICoinExSocketClientSpotApi SpotApiV2 { get; }
        /// <summary>
        /// DEPRECATED FROM 2024/09/25, USE SpotApiV2 INSTEAD
        /// </summary>
        public SpotApiV1.ICoinExSocketClientSpotApi SpotApi { get; }

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}