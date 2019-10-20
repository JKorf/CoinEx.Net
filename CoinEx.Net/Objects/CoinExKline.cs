using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Kline data
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class CoinExKline
    {
        /// <summary>
        /// The open time of this kline
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [ArrayProperty(0)]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The price of the market when this kline started
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(1)]
        public decimal Open { get; set; }
        /// <summary>
        /// The price of the market when this kline ended
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(2)]
        public decimal Close { get; set; }
        /// <summary>
        /// The highest price of the market during this kline
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(3)]
        public decimal High { get; set; }
        /// <summary>
        /// The lowest price of the market during this kline
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(4)]
        public decimal Low { get; set; }
        /// <summary>
        /// The volume of the market asset. i.e. for market ETHBTC this is the volume in ETH
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(5)]
        public decimal Volume { get; set; }
        /// <summary>
        /// The volume of the base asset. i.e. for market ETHBTC this is the volume in BTC
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(6)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The market for this kline
        /// </summary>
        [ArrayProperty(7)]
        public string Market { get; set; } = "";
    }
}
