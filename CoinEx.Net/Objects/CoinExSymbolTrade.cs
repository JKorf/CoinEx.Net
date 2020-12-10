using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using CryptoExchange.Net.ExchangeInterfaces;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Symbol trade info
    /// </summary>
    public class CoinExSymbolTrade: ICommonRecentTrade
    {
        /// <summary>
        /// The amount of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        /// <summary>
        /// The timestamp of the transaction
        /// </summary>
        [JsonProperty("date_ms"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The id of the transaction
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The price per unit of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// The type of the transaction
        /// </summary>
        [JsonConverter(typeof(TransactionTypeConverter))]
        public TransactionType Type { get; set; }

        decimal ICommonRecentTrade.CommonPrice => Price;
        decimal ICommonRecentTrade.CommonQuantity => Amount;
        DateTime ICommonRecentTrade.CommonTradeTime => Timestamp;
    }
}
