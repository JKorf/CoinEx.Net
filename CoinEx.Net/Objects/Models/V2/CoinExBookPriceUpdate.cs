using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Best book prices update
    /// </summary>
    [SerializationModel]
    public record CoinExBookPriceUpdate
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>updated_at</c>"] Update time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// ["<c>best_bid_price</c>"] Current best bid price
        /// </summary>
        [JsonPropertyName("best_bid_price")]
        public decimal BestBidPrice { get; set; }
        /// <summary>
        /// ["<c>best_bid_size</c>"] Current best bid quantity
        /// </summary>
        [JsonPropertyName("best_bid_size")]
        public decimal BestBidQuantity { get; set; }
        /// <summary>
        /// ["<c>best_ask_price</c>"] Current best ask price
        /// </summary>
        [JsonPropertyName("best_ask_price")]
        public decimal BestAskPrice { get; set; }
        /// <summary>
        /// ["<c>best_ask_size</c>"] Current best ask quantity
        /// </summary>
        [JsonPropertyName("best_ask_size")]
        public decimal BestAskQuantity { get; set; }
    }
}
