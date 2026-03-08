using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Position levels info
    /// </summary>
    [SerializationModel]
    public record CoinExPositionLevels
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>level</c>"] Levels
        /// </summary>
        [JsonPropertyName("level")]
        public CoinExPositionLevel[] Levels { get; set; } = Array.Empty<CoinExPositionLevel>();
    }

    /// <summary>
    /// Position level info
    /// </summary>
    [SerializationModel]
    public record CoinExPositionLevel
    {
        /// <summary>
        /// ["<c>amount</c>"] Upper limit of the current position
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        /// <summary>
        /// ["<c>leverage</c>"] Leverage of current level
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// ["<c>maintenance_margin_rate</c>"] Current maintenance margin rate
        /// </summary>
        [JsonPropertyName("maintenance_margin_rate")]
        public decimal MaintenanceMarginRate { get; set; }
        /// <summary>
        /// ["<c>min_initial_margin_rate</c>"] Minimum initial margin rate for the current level
        /// </summary>
        [JsonPropertyName("min_initial_margin_rate")]
        public decimal MinInitialMarginRate { get; set; }
    }
}
