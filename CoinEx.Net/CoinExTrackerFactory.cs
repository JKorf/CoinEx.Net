using CoinEx.Net.Interfaces;
using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CoinEx.Net
{
    /// <inheritdoc />
    public class CoinExTrackerFactory : ICoinExTrackerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public CoinExTrackerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public ITradeTracker CreateTradeTracker(SharedSymbol symbol, int? limit = null, TimeSpan? period = null)
        {
            IRecentTradeRestClient restClient;
            ITradeSocketClient socketClient;
            if (symbol.TradingMode == TradingMode.Spot)
            {
                restClient = _serviceProvider.GetRequiredService<ICoinExRestClient>().SpotApiV2.SharedClient;
                socketClient = _serviceProvider.GetRequiredService<ICoinExSocketClient>().SpotApiV2.SharedClient;
            }
            else
            {
                restClient = _serviceProvider.GetRequiredService<ICoinExRestClient>().SpotApiV2.SharedClient;
                socketClient = _serviceProvider.GetRequiredService<ICoinExSocketClient>().SpotApiV2.SharedClient;
            }

            return new TradeTracker(
                _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(restClient.Exchange),
                restClient,
                socketClient,
                symbol,
                limit,
                period
                );
        }
    }
}
