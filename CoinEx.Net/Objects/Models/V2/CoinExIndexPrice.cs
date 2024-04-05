using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Index price
    /// </summary>
    public record CoinExIndexPrice
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Index sources
        /// </summary>
        [JsonPropertyName("sources")]
        public IEnumerable<CoinExIndexPriceSource> Sources { get; set; } = Array.Empty<CoinExIndexPriceSource>();
    }

    /// <summary>
    /// Index price source
    /// </summary>
    public record CoinExIndexPriceSource
    {
        /// <summary>
        /// Exchange
        /// </summary>
        [JsonPropertyName("exchange")]
        public string Exchange { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Weight of the source
        /// </summary>
        [JsonPropertyName("index_weight")]
        public decimal Weight { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [JsonPropertyName("index_price")]
        public decimal? Price { get; set; }
    }
}
