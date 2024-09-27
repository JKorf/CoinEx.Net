using CryptoExchange.Net.Interfaces;
using System;

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// Futures API
    /// </summary>
    public interface ICoinExRestClientFuturesApi : IRestApiClient, IDisposable
    {
        /// <summary>
        /// Endpoints related to account settings, info or actions
        /// </summary>
        ICoinExRestClientFuturesApiAccount Account { get; }

        /// <summary>
        /// Endpoints related to retrieving market and system data
        /// </summary>
        ICoinExRestClientFuturesApiExchangeData ExchangeData { get; }

        /// <summary>
        /// Endpoints related to orders, trades and managing positions
        /// </summary>
        ICoinExRestClientFuturesApiTrading Trading { get; }

        /// <summary>
        /// Get the shared rest requests client. This interface is shared with other exhanges to allow for a common implementation for different exchanges.
        /// </summary>
        ICoinExRestClientFuturesApiShared SharedClient { get; }
    }
}