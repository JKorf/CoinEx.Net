using System;
using System.Collections.Generic;
using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Order book
    /// </summary>
    public record CoinExOrderBook
    {
        /// <summary>
        /// The price of the last transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("last")]
        public decimal LastPrice { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The asks on this symbol
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Asks { get; set; } = Array.Empty<CoinExDepthEntry>();
        /// <summary>
        /// The bids on this symbol
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Bids { get; set; } = Array.Empty<CoinExDepthEntry>();
    }

    /// <summary>
    /// Depth info
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public record CoinExDepthEntry: ISymbolOrderBookEntry
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
