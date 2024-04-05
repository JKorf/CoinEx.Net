using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Stop order info
    /// </summary>
    public record CoinExStopOrder
    {
        /// <summary>
        /// Order id
        /// </summary>
        [JsonPropertyName("stop_id")]
        public long StopOrderId { get; set; }
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
        /// Asset the quantity is in
        /// </summary>
        [JsonPropertyName("ccy")]
        public string? QuantityAsset { get; set; }
        /// <summary>
        /// Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// Order type
        /// </summary>
        [JsonPropertyName("type")]
        public OrderTypeV2 Type { get; set; }
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
        /// Client order id
        /// </summary>
        [JsonPropertyName("client_id")]
        public string? ClientOrderId { get; set; }
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
        /// Trigger price
        /// </summary>
        [JsonPropertyName("trigger_price")]
        public decimal TriggerPrice { get; set; }
        /// <summary>
        /// Trigger direction
        /// </summary>
        [JsonPropertyName("trigger_direction")]
        public TriggerDirection TriggerDirection { get; set; }
        /// <summary>
        /// Trigger price type
        /// </summary>
        [JsonPropertyName("trigger_price_type")]
        public PriceType TriggerPriceType { get; set; }
        /// <summary>
        /// Taker fee rate
        /// </summary>
        [JsonPropertyName("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// Maker fee rate
        /// </summary>
        [JsonPropertyName("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
    }
}
