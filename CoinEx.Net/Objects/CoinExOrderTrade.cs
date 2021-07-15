using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Globalization;
using CryptoExchange.Net.ExchangeInterfaces;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Order transaction info
    /// </summary>
    public class CoinExOrderTrade:ICommonTrade
    {
        /// <summary>
        /// The amount of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        /// <summary>
        /// The time the transaction was created
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// The value of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_money")]
        public decimal Value { get; set; }
        /// <summary>
        /// The fee of the transactions
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Fee { get; set; }
        /// <summary>
        /// The asset of the fee
        /// </summary>
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; } = string.Empty;
        /// <summary>
        /// The id of the transaction
        /// </summary>
        [JsonProperty("id")]
        public long TransactionId { get; set; }
        /// <summary>
        /// The id of the order
        /// </summary>
        [JsonProperty("order_id")]
        public long? OrderId { get; set; }
        /// <summary>
        /// The price per unit of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// The role of the transaction, maker or taker
        /// </summary>
        [JsonConverter(typeof(TransactionRoleConverter))]
        public TransactionRole Role { get; set; }

        string ICommonTrade.CommonId => TransactionId.ToString(CultureInfo.InvariantCulture);
        decimal ICommonTrade.CommonPrice => Price;
        decimal ICommonTrade.CommonQuantity => Amount;
        decimal ICommonTrade.CommonFee => Fee;
        string ICommonTrade.CommonFeeAsset => FeeAsset;
        DateTime ICommonTrade.CommonTradeTime => CreateTime;
    }

    /// <summary>
    /// Order transaction info
    /// </summary>
    public class CoinExOrderTradeExtended: CoinExOrderTrade
    {
        /// <summary>
        /// The symbol of the transaction
        /// </summary>
        [JsonProperty("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// The type of the transaction
        /// </summary>
        [JsonConverter(typeof(TransactionTypeConverter))]
        public TransactionType Type { get; set; }
    }
}
