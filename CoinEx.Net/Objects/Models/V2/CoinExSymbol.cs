using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Symbol info
    /// </summary>
    public record CoinExSymbol
    {
        /// <summary>
        /// Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Maker fee rate
        /// </summary>
        [JsonPropertyName("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// Taker fee rate
        /// </summary>
        [JsonPropertyName("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// Minimal order quantiy
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinOrderQuantity { get; set; }
        /// <summary>
        /// Base asset
        /// </summary>
        [JsonPropertyName("base_ccy")]
        public string BaseAsset { get; set; } = string.Empty;
        /// <summary>
        /// Quote asset
        /// </summary>
        [JsonPropertyName("quote_ccy")]
        public string QuoteAsset { get; set; } = string.Empty;
        /// <summary>
        /// Quantity precision
        /// </summary>
        [JsonPropertyName("base_ccy_precision")]
        public int QuantityPrecision { get; set; }
        /// <summary>
        /// Price precision
        /// </summary>
        [JsonPropertyName("quote_ccy_precision")]
        public int PricePrecision { get; set; }
        /// <summary>
        /// Is Automated Market Maker available
        /// </summary>
        [JsonPropertyName("is_amm_available")]
        public bool AutoMarketMakerAvailable { get; set; }
        /// <summary>
        /// Is Margin Trading available
        /// </summary>
        [JsonPropertyName("is_margin_available")]
        public bool MarginTradingAvailable { get; set; }
    }
}
