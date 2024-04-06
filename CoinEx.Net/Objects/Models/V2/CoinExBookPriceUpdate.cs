using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Best book prices update
    /// </summary>
    public record CoinExBookPriceUpdate
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Update time
        /// </summary>
        [JsonPropertyName("update_at")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// Current best bid price
        /// </summary>
        [JsonPropertyName("best_bid_price")]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// Current best bid quantity
        /// </summary>
        [JsonPropertyName("best_bid_size")]
        public decimal BestBidQuantity { get; set; }
        /// <summary>
        /// Current best ask price
        /// </summary>
        [JsonPropertyName("best_ask_price")]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// Current best ask quantity
        /// </summary>
        [JsonPropertyName("best_ask_size")]
        public decimal BestAskQuantity { get; set; }
    }
}
