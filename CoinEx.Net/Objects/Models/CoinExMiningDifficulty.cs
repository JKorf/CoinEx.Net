using System;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Mining difficulty info
    /// </summary>
    public record CoinExMiningDifficulty
    {
        /// <summary>
        /// The difficulty in CET/Hour
        /// </summary>
        public string Difficulty { get; set; } = string.Empty;
        /// <summary>
        /// Estimated hourly mining yield to distribute
        /// </summary>
        public string Prediction { get; set; } = string.Empty;
        /// <summary>
        /// The update time of the Prediction field
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
