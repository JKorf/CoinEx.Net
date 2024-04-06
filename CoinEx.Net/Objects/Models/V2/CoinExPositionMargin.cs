using System;
using System.Text.Json.Serialization;
using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Position margin info
    /// </summary>
    public record CoinExPositionMargin
    {
        /// <summary>
        /// Position id
        /// </summary>
        [JsonPropertyName("position_id")]
        public long Id { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Account type
        /// </summary>
        [JsonPropertyName("market_type")]
        public AccountType AccountType { get; set; }
        /// <summary>
        /// Margin mode
        /// </summary>
        [JsonPropertyName("margin_mode")]
        public MarginMode MarginMode { get; set; }
        /// <summary>
        /// Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// Liquidation price
        /// </summary>
        [JsonPropertyName("liq_price")]
        public decimal LiquidationPrice { get; set; }
        /// <summary>
        /// Bankruptcy price
        /// </summary>
        [JsonPropertyName("bkr_price")]
        public decimal BankruptcyPrice { get; set; }
        /// <summary>
        /// Settlement price
        /// </summary>
        [JsonPropertyName("settle_price")]
        public decimal SettlePrice { get; set; }
        /// <summary>
        /// Open interest
        /// </summary>
        [JsonPropertyName("open_interest")]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// Margin available
        /// </summary>
        [JsonPropertyName("margin_avbl")]
        public decimal MarginAvailable { get; set; }
        /// <summary>
        /// Adjusted margin amount
        /// </summary>
        [JsonPropertyName("margin_change")]
        public decimal MarginChange { get; set; }
        /// <summary>
        /// Timestamp created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
    }
}
