﻿using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for accessing the CoinEx API. 
    /// </summary>
    public interface ICoinExRestClient : IRestClient
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

        /// <summary>
        /// Update specific options
        /// </summary>
        /// <param name="options">Options to update. Only specific options are changeable after the client has been created</param>
        void SetOptions(UpdateOptions options);

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}