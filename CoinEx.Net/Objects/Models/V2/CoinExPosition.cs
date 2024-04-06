using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Position info
    /// </summary>
    public record CoinExPosition
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
        public AccountType? AccountType { get; set; }
        /// <summary>
        /// Position side
        /// </summary>
        [JsonPropertyName("side")]
        public PositionSide Side { get; set; }
        /// <summary>
        /// Margin mode
        /// </summary>
        [JsonPropertyName("margin_mode")]
        public MarginMode MarginMode { get; set; }
        /// <summary>
        /// Open interest
        /// </summary>
        [JsonPropertyName("open_interest")]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// position size available for closing
        /// </summary>
        [JsonPropertyName("close_avbl")]
        public decimal CloseAvailable { get; set; }
        /// <summary>
        /// All time high position quantity
        /// </summary>
        [JsonPropertyName("ath_position_amount")]
        public decimal AthPositionQuantity { get; set; }
        /// <summary>
        /// Unrealized profit and loss
        /// </summary>
        [JsonPropertyName("unrealized_pnl")]
        public decimal UnrealizedPnl { get; set; }
        /// <summary>
        /// Realized profit and loss
        /// </summary>
        [JsonPropertyName("realized_pnl")]
        public decimal RealizedPnl { get; set; }
        /// <summary>
        /// Average entry price
        /// </summary>
        [JsonPropertyName("avg_entry_price")]
        public decimal AverageEntryPrice { get; set; }
        /// <summary>
        /// Cumulative position value
        /// </summary>
        [JsonPropertyName("cml_position_value")]
        public decimal PositionValue { get; set; }
        /// <summary>
        /// Max position value
        /// </summary>
        [JsonPropertyName("max_position_value")]
        public decimal MaxPositionValue { get; set; }
        /// <summary>
        /// Take profit price
        /// </summary>
        [JsonPropertyName("take_profit_price")]
        public decimal? TakeProfitPrice { get; set; }
        /// <summary>
        /// Stop loss price
        /// </summary>
        [JsonPropertyName("stop_loss_price")]
        public decimal? StopLossPrice { get; set; }
        /// <summary>
        /// Take profit price type
        /// </summary>
        [JsonPropertyName("take_profit_type")]
        public PriceType? TakeProfitType { get; set; }
        /// <summary>
        /// Stop loss price type
        /// </summary>
        [JsonPropertyName("stop_loss_type")]
        public PriceType? StopLossType { get; set; }
        /// <summary>
        /// Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// Margin available
        /// </summary>
        [JsonPropertyName("margin_avbl")]
        public decimal MarginAvailable { get; set; }
        /// <summary>
        /// All time high margin size
        /// </summary>
        [JsonPropertyName("ath_margin_size")]
        public decimal AthMarginSize { get; set; }
        /// <summary>
        /// Position margin rate
        /// </summary>
        [JsonPropertyName("position_margin_rate")]
        public decimal PositionMarginRate { get; set; }
        /// <summary>
        /// Maintenance margin rate
        /// </summary>
        [JsonPropertyName("maintenance_margin_rate")]
        public decimal MaintenanceMarginRate { get; set; }
        /// <summary>
        /// Maintenance margin value
        /// </summary>
        [JsonPropertyName("maintenance_margin_value")]
        public decimal MaintenanceMarginValue { get; set; }
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
        /// Auto deleveraging level
        /// </summary>
        [JsonPropertyName("adl_level")]
        public int AdlLevel { get; set; }
        /// <summary>
        /// Settlement price
        /// </summary>
        [JsonPropertyName("settle_price")]
        public decimal SettlePrice { get; set; }
        /// <summary>
        /// Settlement value
        /// </summary>
        [JsonPropertyName("settle_value")]
        public decimal SettleValue { get; set; }
        /// <summary>
        /// Timestamp created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Timestamp last updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdateTime { get; set; }
    }
}
