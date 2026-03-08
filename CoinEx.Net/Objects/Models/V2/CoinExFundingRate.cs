using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Funding rate info
    /// </summary>
    [SerializationModel]
    public record CoinExFundingRate
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>mark_price</c>"] Mark price
        /// </summary>
        [JsonPropertyName("mark_price")]
        public decimal MarkPrice { get; set; }
        /// <summary>
        /// ["<c>latest_funding_rate</c>"] Last funding rate
        /// </summary>
        [JsonPropertyName("latest_funding_rate")]
        public decimal LastFundingRate { get; set; }
        /// <summary>
        /// ["<c>next_funding_rate</c>"] Next funding rate
        /// </summary>
        [JsonPropertyName("next_funding_rate")]
        public decimal NextFundingRate { get; set; }
        /// <summary>
        /// ["<c>max_funding_rate</c>"] Max funding rate
        /// </summary>
        [JsonPropertyName("max_funding_rate")]
        public decimal? MaxFundingRate { get; set; }
        /// <summary>
        /// ["<c>min_funding_rate</c>"] Min funding rate
        /// </summary>
        [JsonPropertyName("min_funding_rate")]
        public decimal? MinFundingRate { get; set; }
        /// <summary>
        /// ["<c>latest_funding_time</c>"] Last funding time
        /// </summary>
        [JsonPropertyName("latest_funding_time")]
        public DateTime? LastFundingTime { get; set; }
        /// <summary>
        /// ["<c>next_funding_time</c>"] Next funding time
        /// </summary>
        [JsonPropertyName("next_funding_time")]
        public DateTime? NextFundingTime { get; set; }
    }
}
