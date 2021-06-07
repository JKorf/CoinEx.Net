using CoinEx.Net.Converters;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Globalization;
using CryptoExchange.Net.ExchangeInterfaces;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Order info
    /// </summary>
    public class CoinExOrder: ICommonOrderId, ICommonOrder
    {
        /// <summary>
        /// The amount of the order
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        /// <summary>
        /// The fee of the order
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("asset_fee")]
        public decimal AssetFee { get; set; }
        /// <summary>
        /// The asset of the fee
        /// </summary>
        [JsonProperty("fee_asset")]
        public string FeeAsset { get; set; } = "";
        /// <summary>
        /// The fee discount
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("fee_discount")]
        public decimal FeeDiscount { get; set; }
        /// <summary>
        /// Average price of the executed order for market orders
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("avg_price")]
        public decimal AveragePrice { get; set; }
        /// <summary>
        /// The time the order was created
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("create_time")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// The time the order was finished
        /// </summary>
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("finished_time")]
        [JsonOptionalProperty]
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// The executed amount
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_amount")]
        public decimal ExecutedAmount { get; set; }
        /// <summary>
        /// The fee of the executed amount
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_fee")]
        public decimal OrderFee { get; set; }
        /// <summary>
        /// The value of the executed amount
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("deal_money")]
        public decimal ExecutedValue { get; set; }
        /// <summary>
        /// The id of the order
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The amount still left to execute
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Left { get; set; }
        /// <summary>
        /// The maker fee rate
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// The symbol of the order
        /// </summary>
        [JsonProperty("market")]
        public string Symbol { get; set; } = "";
        /// <summary>
        /// The type of the order
        /// </summary>
        [JsonConverter(typeof(OrderTypeConverter))]
        [JsonProperty("order_type")]
        public OrderType OrderType { get; set; }
        /// <summary>
        /// The price per unit of the order
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// The source id optionally specified by the client
        /// </summary>
        [JsonProperty("source_id")]
        [JsonOptionalProperty]
        public long? SourceId { get; set; }
        /// <summary>
        /// The client id optionally specified by the client
        /// </summary>
        [JsonProperty("client_id")]
        [JsonOptionalProperty]
        public string? ClientId { get; set; }
        /// <summary>
        /// The status of the order
        /// </summary>
        [JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }
        /// <summary>
        /// The taker fee rate
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// The transaction type of the order
        /// </summary>
        [JsonConverter(typeof(TransactionTypeConverter))]
        public TransactionType Type { get; set; }

        string ICommonOrderId.CommonId => Id.ToString(CultureInfo.InvariantCulture);
        string ICommonOrder.CommonSymbol => Symbol;
        decimal ICommonOrder.CommonPrice => Price;
        decimal ICommonOrder.CommonQuantity => Amount;
        IExchangeClient.OrderStatus ICommonOrder.CommonStatus =>
            Status == OrderStatus.Canceled ? IExchangeClient.OrderStatus.Canceled :
            Status == OrderStatus.Executed ? IExchangeClient.OrderStatus.Filled :
            IExchangeClient.OrderStatus.Active;
        bool ICommonOrder.IsActive => Status == OrderStatus.UnExecuted;
        DateTime ICommonOrder.CommonOrderTime => CreateTime;

        IExchangeClient.OrderSide ICommonOrder.CommonSide => Type == TransactionType.Buy
            ? IExchangeClient.OrderSide.Sell
            : IExchangeClient.OrderSide.Buy;

        IExchangeClient.OrderType ICommonOrder.CommonType
        {
            get
            {
                if (OrderType == OrderType.Market)
                    return IExchangeClient.OrderType.Market;
                else
                    return IExchangeClient.OrderType.Limit;
            }
        }
    }
}
