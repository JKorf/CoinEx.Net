using CoinEx.Net.Objects;
using CryptoExchange.Net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json.Linq;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.Clients.SpotApi;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Authentication;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc cref="ICoinExClient" />
    public class CoinExClient : BaseRestClient, ICoinExClient
    {
        #region Api clients
        /// <inheritdoc />
        public ICoinExClientSpotApi SpotApi { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExClient with default options
        /// </summary>
        public CoinExClient() : this(CoinExClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of CoinExClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public CoinExClient(CoinExClientOptions options) : base("CoinEx", options)
        {
            SpotApi = AddApiClient(new CoinExClientSpotApi(log, options));
        }
        #endregion

        #region methods
        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(CoinExClientOptions options)
        {
            CoinExClientOptions.Default = options;
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            SpotApi.SetApiCredentials(credentials);
        }
        #endregion
    }
}
