using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Automated market maker liquidity info
    /// </summary>
    [SerializationModel]
    public record CoinExAamLiquidity
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>base_ccy_amount</c>"] Base asset amount in AMM account
        /// </summary>
        [JsonPropertyName("base_ccy_amount")]
        public decimal BaseAssetQuantity { get; set; }
        /// <summary>
        /// ["<c>quote_ccy_amount</c>"] Quote asset amount in AMM account
        /// </summary>
        [JsonPropertyName("quote_ccy_amount")]
        public decimal QuoteAssetQuantity { get; set; }
        /// <summary>
        /// ["<c>liquidity_proportion</c>"] Liquidity percentage in AMM account
        /// </summary>
        [JsonPropertyName("liquidity_proportion")]
        public decimal LiquidityProportion { get; set; }
    }
}
