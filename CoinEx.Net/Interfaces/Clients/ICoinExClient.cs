using CoinEx.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Interfaces;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the CoinEx API. 
    /// </summary>
    public interface ICoinExClient : IRestClient
    {
        /// <summary>
        /// Spot endpoints
        /// </summary>
        ICoinExClientSpotApi SpotApi { get; }
    }
}