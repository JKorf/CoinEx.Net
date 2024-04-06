using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Automated Market Maker liquidity info
    /// </summary>
    public record CoinExAmmBalance
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
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
        /// Base asset amount
        /// </summary>
        [JsonPropertyName("base_ccy_amount")]
        public decimal BaseAssetQuantity { get; set; }
        /// <summary>
        /// Quote asset amount
        /// </summary>
        [JsonPropertyName("quote_ccy_amount")]
        public decimal QuoteAssetQuantity { get; set; }
        /// <summary>
        /// Liquidity percentage in AMM account
        /// </summary>
        [JsonPropertyName("liquidity_proportion")]
        public decimal LiquidityProportion { get; set; }
    }
}
