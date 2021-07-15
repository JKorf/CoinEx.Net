using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace CoinEx.Net.Objects.Websocket
{
    /// <summary>
    /// Order info
    /// </summary>
    public class CoinExSocketOrder
    {
        /// <summary>
        /// The total amount of the oder
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        /// <summary>
        /// The time the order was created
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("ctime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// The fee of the order
        /// </summary>
        [JsonProperty("asset_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal AssetFee { get; set; }
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
        /// The executed amount transaction fee
        /// </summary>
        [JsonProperty("deal_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal TransactionFee { get; set; }
        /// <summary>
        /// The executed value
        /// </summary>
        [JsonProperty("deal_money")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal TransactionValue { get; set; }
        /// <summary>
        /// The executed amount
        /// </summary>
        [JsonProperty("deal_stock")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal ExecutedAmount { get; set; }
        /// <summary>
        /// The order id
        /// </summary>
        [JsonProperty("id")]
        public long OrderId { get; set; }
        /// <summary>
        /// Amount of order left to execute
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Left { get; set; }
        /// <summary>
        /// Maker fee
        /// </summary>
        [JsonProperty("maker_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal MakerFee { get; set; }
        /// <summary>
        /// The last update time
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("mtime")]
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// The price per unit of the order
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// The transaction type
        /// </summary>
        [JsonConverter(typeof(TransactionTypeIntConverter))]
        [JsonProperty("side")]
        public TransactionType TransactionType { get; set; }
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
        public string ClientId { get; set; } = string.Empty;
    }
}
