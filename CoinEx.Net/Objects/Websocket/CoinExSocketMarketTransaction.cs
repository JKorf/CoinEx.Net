using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketMarketTransaction
    {
        [JsonConverter(typeof(TransactionTypeConverter))]
        public TransactionType Type { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("time")]
        public DateTime Timestamp { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        [JsonProperty("id")]
        public long OrderId { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
    }
}
