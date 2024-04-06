using System;
using System.Text.Json.Serialization;
using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Auto deleveraging info
    /// </summary>
    public record CoinExPositionAdl
    {
        /// <summary>
        /// Position id
        /// </summary>
        [JsonPropertyName("position_id")]
        public long Id { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Account type
        /// </summary>
        [JsonPropertyName("market_type")]
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        [JsonPropertyName("order_id")]
        public long OrderId { get; set; }
        /// <summary>
        /// Trade id
        /// </summary>
        [JsonPropertyName("deal_id")]
        public long TradeId { get; set; }
        /// <summary>
        /// Order side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// Order quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Order price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// Role
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
        /// <summary>
        /// Timestamp created
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
    }
}
