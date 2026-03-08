using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Automated Market Maker liquidity info
    /// </summary>
    [SerializationModel]
    public record CoinExAmmBalance
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
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
        /// ["<c>base_ccy_amount</c>"] Base asset amount
        /// </summary>
        [JsonPropertyName("base_ccy_amount")]
        public decimal BaseAssetQuantity { get; set; }
        /// <summary>
        /// ["<c>quote_ccy_amount</c>"] Quote asset amount
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
