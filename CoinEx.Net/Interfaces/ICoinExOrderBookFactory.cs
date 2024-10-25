using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.SharedApis;
using System;

namespace CoinEx.Net.Interfaces
{
    /// <summary>
    /// CoinEx order book factory
    /// </summary>
    public interface ICoinExOrderBookFactory
    {
        /// <summary>
        /// Spot order book factory methods
        /// </summary>
        public IOrderBookFactory<CoinExOrderBookOptions> Spot { get; }

        /// <summary>
        /// Futures order book factory methods
        /// </summary>
        public IOrderBookFactory<CoinExOrderBookOptions> Futures { get; }

        /// <summary>
        /// Create a SymbolOrderBook for the symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="options">Book options</param>
        /// <returns></returns>
        ISymbolOrderBook Create(SharedSymbol symbol, Action<CoinExOrderBookOptions>? options = null);

        /// <summary>
        /// Create a SymbolOrderBook for the Spot API
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="options">Order book options</param>
        /// <returns></returns>
        ISymbolOrderBook CreateSpot(string symbol, Action<CoinExOrderBookOptions>? options = null);

        /// <summary>
        /// Create a SymbolOrderBook for the Futures API
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="options">Order book options</param>
        /// <returns></returns>
        ISymbolOrderBook CreateFutures(string symbol, Action<CoinExOrderBookOptions>? options = null);
    }
}