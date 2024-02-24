using CryptoExchange.Net;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.Clients.SpotApi;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using System;
using CoinEx.Net.Objects.Options;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc cref="ICoinExSocketClient" />
    public class CoinExSocketClient : BaseSocketClient, ICoinExSocketClient
    {
        #region Api clients

        /// <inheritdoc />
        public ICoinExSocketClientSpotApi SpotApi { get; }

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

            SpotApi = AddApiClient(new CoinExSocketClientSpotApi(_logger, options));
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
            SpotApi.SetApiCredentials(credentials);
        }
    }
}
