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
        /// Endpoints related to account settings, info or actions
        /// </summary>
        ICoinExClientSpotApi SpotApi { get; }
    }
}