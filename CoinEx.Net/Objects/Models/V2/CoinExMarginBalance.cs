using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Margin balance info
    /// </summary>
    [SerializationModel]
    public record CoinExMarginBalance
    {
        /// <summary>
        /// ["<c>margin_account</c>"] Margin account
        /// </summary>
        [JsonPropertyName("margin_account")]
        public string MarginAccount { get; set; } = string.Empty;

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
        /// ["<c>risk_rate</c>"] Current risk rate
        /// </summary>
        [JsonPropertyName("risk_rate")]
        public decimal? RiskRate { get; set; }
        /// <summary>
        /// ["<c>liq_price</c>"] Current liquidation price
        /// </summary>
        [JsonPropertyName("liq_price")]
        public decimal? LiquidationPrice { get; set; }
        /// <summary>
        /// ["<c>available</c>"] Available
        /// </summary>
        [JsonPropertyName("available")]
        public CoinExMarginAssetsBalance Available { get; set; } = null!;
        /// <summary>
        /// ["<c>frozen</c>"] Frozen
        /// </summary>
        [JsonPropertyName("frozen")]
        public CoinExMarginAssetsBalance Frozen { get; set; } = null!;
        /// <summary>
        /// ["<c>repaid</c>"] Repaid
        /// </summary>
        [JsonPropertyName("repaid")]
        public CoinExMarginAssetsBalance Repaid { get; set; } = null!;
        /// <summary>
        /// ["<c>interest</c>"] Interest
        /// </summary>
        [JsonPropertyName("interest")]
        public CoinExMarginAssetsBalance Interest { get; set; } = null!;
    }

    /// <summary>
    /// Assets balance info
    /// </summary>
    [SerializationModel]
    public record CoinExMarginAssetsBalance
    {
        /// <summary>
        /// ["<c>base_ccy</c>"] Base asset amount
        /// </summary>
        [JsonPropertyName("base_ccy")]
        public decimal BaseAsset { get; set; }
        /// <summary>
        /// ["<c>quote_ccy</c>"] Quote asset amount
        /// </summary>
        [JsonPropertyName("quote_ccy")]
        public decimal QuoteAsset { get; set; }
    }
}
