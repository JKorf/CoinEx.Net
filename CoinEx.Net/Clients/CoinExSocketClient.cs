using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using System;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Clients.FuturesApi;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc cref="ICoinExSocketClient" />
    public class CoinExSocketClient : BaseSocketClient, ICoinExSocketClient
    {
        #region Api clients

        /// <inheritdoc />
        public ICoinExSocketClientFuturesApi FuturesApi { get; }
        /// <inheritdoc />
        public Interfaces.Clients.SpotApiV2.ICoinExSocketClientSpotApi SpotApiV2 { get; }
        /// <inheritdoc />
        public Interfaces.Clients.SpotApiV1.ICoinExSocketClientSpotApi SpotApi { get; }

        #endregion

        #region ctor

        /// <summary>
        /// Create a new instance of the CoinExSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        public CoinExSocketClient(ILoggerFactory? loggerFactory = null) : this((x) => { }, loggerFactory)
        {
        }

        /// <summary>
        /// Create a new instance of the CoinExSocketClient
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public CoinExSocketClient(Action<CoinExSocketOptions> optionsDelegate) : this(optionsDelegate, null)
        {
        }

        /// <summary>
        /// Create a new instance of the CoinExSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public CoinExSocketClient(Action<CoinExSocketOptions> optionsDelegate, ILoggerFactory? loggerFactory = null) : base(loggerFactory, "CoinEx")
        {
            var options = CoinExSocketOptions.Default.Copy();
            optionsDelegate(options);
            Initialize(options);

            FuturesApi = AddApiClient(new CoinExSocketClientFuturesApi(_logger, options));
            SpotApi = AddApiClient(new SpotApiV1.CoinExSocketClientSpotApi(_logger, options));
            SpotApiV2 = AddApiClient(new SpotApiV2.CoinExSocketClientSpotApi(_logger, options));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<CoinExSocketOptions> optionsDelegate)
        {
            var options = CoinExSocketOptions.Default.Copy();
            optionsDelegate(options);
            CoinExSocketOptions.Default = options;
        }

        /// <inheritdoc />
        public void SetApiCredentials(ApiCredentials credentials)
        {
            FuturesApi.SetApiCredentials(credentials);
            SpotApiV2.SetApiCredentials(credentials);
            SpotApi.SetApiCredentials(credentials);
        }
    }
}
