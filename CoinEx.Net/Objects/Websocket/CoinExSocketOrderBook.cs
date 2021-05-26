using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Websocket
{
    /// <summary>
    /// Order book
    /// </summary>
    public class CoinExSocketOrderBook
    {
        /// <summary>
        /// The price of the last trade. Only filled on a full update.
        /// </summary>
        public decimal? Last { get; set; }

        /// <summary>
        /// Whether it is a full update or an update based on the last update
        /// </summary>
        [JsonOptionalProperty]
        public bool FullUpdate { get; set; }

        /// <summary>
        /// The timestamp of the data. Only filled on a full update.
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(TimestampConverter))]
        public DateTime? Timestamp { get; set; }
        /// <summary>
        /// The asks on the symbol
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Asks { get; set; } = new List<CoinExDepthEntry>();
        /// <summary>
        /// The bids on the symbol
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Bids { get; set; } = new List<CoinExDepthEntry>();
    }
}
