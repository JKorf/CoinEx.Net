using CoinEx.Net.Enums;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Place order request
    /// </summary>
    public record CoinExPlaceOrderRequest
    {
        /// <summary>
        /// The symbol, for example `ETHUSDT`
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// The account type
        /// </summary>
        [JsonPropertyName("market_type")]
        public AccountType AccountType { get; set; }
        /// <summary>
        /// The side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// The order type
        /// </summary>
        [JsonPropertyName("type")]
        public OrderTypeV2 OrderType { get; set; }
        /// <summary>
        /// The asset the quantity is in, for market orders van be the base or quote asset
        /// </summary>
        [JsonPropertyName("ccy"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? QuantityAsset { get; set; }
        /// <summary>
        /// The quantity
        /// </summary>
        [JsonPropertyName("amount"), JsonConverter(typeof(CryptoExchange.Net.Converters.SystemTextJson.DecimalStringWriterConverter))]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The limit price
        /// </summary>
        [JsonPropertyName("price"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonConverter(typeof(CryptoExchange.Net.Converters.SystemTextJson.DecimalStringWriterConverter))]
        public decimal? Price { get; set; }
        /// <summary>
        /// The client order id
        /// </summary>
        [JsonPropertyName("client_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ClientOrderId { get; set; }
        /// <summary>
        /// Whether to hide the order
        /// </summary>
        [JsonPropertyName("is_hide"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? Hide { get; set; }
        /// <summary>
        /// Self trade prevention mode
        /// </summary>
        [JsonPropertyName("stp_mode"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public SelfTradePreventionMode? StpMode { get; set; }
    }
}
