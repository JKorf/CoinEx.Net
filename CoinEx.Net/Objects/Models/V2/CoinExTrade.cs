using CoinEx.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    internal record CoinExTradeWrapper
    {
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        [JsonPropertyName("deal_list")]
        public IEnumerable<CoinExTrade> Trades { get; set; } = Array.Empty<CoinExTrade>();
    }

    /// <summary>
    /// Trade info
    /// </summary>
    public record CoinExTrade
    {
        /// <summary>
        /// Trade id
        /// </summary>
        [JsonPropertyName("deal_id")]
        public long Id { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Trade side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// Price traded at
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// Quantity traded
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
    }
}
