using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models.Socket
{
    /// <summary>
    /// Order book
    /// </summary>
    public record CoinExSocketOrderBook
    {
        /// <summary>
        /// The price of the last trade. Only filled on a full update.
        /// </summary>
        [JsonProperty("last")]
        public decimal? LastPrice { get; set; }

        /// <summary>
        /// The timestamp of the data. Only filled on a full update.
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Timestamp { get; set; }
        /// <summary>
        /// The asks on the symbol
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Asks { get; set; } = Array.Empty<CoinExDepthEntry>();
        /// <summary>
        /// The bids on the symbol
        /// </summary>
        public IEnumerable<CoinExDepthEntry> Bids { get; set; } = Array.Empty<CoinExDepthEntry>();
        /// <summary>
        /// Signed integer (32 bit) of full depth data checksum
        /// </summary>
        public long? Checksum { get; set; }
    }
}
