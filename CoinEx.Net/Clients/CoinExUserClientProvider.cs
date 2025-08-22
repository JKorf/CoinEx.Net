using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Authentication;
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
    public class CoinExUserClientProvider : ICoinExUserClientProvider
    {
        private static ConcurrentDictionary<string, ICoinExRestClient> _restClients = new ConcurrentDictionary<string, ICoinExRestClient>();
        private static ConcurrentDictionary<string, ICoinExSocketClient> _socketClients = new ConcurrentDictionary<string, ICoinExSocketClient>();

        private readonly IOptions<CoinExRestOptions> _restOptions;
        private readonly IOptions<CoinExSocketOptions> _socketOptions;
        private readonly HttpClient _httpClient;
        private readonly ILoggerFactory? _loggerFactory;

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
        {
            _httpClient = httpClient ?? new HttpClient();
            _loggerFactory = loggerFactory;
            _restOptions = restOptions;
            _socketOptions = socketOptions;
        }

        /// <inheritdoc />
        public void InitializeUserClient(string userIdentifier, ApiCredentials credentials, CoinExEnvironment? environment = null)
        {
            CreateRestClient(userIdentifier, credentials, environment);
            CreateSocketClient(userIdentifier, credentials, environment);
        }

        /// <inheritdoc />
        public void ClearUserClients(string userIdentifier)
        {
            _restClients.TryRemove(userIdentifier, out _);
            _socketClients.TryRemove(userIdentifier, out _);
        }

        /// <inheritdoc />
        public ICoinExRestClient GetRestClient(string userIdentifier, ApiCredentials? credentials = null, CoinExEnvironment? environment = null)
        {
            if (!_restClients.TryGetValue(userIdentifier, out var client))
                client = CreateRestClient(userIdentifier, credentials, environment);

            return client;
        }

        /// <inheritdoc />
        public ICoinExSocketClient GetSocketClient(string userIdentifier, ApiCredentials? credentials = null, CoinExEnvironment? environment = null)
        {
            if (!_socketClients.TryGetValue(userIdentifier, out var client))
                client = CreateSocketClient(userIdentifier, credentials, environment);

            return client;
        }

        private ICoinExRestClient CreateRestClient(string userIdentifier, ApiCredentials? credentials, CoinExEnvironment? environment)
        {
            var clientRestOptions = SetRestEnvironment(environment);
            var client = new CoinExRestClient(_httpClient, _loggerFactory, clientRestOptions);
            if (credentials != null)
            {
                client.SetApiCredentials(credentials);
                _restClients.TryAdd(userIdentifier, client);
            }
            return client;
        }

        private ICoinExSocketClient CreateSocketClient(string userIdentifier, ApiCredentials? credentials, CoinExEnvironment? environment)
        {
            var clientSocketOptions = SetSocketEnvironment(environment);
            var client = new CoinExSocketClient(clientSocketOptions!, _loggerFactory);
            if (credentials != null)
            {
                client.SetApiCredentials(credentials);
                _socketClients.TryAdd(userIdentifier, client);
            }
            return client;
        }

        private IOptions<CoinExRestOptions> SetRestEnvironment(CoinExEnvironment? environment)
        {
            if (environment == null)
                return _restOptions;

            var newRestClientOptions = new CoinExRestOptions();
            var restOptions = _restOptions.Value.Set(newRestClientOptions);
            newRestClientOptions.Environment = environment;
            return Options.Create(newRestClientOptions);
        }

        private IOptions<CoinExSocketOptions> SetSocketEnvironment(CoinExEnvironment? environment)
        {
            if (environment == null)
                return _socketOptions;

            var newSocketClientOptions = new CoinExSocketOptions();
            var restOptions = _socketOptions.Value.Set(newSocketClientOptions);
            newSocketClientOptions.Environment = environment;
            return Options.Create(newSocketClientOptions);
        }

        private static T ApplyOptionsDelegate<T>(Action<T>? del) where T : new()
        {
            var opts = new T();
            del?.Invoke(opts);
            return opts;
        }
    }
}
