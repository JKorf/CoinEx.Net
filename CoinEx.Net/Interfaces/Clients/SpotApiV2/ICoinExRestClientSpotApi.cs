using CryptoExchange.Net.Interfaces;
using System;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV2
{
    /// <summary>
    /// Spot API
    /// </summary>
    public interface ICoinExRestClientSpotApi : IRestApiClient, IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        ICoinExRestClientSpotApiAccount Account { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        ICoinExRestClientSpotApiExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        ICoinExRestClientSpotApiTrading Trading { get; }

        /// <summary>
        /// Get the shared rest requests client. This interface is shared with other exchanges to allow for a common implementation for different exchanges.
        /// </summary>
        ICoinExRestClientSpotApiShared SharedClient { get; }

    }
}