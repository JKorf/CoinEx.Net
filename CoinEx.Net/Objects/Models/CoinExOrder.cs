using System;
using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Order info
    /// </summary>
    public record CoinExOrder
    {
        /// <summary>
        /// The quantity of the order
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The fee of the order
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("asset_fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// The fee of the order in quote
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("money_fee")]
        public decimal QuoteFee { get; set; }
        /// <summary>
        /// The asset of the fee
        /// </summary>
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; } = string.Empty;
        /// <summary>
        /// The fee discount
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("fee_discount")]
        public decimal FeeDiscount { get; set; }
        /// <summary>
        /// Average price of the executed order for market orders
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("avg_price")]
        public decimal AveragePrice { get; set; }
        /// <summary>
        /// The time the order was created
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// The time the order was finished
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("finished_time")]
        public DateTime? CloseTime { get; set; }
        /// <summary>
        /// The executed quantity
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_amount")]
        public decimal QuantityFilled { get; set; }
        /// <summary>
        /// The fee of the executed quantity
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_fee")]
        public decimal OrderFee { get; set; }
        /// <summary>
        /// The value of the executed quantity
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_money")]
        public decimal QuoteQuantityFilled { get; set; }
        /// <summary>
        /// The id of the order
        /// </summary>
        public long Id { get; set; }

        [JsonProperty("order_id")]
        private long OrderId { set => Id = value; }
        /// <summary>
        /// The quantity still left to execute
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("left")]
        public decimal QuantityRemaining { get; set; }
        /// <summary>
        /// The maker fee rate
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        [JsonProperty("maker_fee")]
        private decimal MakerFee { set => MakerFeeRate = value; }
        /// <summary>
        /// The symbol of the order
        /// </summary>
        [JsonProperty("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// The type of the order
        /// </summary>
        [JsonConverter(typeof(OrderTypeConverter))]
        [JsonProperty("order_type")]
        public OrderType OrderType { get; set; }
        /// <summary>
        /// The price per unit of the order
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// The source id optionally specified by the client
        /// </summary>
        [JsonProperty("source_id")]
        public long? SourceId { get; set; }
        /// <summary>
        /// The client id optionally specified by the client
        /// </summary>
        [JsonProperty("client_id")]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// The status of the order
        /// </summary>
        [JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }
        /// <summary>
        /// The taker fee rate
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        [JsonProperty("taker_fee")]
        private decimal TakerFee { set => TakerFeeRate = value; }

        /// <summary>
        /// The stop price
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("stop_price")]
        public decimal? StopPrice { get; set; }
        /// <summary>
        /// The transaction type of the order
        /// </summary>
        [JsonConverter(typeof(OrderSideConverter))]
        [JsonProperty("type")]
        public OrderSide Side { get; set; }
    }
}
