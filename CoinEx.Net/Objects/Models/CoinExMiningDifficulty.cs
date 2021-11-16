using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Mining difficulty info
    /// </summary>
    public class CoinExMiningDifficulty
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
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
