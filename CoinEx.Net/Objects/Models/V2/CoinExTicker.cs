using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Ticker (24h price stats) info
    /// </summary>
    public record CoinExTicker
    {
        /// <summary>
        /// Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Last price
        /// </summary>
        [JsonPropertyName("last")]
        public decimal LastPrice { get; set; }
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
        /// Volume in base asset
        /// </summary>
        [JsonPropertyName("volume")]
        public decimal Volume { get; set; }
        /// <summary>
        /// Volume in quote asset
        /// </summary>
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
        /// <summary>
        /// Sell volume
        /// </summary>
        [JsonPropertyName("volume_sell")]
        public decimal SellVolume { get; set; }
        /// <summary>
        /// Buy volume
        /// </summary>
        [JsonPropertyName("volume_buy")]
        public decimal BuyVolume { get; set; }
        ///// <summary>
        ///// Period
        ///// </summary>
        //[JsonPropertyName("period")]
        //public KlineInterval Period { get; set; }
    }
}
