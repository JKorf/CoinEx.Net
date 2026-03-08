using CryptoExchange.Net.Converters.SystemTextJson;
using CoinEx.Net.Enums;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Stop order update
    /// </summary>
    [SerializationModel]
    public record CoinExStopOrderUpdate
    {
        /// <summary>
        /// ["<c>event</c>"] Event that triggered the update
        /// </summary>
        [JsonPropertyName("event")]
        public StopOrderUpdateType Event { get; set; }
        /// <summary>
        /// ["<c>stop</c>"] Order data
        /// </summary>
        [JsonPropertyName("stop")]
        public CoinExStopOrder Order { get; set; } = null!;
    }
}
