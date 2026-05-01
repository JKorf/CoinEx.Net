using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Premium update information
    /// </summary>
    [SerializationModel]
    public record CoinExPremiumUpdate
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>updated_at</c>"] Update time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// ["<c>premium_index</c>"] Premium index price
        /// </summary>
        [JsonPropertyName("premium_index")]
        public decimal PremiumIndex { get; set; }
    }
}
