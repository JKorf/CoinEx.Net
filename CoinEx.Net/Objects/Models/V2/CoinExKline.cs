using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Kline/candlestick info
    /// </summary>
    public record CoinExKline
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Open time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime OpenTime { get; set; }
        /// <summary>
        /// Open price
        /// </summary>
        [JsonPropertyName("open")]
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// Close price
        /// </summary>
        [JsonPropertyName("close")]
        public decimal ClosePrice { get; set; }
        /// <summary>
        /// High price
        /// </summary>
        [JsonPropertyName("high")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// Low price
        /// </summary>
        [JsonPropertyName("low")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// Volume
        /// </summary>
        [JsonPropertyName("volume")]
        public decimal Volume { get; set; }
        /// <summary>
        /// Value (Quote asset volume)
        /// </summary>
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
    }
}
