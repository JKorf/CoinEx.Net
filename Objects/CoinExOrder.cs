using CoinEx.Net.Converters;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects
{
    public class CoinExOrder
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("asset_fee")]
        public decimal AssetFee { get; set; }
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("fee_discount")]
        public decimal FeeDiscount { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("avg_price")]
        public decimal AveragePrice { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("finished_time")]
        [JsonOptionalProperty]
        public DateTime FinishTime { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_amount")]
        public decimal ExecutedAmount { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_fee")]
        public decimal OrderFee { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_money")]
        public decimal ExecutedValue { get; set; }
        public long Id { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Left { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        public string Market { get; set; }
        [JsonConverter(typeof(OrderTypeConverter))]
        [JsonProperty("order_type")]
        public OrderType OrderType { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        [JsonProperty("source_id")]
        [JsonOptionalProperty]
        public long? SourceId { get; set; }
        [JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("taker_fee_rate")]
        public decimal TakeFeeRate { get; set; }
        [JsonConverter(typeof(TransactionTypeConverter))]
        public TransactionType Type { get; set; }

    }
}
