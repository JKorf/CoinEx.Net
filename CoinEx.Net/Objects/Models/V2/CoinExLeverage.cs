using CoinEx.Net.Enums;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Leverage info
    /// </summary>
    public record CoinExLeverage
    {
        /// <summary>
        /// Margin mode
        /// </summary>
        [JsonPropertyName("margin_mode")]
        public MarginMode MarginMode { get; set; }
        /// <summary>
        /// Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
    }
}
