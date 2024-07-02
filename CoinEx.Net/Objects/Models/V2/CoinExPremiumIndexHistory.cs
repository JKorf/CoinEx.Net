using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Historic premium index
    /// </summary>
    public record CoinExPremiumIndexHistory
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
        public DateTime? Timestamp { get; set; }
        /// <summary>
        /// 	Premium index. The main basis for calculating the funding rate
        /// </summary>
        [JsonPropertyName("premium_index")]
        public decimal PremiumIndex { get; set; }
    }
}
