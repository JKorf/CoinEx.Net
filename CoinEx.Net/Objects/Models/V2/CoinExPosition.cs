using CryptoExchange.Net.Converters.SystemTextJson;
using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Position info
    /// </summary>
    [SerializationModel]
    public record CoinExPosition
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
        public AccountType? AccountType { get; set; }
        /// <summary>
        /// ["<c>side</c>"] Position side
        /// </summary>
        [JsonPropertyName("side")]
        public PositionSide Side { get; set; }
        /// <summary>
        /// ["<c>margin_mode</c>"] Margin mode
        /// </summary>
        [JsonPropertyName("margin_mode")]
        public MarginMode MarginMode { get; set; }
        /// <summary>
        /// ["<c>open_interest</c>"] Open interest
        /// </summary>
        [JsonPropertyName("open_interest")]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// ["<c>close_avbl</c>"] position size available for closing
        /// </summary>
        [JsonPropertyName("close_avbl")]
        public decimal CloseAvailable { get; set; }
        /// <summary>
        /// ["<c>ath_position_amount</c>"] All time high position quantity
        /// </summary>
        [JsonPropertyName("ath_position_amount")]
        public decimal AthPositionQuantity { get; set; }
        /// <summary>
        /// ["<c>unrealized_pnl</c>"] Unrealized profit and loss
        /// </summary>
        [JsonPropertyName("unrealized_pnl")]
        public decimal UnrealizedPnl { get; set; }
        /// <summary>
        /// ["<c>realized_pnl</c>"] Realized profit and loss
        /// </summary>
        [JsonPropertyName("realized_pnl")]
        public decimal RealizedPnl { get; set; }
        /// <summary>
        /// ["<c>avg_entry_price</c>"] Average entry price
        /// </summary>
        [JsonPropertyName("avg_entry_price")]
        public decimal AverageEntryPrice { get; set; }
        /// <summary>
        /// ["<c>cml_position_value</c>"] Cumulative position value
        /// </summary>
        [JsonPropertyName("cml_position_value")]
        public decimal PositionValue { get; set; }
        /// <summary>
        /// ["<c>max_position_value</c>"] Max position value
        /// </summary>
        [JsonPropertyName("max_position_value")]
        public decimal MaxPositionValue { get; set; }
        /// <summary>
        /// ["<c>take_profit_price</c>"] Take profit price
        /// </summary>
        [JsonPropertyName("take_profit_price")]
        public decimal? TakeProfitPrice { get; set; }
        /// <summary>
        /// ["<c>stop_loss_price</c>"] Stop loss price
        /// </summary>
        [JsonPropertyName("stop_loss_price")]
        public decimal? StopLossPrice { get; set; }
        /// <summary>
        /// ["<c>take_profit_type</c>"] Take profit price type
        /// </summary>
        [JsonPropertyName("take_profit_type")]
        public PriceType? TakeProfitType { get; set; }
        /// <summary>
        /// ["<c>stop_loss_type</c>"] Stop loss price type
        /// </summary>
        [JsonPropertyName("stop_loss_type")]
        public PriceType? StopLossType { get; set; }
        /// <summary>
        /// ["<c>leverage</c>"] Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// ["<c>margin_avbl</c>"] Margin available
        /// </summary>
        [JsonPropertyName("margin_avbl")]
        public decimal MarginAvailable { get; set; }
        /// <summary>
        /// ["<c>ath_margin_size</c>"] All time high margin size
        /// </summary>
        [JsonPropertyName("ath_margin_size")]
        public decimal AthMarginSize { get; set; }
        /// <summary>
        /// ["<c>position_margin_rate</c>"] Position margin rate
        /// </summary>
        [JsonPropertyName("position_margin_rate")]
        public decimal PositionMarginRate { get; set; }
        /// <summary>
        /// ["<c>maintenance_margin_rate</c>"] Maintenance margin rate
        /// </summary>
        [JsonPropertyName("maintenance_margin_rate")]
        public decimal MaintenanceMarginRate { get; set; }
        /// <summary>
        /// ["<c>maintenance_margin_value</c>"] Maintenance margin value
        /// </summary>
        [JsonPropertyName("maintenance_margin_value")]
        public decimal MaintenanceMarginValue { get; set; }
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
        /// ["<c>adl_level</c>"] Auto deleveraging level
        /// </summary>
        [JsonPropertyName("adl_level")]
        public int AdlLevel { get; set; }
        /// <summary>
        /// ["<c>settle_price</c>"] Settlement price
        /// </summary>
        [JsonPropertyName("settle_price")]
        public decimal SettlePrice { get; set; }
        /// <summary>
        /// ["<c>settle_value</c>"] Settlement value
        /// </summary>
        [JsonPropertyName("settle_value")]
        public decimal SettleValue { get; set; }
        [JsonInclude, JsonPropertyName("settle_val")]
        internal decimal SettleValueIn { get => SettleValue; set => SettleValue = value; }
        /// <summary>
        /// ["<c>created_at</c>"] Timestamp created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>updated_at</c>"] Timestamp last updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// ["<c>stop_loss_list</c>"] Stop loss orders
        /// </summary>
        [JsonPropertyName("stop_loss_list")]
        public CoinExStopOrderItem[] StopLosses { get; set; } = [];
        /// <summary>
        /// ["<c>take_profit_list</c>"] Take profit orders
        /// </summary>
        [JsonPropertyName("take_profit_list")]
        public CoinExStopOrderItem[] TakeProfits { get; set; } = [];
    }

    /// <summary>
    /// Take profit / Stop loss order
    /// </summary>
    public record CoinExStopOrderItem
    {
        /// <summary>
        /// ["<c>id</c>"] Id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// ["<c>type</c>"] Price type
        /// </summary>
        [JsonPropertyName("type")]
        public PriceType PriceType { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal? Quantity { get; set; }
        /// <summary>
        /// ["<c>is_all</c>"] Full position or partial
        /// </summary>
        [JsonPropertyName("is_all")]
        public bool IsAll { get; set; }
        /// <summary>
        /// ["<c>created_at</c>"] Create time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
    }
}
