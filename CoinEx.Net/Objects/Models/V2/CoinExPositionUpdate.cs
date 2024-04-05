using CoinEx.Net.Enums;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Position update
    /// </summary>
    public record CoinExPositionUpdate
    {
        /// <summary>
        /// Event that triggered the update
        /// </summary>
        [JsonPropertyName("event")]
        public PositionUpdateType Event { get; set; }
        /// <summary>
        /// Position data
        /// </summary>
        [JsonPropertyName("position")]
        public CoinExStreamPosition Position { get; set; } = null!;
    }

    /// <summary>
    /// Position info
    /// </summary>
    public record CoinExStreamPosition : CoinExPosition
    {
        /// <summary>
        /// First filled price
        /// </summary>
        [JsonPropertyName("first_filled_price")]
        public decimal FirstFilledPrice { get; set; }
        /// <summary>
        /// Last filled price
        /// </summary>
        [JsonPropertyName("latest_filled_price")]
        public decimal LastFilledPrice { get; set; }
    }
}
