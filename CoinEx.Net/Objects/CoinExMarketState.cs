using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects
{
    public class CoinExMarketState
    {
        /// <summary>
        /// The timestamp of the data
        /// </summary>
        [JsonProperty("date"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The market state data
        /// </summary>
        public CoinExMarketStateData Ticker { get; set; }
    }

    public class CoinExMarketStatesList
    {
        /// <summary>
        /// The timestamp of the data
        /// </summary>
        [JsonProperty("date"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The data specifiec as market -> market state data
        /// </summary>
        [JsonProperty("ticker")]
        public Dictionary<string, CoinExMarketStateData> Tickers { get; set; }
    }

    public class CoinExMarketStateData
    {
        /// <summary>
        /// The best buy price available on the market
        /// </summary>
        [JsonProperty("buy"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBuyPrice { get; set; }
        /// <summary>
        /// The amount of the best buy price
        /// </summary>
        [JsonProperty("buy_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBuyAmount { get; set; }
        /// <summary>
        /// The best sell price available on the market
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
        /// The price of the last transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Last { get; set; }
        /// <summary>
        /// The volume of the market asset. i.e. for market ETHBTC this is the volume in ETH
        /// </summary>
        [JsonProperty("vol"), JsonConverter(typeof(DecimalConverter))]
        public decimal Volume { get; set; }
    }
}
