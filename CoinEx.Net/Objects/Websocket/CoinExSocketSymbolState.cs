using CoinEx.Net.Converters;
using CryptoExchange.Net.Attributes;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Websocket
{
    /// <summary>
    /// Symbol state info
    /// </summary>
    public class CoinExSocketSymbolState
    {
        /// <summary>
        /// The close price of the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Close { get; set; }
        /// <summary>
        /// The volume of the base asset. i.e. for symbol ETHBTC this is the volume in BTC
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal")]
        public decimal Value { get; set; }
        /// <summary>
        /// The highest symbol price in the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal High { get; set; }
        /// <summary>
        /// The last symbol trade in the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Last { get; set; }
        /// <summary>
        /// The lowest symbol price in the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Low { get; set; }
        /// <summary>
        /// The open price of the period
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Open { get; set; }
        /// <summary>
        /// The period the data is over in seconds
        /// </summary>
        public int Period { get; set; }
        /// <summary>
        /// The volume of the quote asset. i.e. for symbol ETHBTC this is the volume in ETH
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Volume { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        [JsonOptionalProperty]
        public string Symbol { get; set; }
    }
}
