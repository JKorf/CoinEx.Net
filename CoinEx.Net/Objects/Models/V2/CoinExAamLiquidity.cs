using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Automated market maker liquidity info
    /// </summary>
    public record CoinExAamLiquidity
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Base asset amount in AMM account
        /// </summary>
        [JsonPropertyName("base_ccy_amount")]
        public decimal BaseAssetQuantity { get; set; }
        /// <summary>
        /// Quote asset amount in AMM account
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
