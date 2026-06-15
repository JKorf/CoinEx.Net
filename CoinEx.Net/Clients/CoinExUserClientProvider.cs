using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc />
    public class CoinExUserClientProvider : UserClientProvider<
        ICoinExRestClient,
        ICoinExSocketClient,
        CoinExRestOptions,
        CoinExSocketOptions,
        CoinExCredentials,
        CoinExEnvironment
        >, ICoinExUserClientProvider
    {
        /// <inheritdoc />
        public override string ExchangeName => CoinExExchange.ExchangeName;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="optionsDelegate">Options to use for created clients</param>
        public CoinExUserClientProvider(Action<CoinExOptions>? optionsDelegate = null)
            : this(null, null, Options.Create(ApplyOptionsDelegate(optionsDelegate).Rest), Options.Create(ApplyOptionsDelegate(optionsDelegate).Socket))
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExUserClientProvider(
            HttpClient? httpClient,
            ILoggerFactory? loggerFactory,
            IOptions<CoinExRestOptions> restOptions,
            IOptions<CoinExSocketOptions> socketOptions)
            : base(httpClient, loggerFactory, restOptions, socketOptions)
        {
        }

        /// <inheritdoc />
        protected override ICoinExRestClient ConstructRestClient(HttpClient client, ILoggerFactory? loggerFactory, IOptions<CoinExRestOptions> options)
            => new CoinExRestClient(client, loggerFactory, options);
        /// <inheritdoc />
        protected override ICoinExSocketClient ConstructSocketClient(ILoggerFactory? loggerFactory, IOptions<CoinExSocketOptions> options)
            => new CoinExSocketClient(options, loggerFactory);
    }
}
