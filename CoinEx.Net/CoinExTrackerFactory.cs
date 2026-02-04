using CoinEx.Net.Clients;
using CoinEx.Net.Interfaces;
using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using CryptoExchange.Net.Trackers.UserData.Interfaces;
using CryptoExchange.Net.Trackers.UserData.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace CoinEx.Net
{
    /// <inheritdoc />
    public class CoinExTrackerFactory : ICoinExTrackerFactory
    {
        private readonly IServiceProvider? _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExTrackerFactory()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public CoinExTrackerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        /// <inheritdoc />
        public bool CanCreateKlineTracker(SharedSymbol symbol, SharedKlineInterval interval) => false;

        /// <inheritdoc />
        public bool CanCreateTradeTracker(SharedSymbol symbol) => true;

        /// <inheritdoc />
        public IKlineTracker CreateKlineTracker(SharedSymbol symbol, SharedKlineInterval interval, int? limit = null, TimeSpan? period = null) => throw new NotImplementedException();

        /// <inheritdoc />
        public ITradeTracker CreateTradeTracker(SharedSymbol symbol, int? limit = null, TimeSpan? period = null)
        {
            var restClient = _serviceProvider?.GetRequiredService<ICoinExRestClient>() ?? new CoinExRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<ICoinExSocketClient>() ?? new CoinExSocketClient();

            IRecentTradeRestClient sharedRestClient;
            ITradeSocketClient sharedSocketClient;
            if (symbol.TradingMode == TradingMode.Spot)
            {
                sharedRestClient = restClient.SpotApiV2.SharedClient;
                sharedSocketClient = socketClient.SpotApiV2.SharedClient;
            }
            else
            {
                sharedRestClient = restClient.FuturesApi.SharedClient;
                sharedSocketClient = socketClient.FuturesApi.SharedClient;
            }

            return new TradeTracker(
                _serviceProvider?.GetRequiredService<ILoggerFactory>().CreateLogger(restClient.Exchange),
                sharedRestClient,
                null,
                sharedSocketClient,
                symbol,
                limit,
                period
                );
        }

        /// <inheritdoc />
        public IUserSpotDataTracker CreateUserSpotDataTracker(SpotUserDataTrackerConfig config)
        {
            var restClient = _serviceProvider?.GetRequiredService<ICoinExRestClient>() ?? new CoinExRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<ICoinExSocketClient>() ?? new CoinExSocketClient();
            return new CoinExUserSpotDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<CoinExUserSpotDataTracker>>() ?? new NullLogger<CoinExUserSpotDataTracker>(),
                restClient,
                socketClient,
                null,
                config
                );
        }

        /// <inheritdoc />
        public IUserSpotDataTracker CreateUserSpotDataTracker(string userIdentifier, SpotUserDataTrackerConfig config, ApiCredentials credentials, CoinExEnvironment? environment = null)
        {
            var clientProvider = _serviceProvider?.GetRequiredService<ICoinExUserClientProvider>() ?? new CoinExUserClientProvider();
            var restClient = clientProvider.GetRestClient(userIdentifier, credentials, environment);
            var socketClient = clientProvider.GetSocketClient(userIdentifier, credentials, environment);
            return new CoinExUserSpotDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<CoinExUserSpotDataTracker>>() ?? new NullLogger<CoinExUserSpotDataTracker>(),
                restClient,
                socketClient,
                userIdentifier,
                config
                );
        }

        /// <inheritdoc />
        public IUserFuturesDataTracker CreateUserFuturesDataTracker(FuturesUserDataTrackerConfig config)
        {
            var restClient = _serviceProvider?.GetRequiredService<ICoinExRestClient>() ?? new CoinExRestClient();
            var socketClient = _serviceProvider?.GetRequiredService<ICoinExSocketClient>() ?? new CoinExSocketClient();
            return new CoinExUserFuturesDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<CoinExUserFuturesDataTracker>>() ?? new NullLogger<CoinExUserFuturesDataTracker>(),
                restClient,
                socketClient,
                null,
                config
                );
        }

        /// <inheritdoc />
        public IUserFuturesDataTracker CreateUserFuturesDataTracker(string userIdentifier, FuturesUserDataTrackerConfig config, ApiCredentials credentials, CoinExEnvironment? environment = null)
        {
            var clientProvider = _serviceProvider?.GetRequiredService<ICoinExUserClientProvider>() ?? new CoinExUserClientProvider();
            var restClient = clientProvider.GetRestClient(userIdentifier, credentials, environment);
            var socketClient = clientProvider.GetSocketClient(userIdentifier, credentials, environment);
            return new CoinExUserFuturesDataTracker(
                _serviceProvider?.GetRequiredService<ILogger<CoinExUserFuturesDataTracker>>() ?? new NullLogger<CoinExUserFuturesDataTracker>(),
                restClient,
                socketClient,
                userIdentifier,
                config
                );
        }
    }
}
