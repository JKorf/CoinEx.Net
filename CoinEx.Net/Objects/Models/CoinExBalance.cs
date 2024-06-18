using System;
using CoinEx.Net.Converters;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Balance info
    /// </summary>
    public record CoinExBalance
    {
        /// <summary>
        /// The asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// The quantity of the asset that is available
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Available { get; set; }
        /// <summary>
        /// The quantity of the asset not currently available
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Frozen { get; set; }
        /// <summary>
        /// Data timestamp
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Timestamp { get; set; }
    }
}
