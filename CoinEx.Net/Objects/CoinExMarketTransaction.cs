using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects
{
    public class CoinExMarketTransaction
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        [JsonProperty("date_ms"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        public long Id { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        [JsonConverter(typeof(TransactionTypeConverter))]
        public TransactionType Type { get; set; }
    }
}
