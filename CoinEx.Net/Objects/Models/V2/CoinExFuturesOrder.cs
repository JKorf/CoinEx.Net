using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Order info
    /// </summary>
    public record CoinExFuturesOrder
    {
        /// <summary>
        /// Order id
        /// </summary>
        [JsonPropertyName("order_id")]
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
        /// Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// Order type
        /// </summary>
        [JsonPropertyName("type")]
        public OrderTypeV2 OrderType { get; set; }
        /// <summary>
        /// Order quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Order price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        /// <summary>
        /// Quantity remaining
        /// </summary>
        [JsonPropertyName("unfilled_amount")]
        public decimal QuantityRemaining { get; set; }
        /// <summary>
        /// Quantity filled
        /// </summary>
        [JsonPropertyName("filled_amount")]
        public decimal? QuantityFilled { get; set; }
        /// <summary>
        /// Value of the filled part
        /// </summary>
        [JsonPropertyName("filled_value")]
        public decimal? ValueFilled { get; set; }
        /// <summary>
        /// Client order id
        /// </summary>
        [JsonPropertyName("client_id")]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// Fee
        /// </summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// Fee asset
        /// </summary>
        [JsonPropertyName("fee_ccy")]
        public string FeeAsset { get; set; } = string.Empty;
        /// <summary>
        /// Maker fee rate
        /// </summary>
        [JsonPropertyName("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// Taker fee rate
        /// </summary>
        [JsonPropertyName("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// Filled amount of the last trade
        /// </summary>
        [JsonPropertyName("last_filled_amount")]
        public decimal? LastFilledQuantity { get; set; }
        /// <summary>
        /// Price of the last trade
        /// </summary>
        [JsonPropertyName("last_filled_price")]
        public decimal? LastFilledPrice { get; set; }
        /// <summary>
        /// Realized profit and loss
        /// </summary>
        [JsonPropertyName("realized_pnl")]
        public decimal? RealizedPnl { get; set; }
        /// <summary>
        /// Timestamp order was created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Timestamp order was last updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// Status of the order
        /// </summary>
        [JsonPropertyName("status")]
        public OrderStatusV2? Status { get; set; }
    }
}
