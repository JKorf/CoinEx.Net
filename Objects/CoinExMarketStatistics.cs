using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects
{
    public class CoinExMarketState
    {
        [JsonProperty("date"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        public CoinExMarketStateData Ticker { get; set; }
    }

    public class CoinExMarketStatesList
    {
        [JsonProperty("date"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        [JsonProperty("ticker")]
        public Dictionary<string, CoinExMarketStateData> Tickers { get; set; }
    }

    public class CoinExMarketStateData
    {
        [JsonProperty("buy"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBuyPrice { get; set; }
        [JsonProperty("buy_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBuyAmount { get; set; }
        [JsonProperty("sell"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestSellPrice { get; set; }
        [JsonProperty("sell_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestSellAmount { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Open { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal High { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Low { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Last { get; set; }
        [JsonProperty("vol"), JsonConverter(typeof(DecimalConverter))]
        public decimal Volume { get; set; }
    }
}
