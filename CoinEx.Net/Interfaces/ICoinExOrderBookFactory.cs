using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Interfaces;
using System;

namespace CoinEx.Net.Interfaces
{
    /// <summary>
    /// CoinEx order book factory
    /// </summary>
    public interface ICoinExOrderBookFactory
    {
        /// <summary>
        /// Create a SymbolOrderBook for the Spot API
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="options">Order book options</param>
        /// <returns></returns>
        ISymbolOrderBook CreateSpot(string symbol, Action<CoinExOrderBookOptions>? options = null);
    }
}