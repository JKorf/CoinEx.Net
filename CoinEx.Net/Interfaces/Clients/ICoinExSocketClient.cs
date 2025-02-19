using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;

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
        /// Update specific options
        /// </summary>
        /// <param name="options">Options to update. Only specific options are changable after the client has been created</param>
        void SetOptions(UpdateOptions options);

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}