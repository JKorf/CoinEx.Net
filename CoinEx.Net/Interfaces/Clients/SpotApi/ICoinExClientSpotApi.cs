using System;

namespace CoinEx.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// Spot API
    /// </summary>
    public interface ICoinExClientSpotApi : IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        ICoinExClientSpotApiAccount Account { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        ICoinExClientSpotApiExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders and trades
        /// </summary>
        ICoinExClientSpotApiTrading Trading { get; }
    }
}