﻿using CoinEx.Net.Interfaces.Clients.FuturesApi;
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
        /// DEPRECATED FROM 2024/09/25, USE SpotApiV2 INSTEAD
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
        /// Update specific options
        /// </summary>
        /// <param name="options">Options to update. Only specific options are changable after the client has been created</param>
        void SetOptions(UpdateOptions options);

        /// <summary>
        /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
        /// </summary>
        /// <param name="credentials">The credentials to set</param>
        void SetApiCredentials(ApiCredentials credentials);
    }
}