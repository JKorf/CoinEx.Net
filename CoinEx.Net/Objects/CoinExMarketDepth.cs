using System.Collections.Generic;
using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Order book
    /// </summary>
    public class CoinExMarketDepth
    {
        /// <summary>
        /// The price of the last transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Last { get; set; }
        /// <summary>
        /// The asks on this market
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Asks { get; set; } = new List<CoinExDepthEntry>();
        /// <summary>
        /// The bids on this market
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Bids { get; set; } = new List<CoinExDepthEntry>();
    }

    /// <summary>
    /// Depth info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class CoinExDepthEntry: ISymbolOrderBookEntry
    {
        /// <summary>
        /// The price per unit of the entry
        /// </summary>
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        /// <summary>
        /// The quantity of the entry
        /// </summary>
        [ArrayProperty(1)]
        public decimal Quantity { get; set; }
    }
}
