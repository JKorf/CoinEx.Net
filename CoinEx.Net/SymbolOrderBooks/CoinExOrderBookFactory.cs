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

            Spot = new OrderBookFactory<CoinExOrderBookOptions>(
                CreateSpot,
                (sharedSymbol, options) => CreateSpot(CoinExExchange.FormatSymbol(sharedSymbol.BaseAsset, sharedSymbol.QuoteAsset, sharedSymbol.TradingMode, sharedSymbol.DeliverTime), options));
            Futures = new OrderBookFactory<CoinExOrderBookOptions>(
                CreateFutures,
                (sharedSymbol, options) => CreateFutures(CoinExExchange.FormatSymbol(sharedSymbol.BaseAsset, sharedSymbol.QuoteAsset, sharedSymbol.TradingMode, sharedSymbol.DeliverTime), options));
        }

        /// <inheritdoc />
        public ISymbolOrderBook Create(SharedSymbol symbol, Action<CoinExOrderBookOptions>? options = null)
        {
            var symbolName = CoinExExchange.FormatSymbol(symbol.BaseAsset, symbol.QuoteAsset, symbol.TradingMode, symbol.DeliverTime);
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
