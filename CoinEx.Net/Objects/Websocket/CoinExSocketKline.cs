using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace CoinEx.Net.Objects.Websocket
{
    [JsonConverter(typeof(ArrayConverter))]
    public class CoinExSocketKline
    {
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [ArrayProperty(0)]
        public DateTime Timestamp { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(1)]
        public decimal Open { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(2)]
        public decimal Close { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(3)]
        public decimal High { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(4)]
        public decimal Low { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(5)]
        public decimal Volume { get; set; }
    }
}
