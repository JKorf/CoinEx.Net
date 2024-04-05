using CoinEx.Net.Enums;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Stop order update
    /// </summary>
    public record CoinExStopOrderUpdate
    {
        /// <summary>
        /// Event that triggered the update
        /// </summary>
        [JsonPropertyName("event")]
        public StopOrderUpdateType Event { get; set; }
        /// <summary>
        /// Order data
        /// </summary>
        [JsonPropertyName("stop")]
        public CoinExStopOrder Order { get; set; } = null!;
    }
}
