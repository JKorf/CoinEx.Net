using CryptoExchange.Net.Interfaces;
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
    public interface ICoinExTrackerFactory : ITrackerFactory
    {
    }
}
