using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Historic premium index
    /// </summary>
    [SerializationModel]
    public record CoinExPremiumIndexHistory
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>created_at</c>"] Timestamp
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime? Timestamp { get; set; }
        /// <summary>
        /// 	["<c>premium_index</c>"] Premium index. The main basis for calculating the funding rate
        /// </summary>
        [JsonPropertyName("premium_index")]
        public decimal PremiumIndex { get; set; }
    }
}
