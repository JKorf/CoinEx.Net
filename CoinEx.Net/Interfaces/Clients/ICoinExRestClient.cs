using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the CoinEx API. 
    /// </summary>
    public interface ICoinExRestClient : IRestClient
    {
        /// <summary>
        /// Spot V1 API endpoints. Use V2 API if possible, V1 API will be removed at a later date
        /// </summary>
        SpotApiV1.ICoinExRestClientSpotApi SpotApi { get; }
        /// <summary>
        /// Spot V2 API endpoints
        /// </summary>
        SpotApiV2.ICoinExRestClientSpotApi SpotApiV2 { get; }
        /// <summary>
        /// Futures V2 API endpoints
        /// </summary>
        ICoinExRestClientFuturesApi FuturesApi { get; }

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}