using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <inheritdoc />
    public record CoinExFuturesTicker : CoinExTicker
    {
        /// <summary>
        /// Index price
        /// </summary>
        [JsonPropertyName("index_price")]
        public decimal IndexPrice { get; set; }
        /// <summary>
        /// Mark price
        /// </summary>
        [JsonPropertyName("mark_price")]
        public decimal MarkPrice { get; set; }
    }
}
