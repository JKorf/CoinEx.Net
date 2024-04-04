using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Clients.FuturesApi;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc cref="ICoinExRestClient" />
    public class CoinExRestClient : BaseRestClient, ICoinExRestClient
    {
        #region Api clients
        /// <inheritdoc />
        public ICoinExRestClientFuturesApi FuturesApi { get; }
        /// <inheritdoc />
        public Interfaces.Clients.SpotApiV1.ICoinExRestClientSpotApi SpotApi { get; }
        /// <inheritdoc />
        public Interfaces.Clients.SpotApiV2.ICoinExRestClientSpotApi SpotApiV2 { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of the CoinExRestClient using provided options
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public CoinExRestClient(Action<CoinExRestOptions>? optionsDelegate = null) : this(null, null, optionsDelegate)
        {
        }

        /// <summary>
        /// Create a new instance of the CoinExRestClient
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="httpClient">Http client for this client</param>
        public CoinExRestClient(HttpClient? httpClient, ILoggerFactory? loggerFactory, Action<CoinExRestOptions>? optionsDelegate = null)
            : base(loggerFactory, "CoinEx")
        {
            var options = CoinExRestOptions.Default.Copy();
            if (optionsDelegate != null)
                optionsDelegate(options);
            Initialize(options);

            FuturesApi = AddApiClient(new CoinExRestClientFuturesApi(_logger, httpClient, options));
            SpotApi = AddApiClient(new SpotApiV1.CoinExRestClientSpotApi(_logger, httpClient, options));
            SpotApiV2 = AddApiClient(new SpotApiV2.CoinExRestClientSpotApi(_logger, httpClient, options));
        }
        #endregion

        #region methods
        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<CoinExRestOptions> optionsDelegate)
        {
            var options = CoinExRestOptions.Default.Copy();
            optionsDelegate(options);
            CoinExRestOptions.Default = options;
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            FuturesApi.SetApiCredentials(credentials);
            SpotApi.SetApiCredentials(credentials);
            SpotApiV2.SetApiCredentials(credentials);
        }
        #endregion
    }
}
