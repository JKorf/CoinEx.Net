using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Position levels info
    /// </summary>
    public record CoinExPositionLevels
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Levels
        /// </summary>
        [JsonPropertyName("level")]
        public IEnumerable<CoinExPositionLevel> Levels { get; set; } = Array.Empty<CoinExPositionLevel>();
    }

    /// <summary>
    /// Position level info
    /// </summary>
    public record CoinExPositionLevel
    {
        /// <summary>
        /// Upper limit of the current position
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        /// <summary>
        /// Leverage of current level
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// Current maintenance margin rate
        /// </summary>
        [JsonPropertyName("maintenance_margin_rate")]
        public decimal MaintenanceMarginRate { get; set; }
        /// <summary>
        /// Minimum initial margin rate for the current level
        /// </summary>
        [JsonPropertyName("min_initial_margin_rate")]
        public decimal MinInitialMarginRate { get; set; }
    }
}
