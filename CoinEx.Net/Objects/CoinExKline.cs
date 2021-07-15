using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using CryptoExchange.Net.ExchangeInterfaces;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Kline data
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class CoinExKline: ICommonKline
    {
        /// <summary>
        /// The open time of this kline
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [ArrayProperty(0)]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The price of the symbol when this kline started
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(1)]
        public decimal Open { get; set; }
        /// <summary>
        /// The price of the symbol when this kline ended
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(2)]
        public decimal Close { get; set; }
        /// <summary>
        /// The highest price of the symbol during this kline
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(3)]
        public decimal High { get; set; }
        /// <summary>
        /// The lowest price of the symbol during this kline
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(4)]
        public decimal Low { get; set; }
        /// <summary>
        /// The volume of the quote asset. i.e. for symbol ETHBTC this is the volume in ETH
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(5)]
        public decimal Volume { get; set; }
        /// <summary>
        /// The volume of the base asset. i.e. for symbol ETHBTC this is the volume in BTC
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [ArrayProperty(6)]
        public decimal Amount { get; set; }

        /// <summary>
        /// The symbol for this kline
        /// </summary>
        [ArrayProperty(7)]
        public string Symbol { get; set; } = string.Empty;

        decimal ICommonKline.CommonHigh => High;
        decimal ICommonKline.CommonLow => Low;
        decimal ICommonKline.CommonOpen => Open;
        decimal ICommonKline.CommonClose => Close;
        decimal ICommonKline.CommonVolume => Volume;
        DateTime ICommonKline.CommonOpenTime => Timestamp;
    }
}
