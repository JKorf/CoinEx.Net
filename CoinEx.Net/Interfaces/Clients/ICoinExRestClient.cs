using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects.Options;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the CoinEx API. 
    /// </summary>
    public interface ICoinExRestClient : IRestClient<CoinExCredentials>
    {
        /// <summary>
        /// Spot V2 API endpoints
        /// </summary>
        /// <see cref="ICoinExRestClientSpotApi"/>
        ICoinExRestClientSpotApi SpotApiV2 { get; }
        /// <summary>
        /// Futures V2 API endpoints
        /// </summary>
        /// <see cref="ICoinExRestClientFuturesApi"/>
        ICoinExRestClientFuturesApi FuturesApi { get; }
    }
}