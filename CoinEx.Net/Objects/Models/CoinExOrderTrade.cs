using System;
using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Order transaction info
    /// </summary>
    public record CoinExOrderTrade
    {
        /// <summary>
        /// The quantity of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The time the transaction was created
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("create_time")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The value of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_money")]
        public decimal QuoteQuantity { get; set; }
        /// <summary>
        /// The fee of the transactions
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Fee { get; set; }
        /// <summary>
        /// The asset of the fee
        /// </summary>
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; } = string.Empty;
        /// <summary>
        /// The id of the transaction
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }
        /// <summary>
        /// The id of the order
        /// </summary>
        [JsonProperty("order_id")]
        public long? OrderId { get; set; }
        /// <summary>
        /// The price per unit of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// The role of the transaction, maker or taker
        /// </summary>
        [JsonConverter(typeof(TransactionRoleConverter))]
        public TransactionRole Role { get; set; }
        /// <summary>
        /// Order side
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }
    }

    /// <summary>
    /// Order transaction info
    /// </summary>
    public record CoinExOrderTradeExtended: CoinExOrderTrade
    {
        /// <summary>
        /// The symbol of the transaction
        /// </summary>
        [JsonProperty("market")]
        public string Symbol { get; set; } = string.Empty;
    }
}
