using CoinEx.Net.Converters;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models.Socket
{
    /// <summary>
    /// Symbol state info
    /// </summary>
    public record CoinExSocketSymbolState
    {
        /// <summary>
        /// The close price of the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Close { get; set; }
        /// <summary>
        /// The volume of the quote asset. i.e. for symbol ETHBTC this is the volume in BTC
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal")]
        public decimal QuoteVolume { get; set; }
        /// <summary>
        /// The highest symbol price in the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("high")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// The last symbol trade in the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("last")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// The lowest symbol price in the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("low")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// The open price of the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("open")]
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// The period the data is over in seconds
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// The volume of the base asset. i.e. for symbol ETHBTC this is the volume in ETH
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Volume { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
    }
}
