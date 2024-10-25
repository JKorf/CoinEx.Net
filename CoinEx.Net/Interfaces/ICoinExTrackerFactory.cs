using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.Klines;
using CryptoExchange.Net.Trackers.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Interfaces
{
    /// <summary>
    /// Tracker factory
    /// </summary>
    public interface ICoinExTrackerFactory
    {
        /// <summary>
        /// Create a new trade tracker for a symbol
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="limit">The max amount of klines to retain</param>
        /// <param name="period">The max period the data should be retained</param>
        /// <returns></returns>
        ITradeTracker CreateTradeTracker(SharedSymbol symbol, int? limit = null, TimeSpan? period = null);
    }
}
