using System;
using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models.Socket
{
    /// <summary>
    /// Transaction data
    /// </summary>
    public record CoinExSocketSymbolTrade
    {
        /// <summary>
        /// The orde side
        /// </summary>
        [JsonConverter(typeof(OrderSideConverter))]
        [JsonProperty("type")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// The timestamp of the transaction
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        [JsonProperty("time")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// The price per unit of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Price { get; set; }
        /// <summary>
        /// The order id of the transaction
        /// </summary>
        [JsonProperty("id")]
        public long OrderId { get; set; }
        /// <summary>
        /// The quantity of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
    }
}
