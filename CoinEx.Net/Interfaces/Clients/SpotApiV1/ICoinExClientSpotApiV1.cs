using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.CommonClients;
using System;

namespace CoinEx.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Spot API
    /// </summary>
    public interface ICoinExClientSpotApiV1 : IRestApiClient, IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        ICoinExClientSpotApiV1Account Account { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        ICoinExClientSpotApiV1ExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        ICoinExClientSpotApiV1Trading Trading { get; }

        /// <summary>
        /// Get the ISpotClient for this client. This is a common interface which allows for some basic operations without knowing any details of the exchange.
        /// </summary>
        /// <returns></returns>
        public ISpotClient CommonSpotClient { get; }
    }
}