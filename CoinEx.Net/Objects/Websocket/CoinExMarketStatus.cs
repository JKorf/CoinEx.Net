using CoinEx.Net.Converters;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExMarketStatus
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Close { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal")]
        public decimal Value { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal High { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Last { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Low { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Open { get; set; }
        public int Period { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Volume { get; set; }
    }
}
