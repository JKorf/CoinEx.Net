using System;
using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Symbol trade info
    /// </summary>
    public record CoinExSymbolTrade
    {
        /// <summary>
        /// The quantity of the transaction
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The timestamp of the transaction
        /// </summary>
        [JsonProperty("date_ms"), JsonConverter(typeof(DateTimeConverter))]
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
        /// Order side
        /// </summary>
        [JsonConverter(typeof(OrderSideConverter))]
        [JsonProperty("type")]
        public OrderSide Side { get; set; }
    }
}
