using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects
{
    public class CoinExOrderTransaction
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_money")]
        public decimal Value { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Fee { get; set; }
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; }
        [JsonProperty("id")]
        public long TransactionId { get; set; }
        [JsonProperty("order_id")]
        public long OrderId { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        [JsonConverter(typeof(TransactionRoleConverter))]
        public TransactionRole Role { get; set; }
    }

    public class CoinExOrderTransactionExtended: CoinExOrderTransaction
    {
        public string Market { get; set; }
        [JsonConverter(typeof(TransactionTypeConverter))]
        public TransactionType Type { get; set; }
    }
}
