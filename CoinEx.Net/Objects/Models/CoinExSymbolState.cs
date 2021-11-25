﻿using System;
using System.Collections.Generic;
using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.ExchangeInterfaces;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Symbol state info
    /// </summary>
    public class CoinExSymbolState
    {
        /// <summary>
        /// The timestamp of the data
        /// </summary>
        [JsonProperty("date"), JsonConverter(typeof(DateTimeConverter))]
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
        [JsonProperty("date"), JsonConverter(typeof(DateTimeConverter))]
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
    public class CoinExSymbolStateData: ICommonTicker
    {
        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// The best buy price available on the symbol
        /// </summary>
        [JsonProperty("buy"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// The quantity of the best buy price
        /// </summary>
        [JsonProperty("buy_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestAskQuantity { get; set; }
        /// <summary>
        /// The best sell price available on the symbol
        /// </summary>
        [JsonProperty("sell"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// The quantity of the best sell price
        /// </summary>
        [JsonProperty("sell_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal BestBidQuantity { get; set; }
        /// <summary>
        /// The open price based on a 24H ticker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("open")]
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// The high price based on a 24H ticker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("high")]
        public decimal HighPrice { get; set; }
        /// <summary>
        /// The low price based on a 24H ticker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("low")]
        public decimal LowPrice { get; set; }
        /// <summary>
        /// The price of the last trade
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("last")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// The volume of the quote asset. i.e. for symbol ETHBTC this is the volume in ETH
        /// </summary>
        [JsonProperty("vol"), JsonConverter(typeof(DecimalConverter))]
        public decimal Volume { get; set; }

        string ICommonTicker.CommonSymbol => Symbol;
        decimal ICommonTicker.CommonHighPrice => HighPrice;
        decimal ICommonTicker.CommonLowPrice => LowPrice;
        decimal ICommonTicker.CommonVolume => Volume;
    }
}
