using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.CommonClients;
using System;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV1
{
    /// <summary>
    /// Spot API
    /// </summary>
    public interface ICoinExRestClientSpotApi : IRestApiClient, IDisposable
    {
        /// <summary>
        /// DEPRECATED FROM 2024/09/25, USE SpotApiV2 INSTEAD
        /// </summary>
        ICoinExRestClientSpotApiAccount Account { get; }

        /// <summary>
        /// DEPRECATED FROM 2024/09/25, USE SpotApiV2 INSTEAD
        /// </summary>
        ICoinExRestClientSpotApiExchangeData ExchangeData { get; }

        /// <summary>
        /// DEPRECATED FROM 2024/09/25, USE SpotApiV2 INSTEAD
        /// </summary>
        ICoinExRestClientSpotApiTrading Trading { get; }

        /// <summary>
        /// Get the ISpotClient for this client. This is a common interface which allows for some basic operations without knowing any details of the exchange.
        /// </summary>
        /// <returns></returns>
        public ISpotClient CommonSpotClient { get; }
    }
}