using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketOrder
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("ctime")]
        public DateTime CreateTime { get; set; }
        [JsonProperty("asset_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal AssetFee { get; set; }
        [JsonProperty("fee_discount")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal FeeDiscount { get; set; }
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; }
        [JsonProperty("deal_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal TransactionFee { get; set; }
        [JsonProperty("deal_money")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal TransactionValue { get; set; }
        [JsonProperty("deal_stock")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal TransactionStock { get; set; }
        [JsonProperty("id")]
        public long OrderId { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Left { get; set; }
        [JsonProperty("maker_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal MakerFee { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("mtime")]
        public DateTime ModifyTime { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        [JsonConverter(typeof(TransactionTypeIntConverter))]
        [JsonProperty("side")]
        public TransactionType TransactionType { get; set; }
        public string Source { get; set; }
        [JsonProperty("taker_fee")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal TakerFee { get; set; }
        [JsonConverter(typeof(OrderTypeIntConverter))]
        public OrderType Type { get; set; }
        [JsonProperty("user")]
        public long UserId { get; set; }
        public string Market { get; set; }
    }
}
