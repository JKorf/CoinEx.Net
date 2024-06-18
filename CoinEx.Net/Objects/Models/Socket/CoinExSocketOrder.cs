using System;
using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models.Socket
{
    /// <summary>
    /// Order info
    /// </summary>
    public record CoinExSocketOrder
    {
        /// <summary>
        /// The total quantity of the oder
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The time the order was created
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("ctime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// The fee of the order
        /// </summary>
        [JsonProperty("asset_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Fee { get; set; }
        /// <summary>
        /// The fee discount
        /// </summary>
        [JsonProperty("fee_discount")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal FeeDiscount { get; set; }
        /// <summary>
        /// The asset the fee is on
        /// </summary>
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; } = string.Empty;
        /// <summary>
        /// The executed quantity transaction fee
        /// </summary>
        [JsonProperty("deal_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal TransactionFee { get; set; }
        /// <summary>
        /// The executed value in this update
        /// </summary>
        [JsonProperty("deal_money")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal QuoteQuantityFilled { get; set; }
        /// <summary>
        /// The executed quantity in this update
        /// </summary>
        [JsonProperty("deal_stock")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal QuantityFilled { get; set; }
        /// <summary>
        /// The order id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Quantity of order left to execute
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("left")]
        public decimal QuantityRemaining { get; set; }
        /// <summary>
        /// Maker fee
        /// </summary>
        [JsonProperty("maker_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// The last update time
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("mtime")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// The price per unit of the order
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// The order side
        /// </summary>
        [JsonConverter(typeof(OrderSideIntConverter))]
        [JsonProperty("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// The source of the order
        /// </summary>
        public string Source { get; set; } = string.Empty;
        /// <summary>
        /// Taker fee
        /// </summary>
        [JsonProperty("taker_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal TakerFee { get; set; }
        /// <summary>
        /// The type of the order
        /// </summary>
        [JsonConverter(typeof(OrderTypeIntConverter))]
        public OrderType Type { get; set; }
        /// <summary>
        /// The id of the user that placed the order
        /// </summary>
        [JsonProperty("user")]
        public long UserId { get; set; }
        /// <summary>
        /// The symbol of the order
        /// </summary>
        [JsonProperty("market")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The client id
        /// </summary>
        [JsonProperty("client_id")]
        public string ClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// Quantity of the last trade filled for this order
        /// </summary>
        [JsonProperty("last_deal_amount")]
        public decimal? LastTradeQuantity { get; set; }
        /// <summary>
        /// Price of the last trade filled for this order
        /// </summary>
        [JsonProperty("last_deal_price")]
        public decimal? LastTradePrice { get; set; }
        /// <summary>
        /// Timestamp of the last trade filled for this order
        /// </summary>
        [JsonProperty("last_deal_time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime? LastTradeTime { get; set; }
        /// <summary>
        /// Id of the last trade filled for this order
        /// </summary>
        [JsonProperty("last_deal_id")]
        public long? LastTradeId { get; set; }
        /// <summary>
        /// Role of the last trade filled for this order
        /// </summary>
        [JsonProperty("last_role")]
        public TransactionRole? LastTradeRole { get; set; }
    }
}
