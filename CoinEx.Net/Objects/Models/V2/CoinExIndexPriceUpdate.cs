
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Index price update
    /// </summary>
    public record CoinExIndexPriceUpdate
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Index price
        /// </summary>
        [JsonPropertyName("index_price")]
        public decimal IndexPrice { get; set; }
        /// <summary>
        /// Mark price
        /// </summary>
        [JsonPropertyName("mark_price")]
        public decimal? MarkPrice { get; set; }
    }
}
