using CoinEx.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using CryptoExchange.Net.ExchangeInterfaces;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Market Info
    /// </summary>
    public class CoinExMarket: ICommonSymbol
    {
        /// <summary>
        /// The name of the market
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }= "";

        /// <summary>
        /// The minimum amount that can be traded
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("min_amount")]
        public decimal MinAmount { get; set; }

        /// <summary>
        /// The fee for the maker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }

        /// <summary>
        /// The fee for the taker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        
        /// <summary>
        /// The coin being that is being traded against
        /// </summary>
        [JsonProperty("pricing_name")]
        public string PricingName { get; set; }= "";

        /// <summary>
        /// The number of decimals for the price
        /// </summary>
        [JsonConverter(typeof(IntConverter))]
        [JsonProperty("pricing_decimal")]
        public int PricingDecimal { get; set; }

        /// <summary>
        /// The coin being traded
        /// </summary>
        [JsonProperty("trading_name")]
        public string TradingName { get; set; } = "";
        
        /// <summary>
        /// The number of decimals for the price
        /// </summary>
        [JsonProperty("trading_decimal")]
        //[JsonConverter(typeof(IntConverter))]
        public int TradingDecimal { get; set; }

        string ICommonSymbol.CommonName => Name;
        decimal ICommonSymbol.CommonMinimumTradeSize => MinAmount;
    }
}
