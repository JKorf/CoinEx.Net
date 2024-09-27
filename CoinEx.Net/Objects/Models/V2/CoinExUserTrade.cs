using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// User trade info
    /// </summary>
    public record CoinExUserTrade
    {
        /// <summary>
        /// Trade id
        /// </summary>
        [JsonPropertyName("deal_id")]
        public long Id { get; set; }
        /// <summary>
        /// Trade time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Trade side
        /// </summary>
        [JsonPropertyName("side")]
        public OrderSide Side { get; set; }
        /// <summary>
        /// Order id
        /// </summary>
        [JsonPropertyName("order_id")]
        public long OrderId { get; set; }
        /// <summary>
        /// Margin symbol
        /// </summary>
        [JsonPropertyName("margin_market")]
        public string? MarginSymbol { get; set; }
        /// <summary>
        /// Trade price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        /// <summary>
        /// Quantity traded
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Fee paid
        /// </summary>
        [JsonPropertyName("fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// Fee asset
        /// </summary>
        [JsonPropertyName("fee_ccy")]
        public string FeeAsset { get; set; } = string.Empty;
        /// <summary>
        /// Role
        /// </summary>
        [JsonPropertyName("role")]
        public TransactionRole Role { get; set; }
    }
}
