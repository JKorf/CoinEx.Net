using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Margin balance info
    /// </summary>
    public record CoinExMarginBalance
    {
        /// <summary>
        /// Margin account
        /// </summary>
        [JsonPropertyName("margin_account")]
        public string MarginAccount { get; set; } = string.Empty;

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
        /// Current risk rate
        /// </summary>
        [JsonPropertyName("risk_rate")]
        public decimal? RiskRate { get; set; }
        /// <summary>
        /// Current liquidation price
        /// </summary>
        [JsonPropertyName("liq_price")]
        public decimal? LiquidationPrice { get; set; }
        /// <summary>
        /// Available
        /// </summary>
        [JsonPropertyName("available")]
        public CoinExMarginAssetsBalance Available { get; set; } = null!;
        /// <summary>
        /// Frozen
        /// </summary>
        [JsonPropertyName("frozen")]
        public CoinExMarginAssetsBalance Frozen { get; set; } = null!;
        /// <summary>
        /// Repaid
        /// </summary>
        [JsonPropertyName("repaid")]
        public CoinExMarginAssetsBalance Repaid { get; set; } = null!;
        /// <summary>
        /// Interest
        /// </summary>
        [JsonPropertyName("interest")]
        public CoinExMarginAssetsBalance Interest { get; set; } = null!;
    }

    /// <summary>
    /// Assets balance info
    /// </summary>
    public record CoinExMarginAssetsBalance
    {
        /// <summary>
        /// Base asset amount
        /// </summary>
        [JsonPropertyName("base_ccy")]
        public decimal BaseAsset { get; set; }
        /// <summary>
        /// Quote asset amount
        /// </summary>
        [JsonPropertyName("quote_ccy")]
        public decimal QuoteAsset { get; set; }
    }
}
