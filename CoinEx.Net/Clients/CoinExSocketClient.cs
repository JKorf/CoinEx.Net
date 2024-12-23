using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using System;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Clients;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Clients.FuturesApi;
using Microsoft.Extensions.Options;
using CryptoExchange.Net.Objects.Options;

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
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public CoinExSocketClient(Action<CoinExSocketOptions>? optionsDelegate = null)
            : this(Options.Create(ApplyOptionsDelegate(optionsDelegate)), null)
        {
        }

        /// <summary>
        /// Create a new instance of the CoinExSocketClient
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        /// <param name="options">Option configuration</param>
        public CoinExSocketClient(IOptions<CoinExSocketOptions> options, ILoggerFactory? loggerFactory = null) : base(loggerFactory, "CoinEx")
        {
            Initialize(options.Value);

            FuturesApi = AddApiClient(new CoinExSocketClientFuturesApi(_logger, options.Value));
            SpotApi = AddApiClient(new SpotApiV1.CoinExSocketClientSpotApi(_logger, options.Value));
            SpotApiV2 = AddApiClient(new SpotApiV2.CoinExSocketClientSpotApi(_logger, options.Value));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public static void SetDefaultOptions(Action<CoinExSocketOptions> optionsDelegate)
        {
            CoinExSocketOptions.Default = ApplyOptionsDelegate(optionsDelegate);
        }

        /// <inheritdoc />
        public void SetOptions(UpdateOptions options)
        {
            FuturesApi.SetOptions(options);
            SpotApi.SetOptions(options);
            SpotApiV2.SetOptions(options);
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
