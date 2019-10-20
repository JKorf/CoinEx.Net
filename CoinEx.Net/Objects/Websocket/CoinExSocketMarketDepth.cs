using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Websocket
{
    /// <summary>
    /// Market depth info
    /// </summary>
    public class CoinExSocketMarketDepth
    {
        /// <summary>
        /// The price of the last trade. Only filled on a full update.
        /// </summary>
        public decimal? Last { get; set; }

        /// <summary>
        /// The timestamp of the data. Only filled on a full update.
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(TimestampConverter))]
        public DateTime? Timestamp { get; set; }
        /// <summary>
        /// The asks on the market
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Asks { get; set; } = new List<CoinExDepthEntry>();
        /// <summary>
        /// The bids on the market
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Bids { get; set; } = new List<CoinExDepthEntry>();
    }
}
