using CoinEx.Net.Interfaces.Clients.SpotApi;
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
        /// Spot streams
        /// </summary>
        public ICoinExSocketClientSpotApiV1 SpotApiV1 { get; }

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}