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
        /// V2 API Spot streams
        /// </summary>
        public SpotApiV2.ICoinExSocketClientSpotApi SpotApiV2 { get; }
        /// <summary>
        /// V1 API Spot streams. Use V2 if possible, V1 will be removed at a later date
        /// </summary>
        public SpotApiV1.ICoinExSocketClientSpotApi SpotApi { get; }

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}