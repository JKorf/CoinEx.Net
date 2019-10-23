using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Symbol state info
    /// </summary>
    public class CoinExSymbolState
    {
        /// <summary>
        /// The timestamp of the data
        /// </summary>
        [JsonProperty("date"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The symbol state data
        /// </summary>
        public CoinExSymbolStateData Ticker { get; set; } = default!;
    }

    /// <summary>
    /// Symbol state list
    /// </summary>
    public class CoinExSymbolStatesList
    {
        /// <summary>
        /// The timestamp of the data
        /// </summary>
        [JsonProperty("date"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The data specified as symbol -> symbol state data
        /// </summary>
        [JsonProperty("ticker")]
        public Dictionary<string, CoinExSymbolStateData> Tickers { get; set; } = new Dictionary<string, CoinExSymbolStateData>();
    }

    /// <summary>
    /// Symbol state data
    /// </summary>
    public class CoinExSymbolStateData
    {
        /// <summary>
        /// The best buy price available on the symbol
        /// </summary>
        [JsonProperty("buy"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBuyPrice { get; set; }
        /// <summary>
        /// The amount of the best buy price
        /// </summary>
        [JsonProperty("buy_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBuyAmount { get; set; }
        /// <summary>
        /// The best sell price available on the symbol
        /// </summary>
        [JsonProperty("sell"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestSellPrice { get; set; }
        /// <summary>
        /// The amount of the best sell price
        /// </summary>
        [JsonProperty("sell_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestSellAmount { get; set; }
        /// <summary>
        /// The open price based on a 24H ticker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Open { get; set; }
        /// <summary>
        /// The high price based on a 24H ticker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal High { get; set; }
        /// <summary>
        /// The low price based on a 24H ticker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Low { get; set; }
        /// <summary>
        /// The price of the last trade
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Last { get; set; }
        /// <summary>
        /// The volume of the quote asset. i.e. for symbol ETHBTC this is the volume in ETH
        /// </summary>
        [JsonProperty("vol"), JsonConverter(typeof(DecimalConverter))]
        public decimal Volume { get; set; }
    }
}
