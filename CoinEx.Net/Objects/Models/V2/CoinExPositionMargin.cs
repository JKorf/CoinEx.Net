using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Position margin info
    /// </summary>
    [SerializationModel]
    public record CoinExPositionMargin
    {
        /// <summary>
        /// ["<c>position_id</c>"] Position id
        /// </summary>
        [JsonPropertyName("position_id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>market_type</c>"] Account type
        /// </summary>
        [JsonPropertyName("market_type")]
        public AccountType AccountType { get; set; }
        /// <summary>
        /// ["<c>margin_mode</c>"] Margin mode
        /// </summary>
        [JsonPropertyName("margin_mode")]
        public MarginMode MarginMode { get; set; }
        /// <summary>
        /// ["<c>leverage</c>"] Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// ["<c>liq_price</c>"] Liquidation price
        /// </summary>
        [JsonPropertyName("liq_price")]
        public decimal LiquidationPrice { get; set; }
        /// <summary>
        /// ["<c>bkr_price</c>"] Bankruptcy price
        /// </summary>
        [JsonPropertyName("bkr_price")]
        public decimal BankruptcyPrice { get; set; }
        /// <summary>
        /// ["<c>settle_price</c>"] Settlement price
        /// </summary>
        [JsonPropertyName("settle_price")]
        public decimal SettlePrice { get; set; }
        /// <summary>
        /// ["<c>open_interest</c>"] Open interest
        /// </summary>
        [JsonPropertyName("open_interest")]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// ["<c>margin_avbl</c>"] Margin available
        /// </summary>
        [JsonPropertyName("margin_avbl")]
        public decimal MarginAvailable { get; set; }
        /// <summary>
        /// ["<c>margin_change</c>"] Adjusted margin amount
        /// </summary>
        [JsonPropertyName("margin_change")]
        public decimal MarginChange { get; set; }
        /// <summary>
        /// ["<c>created_at</c>"] Timestamp created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
    }
}
