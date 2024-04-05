using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Basis rate
    /// </summary>
    public record CoinExBasis
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Create time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Basis rate
        /// </summary>
        [JsonPropertyName("basis_rate")]
        public decimal BasisRate { get; set; }
    }
}
