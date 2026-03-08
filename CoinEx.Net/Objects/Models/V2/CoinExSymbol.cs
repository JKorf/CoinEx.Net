using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Symbol info
    /// </summary>
    [SerializationModel]
    public record CoinExSymbol
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>maker_fee_rate</c>"] Maker fee rate
        /// </summary>
        [JsonPropertyName("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// ["<c>taker_fee_rate</c>"] Taker fee rate
        /// </summary>
        [JsonPropertyName("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// ["<c>min_amount</c>"] Minimal order quantiy
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinOrderQuantity { get; set; }
        /// <summary>
        /// ["<c>base_ccy</c>"] Base asset
        /// </summary>
        [JsonPropertyName("base_ccy")]
        public string BaseAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>quote_ccy</c>"] Quote asset
        /// </summary>
        [JsonPropertyName("quote_ccy")]
        public string QuoteAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>base_ccy_precision</c>"] Quantity precision
        /// </summary>
        [JsonPropertyName("base_ccy_precision")]
        public int QuantityPrecision { get; set; }
        /// <summary>
        /// ["<c>quote_ccy_precision</c>"] Price precision
        /// </summary>
        [JsonPropertyName("quote_ccy_precision")]
        public int PricePrecision { get; set; }
        /// <summary>
        /// ["<c>is_amm_available</c>"] Is Automated Market Maker available
        /// </summary>
        [JsonPropertyName("is_amm_available")]
        public bool AutoMarketMakerAvailable { get; set; }
        /// <summary>
        /// ["<c>is_margin_available</c>"] Is Margin Trading available
        /// </summary>
        [JsonPropertyName("is_margin_available")]
        public bool MarginTradingAvailable { get; set; }
    }
}
