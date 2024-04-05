using CoinEx.Net.Enums;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Order update
    /// </summary>
    public record CoinExOrderUpdate
    {
        /// <summary>
        /// Event that triggered the update
        /// </summary>
        [JsonPropertyName("event")]
        public OrderUpdateType Event { get; set; }
        /// <summary>
        /// Order data
        /// </summary>
        [JsonPropertyName("order")]
        public CoinExStreamOrder Order { get; set; } = null!;
    }
}
