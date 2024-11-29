using CoinEx.Net.Interfaces;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CoinEx.Net.SymbolOrderBooks
{
    /// <inheritdoc />
    public class CoinExOrderBookFactory : ICoinExOrderBookFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <inheritdoc />
        public IOrderBookFactory<CoinExOrderBookOptions> Spot { get; }

        /// <inheritdoc />
        public IOrderBookFactory<CoinExOrderBookOptions> Futures { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider">Service provider for resolving logging and clients</param>
        public CoinExOrderBookFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            Spot = new OrderBookFactory<CoinExOrderBookOptions>(CreateSpot, Create);
            Futures = new OrderBookFactory<CoinExOrderBookOptions>(CreateFutures, Create);
        }

        /// <inheritdoc />
        public ISymbolOrderBook Create(SharedSymbol symbol, Action<CoinExOrderBookOptions>? options = null)
        {
            var symbolName = symbol.GetSymbol(CoinExExchange.FormatSymbol);
            if (symbol.TradingMode == TradingMode.Spot)
                return CreateSpot(symbolName, options);

            return CreateFutures(symbolName, options);
        }

        /// <inheritdoc />
        public ISymbolOrderBook CreateSpot(string symbol, Action<CoinExOrderBookOptions>? options = null)
            => new CoinExSpotSymbolOrderBook(symbol,
                                        options,
                                        _serviceProvider.GetRequiredService<ILoggerFactory>(),
                                        _serviceProvider.GetRequiredService<ICoinExSocketClient>());

        /// <inheritdoc />
        public ISymbolOrderBook CreateFutures(string symbol, Action<CoinExOrderBookOptions>? options = null)
            => new CoinExFuturesSymbolOrderBook(symbol,
                                        options,
                                        _serviceProvider.GetRequiredService<ILoggerFactory>(),
                                        _serviceProvider.GetRequiredService<ICoinExSocketClient>());
    }
}
