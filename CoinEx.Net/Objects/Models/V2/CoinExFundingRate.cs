using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Funding rate info
    /// </summary>
    public record CoinExFundingRate
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Mark price
        /// </summary>
        [JsonPropertyName("mark_price")]
        public decimal MarkPrice { get; set; }
        /// <summary>
        /// Last funding rate
        /// </summary>
        [JsonPropertyName("latest_funding_rate")]
        public decimal LastFundingRate { get; set; }
        /// <summary>
        /// Next funding rate
        /// </summary>
        [JsonPropertyName("next_funding_rate")]
        public decimal NextFundingRate { get; set; }
        /// <summary>
        /// Last funding time
        /// </summary>
        [JsonPropertyName("latest_funding_time")]
        public DateTime? LastFundingTime { get; set; }
        /// <summary>
        /// Next funding time
        /// </summary>
        [JsonPropertyName("next_funding_time")]
        public DateTime? NextFundingTime { get; set; }
    }
}
