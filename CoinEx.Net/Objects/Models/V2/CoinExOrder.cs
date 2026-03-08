using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Order info
    /// </summary>
    [SerializationModel]
    public record CoinExOrder
    {
        /// <summary>
        /// ["<c>order_id</c>"] Order id
        /// </summary>
        [JsonPropertyName("order_id")]
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
        /// ["<c>ccy</c>"] Asset the quantity is in
        /// </summary>
        [JsonPropertyName("ccy")]
        public string QuantityAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>side</c>"] Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// ["<c>type</c>"] Order type
        /// </summary>
        [JsonPropertyName("type")]
        public OrderTypeV2 OrderType { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Order quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Order price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        /// <summary>
        /// ["<c>unfilled_amount</c>"] Quantity remaining
        /// </summary>
        [JsonPropertyName("unfilled_amount")]
        public decimal QuantityRemaining { get; set; }
        /// <summary>
        /// ["<c>filled_amount</c>"] Quantity filled
        /// </summary>
        [JsonPropertyName("filled_amount")]
        public decimal QuantityFilled { get; set; }
        /// <summary>
        /// ["<c>filled_value</c>"] Value of the filled part
        /// </summary>
        [JsonPropertyName("filled_value")]
        public decimal ValueFilled { get; set; }
        /// <summary>
        /// ["<c>client_id</c>"] Client order id
        /// </summary>
        [JsonPropertyName("client_id")]
        [JsonConverter(typeof(ClientIdConverter))]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// ["<c>base_fee</c>"] Fee in base asset
        /// </summary>
        [JsonPropertyName("base_fee")]
        public decimal FeeBaseAsset { get; set; }
        /// <summary>
        /// ["<c>quote_fee</c>"] Fee in quote asset
        /// </summary>
        [JsonPropertyName("quote_fee")]
        public decimal FeeQuoteAsset { get; set; }
        /// <summary>
        /// ["<c>discount_fee</c>"] Fee discount
        /// </summary>
        [JsonPropertyName("discount_fee")]
        public decimal FeeDiscount { get; set; }
        /// <summary>
        /// ["<c>maker_fee_rate</c>"] Maker fee rate
        /// </summary>
        [JsonPropertyName("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// ["<c>taker_fee_rate</c>"] Taker fee rate
        /// </summary>
        [JsonPropertyName("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// ["<c>last_filled_amount</c>"] Filled amount of the last trade
        /// </summary>
        [JsonPropertyName("last_filled_amount")]
        public decimal? LastFilledQuantity { get; set; }
        /// <summary>
        /// ["<c>last_filled_price</c>"] Price of the last trade
        /// </summary>
        [JsonPropertyName("last_filled_price")]
        public decimal? LastFilledPrice { get; set; }
        /// <summary>
        /// ["<c>created_at</c>"] Timestamp order was created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>updated_at</c>"] Timestamp order was last updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// ["<c>status</c>"] Status of the order
        /// </summary>
        [JsonPropertyName("status")]
        public OrderStatusV2? Status { get; set; }
    }
}
