using CryptoExchange.Net.Objects.Options;
using System;

namespace CoinEx.Net.Objects.Options
{
    /// <summary>
    /// Options for CoinEx SymbolOrderBook
    /// </summary>
    public class CoinExOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// Default options for the CoinEx SymbolOrderBook
        /// </summary>
        public static CoinExOrderBookOptions Default { get; set; } = new CoinExOrderBookOptions();

        /// <summary>
        /// After how much time we should consider the connection dropped if no data is received for this time after the initial subscriptions
        /// </summary>
        public TimeSpan? InitialDataTimeout { get; set; }

        /// <summary>
        /// The amount of rows. Should be one of: 5/10/20/50
        /// </summary>
        public int? Limit { get; set; }

        internal CoinExOrderBookOptions Copy()
        {
            var options = Copy<CoinExOrderBookOptions>();
            options.InitialDataTimeout = InitialDataTimeout;
            options.Limit = Limit;
            return options;
        }
    }
}
