using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Stop order info
    /// </summary>
    [SerializationModel]
    public record CoinExStopOrder
    {
        /// <summary>
        /// ["<c>stop_id</c>"] Order id
        /// </summary>
        [JsonPropertyName("stop_id")]
        public long StopOrderId { get; set; }
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>margin_market</c>"] Margin symbol
        /// </summary>
        [JsonPropertyName("margin_market")]
        public string? MarginSymbol { get; set; }
        /// <summary>
        /// ["<c>market_type</c>"] Account type
        /// </summary>
        [JsonPropertyName("market_type")]
        public AccountType? AccountType { get; set; }
        /// <summary>
        /// ["<c>ccy</c>"] Asset the quantity is in
        /// </summary>
        [JsonPropertyName("ccy")]
        public string? QuantityAsset { get; set; }
        /// <summary>
        /// ["<c>side</c>"] Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// ["<c>type</c>"] Order type
        /// </summary>
        [JsonPropertyName("type")]
        public OrderTypeV2 Type { get; set; }
        /// <summary>
        /// ["<c>amount</c>"] Order quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Order price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        /// <summary>
        /// ["<c>client_id</c>"] Client order id
        /// </summary>
        [JsonPropertyName("client_id")]
        [JsonConverter(typeof(ClientIdConverter))]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// ["<c>created_at</c>"] Timestamp order was created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>updated_at</c>"] Timestamp order was last updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// ["<c>trigger_price</c>"] Trigger price
        /// </summary>
        [JsonPropertyName("trigger_price")]
        public decimal TriggerPrice { get; set; }
        /// <summary>
        /// ["<c>trigger_direction</c>"] Trigger direction
        /// </summary>
        [JsonPropertyName("trigger_direction")]
        public TriggerDirection TriggerDirection { get; set; }
        /// <summary>
        /// ["<c>trigger_price_type</c>"] Trigger price type
        /// </summary>
        [JsonPropertyName("trigger_price_type")]
        public PriceType TriggerPriceType { get; set; }
        /// <summary>
        /// ["<c>taker_fee_rate</c>"] Taker fee rate
        /// </summary>
        [JsonPropertyName("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// ["<c>maker_fee_rate</c>"] Maker fee rate
        /// </summary>
        [JsonPropertyName("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
    }
}
