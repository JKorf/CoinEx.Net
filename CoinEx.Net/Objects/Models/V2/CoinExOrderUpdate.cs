using CryptoExchange.Net.Converters.SystemTextJson;
using CoinEx.Net.Enums;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Order update
    /// </summary>
    [SerializationModel]
    public record CoinExOrderUpdate
    {
        /// <summary>
        /// ["<c>event</c>"] Event that triggered the update
        /// </summary>
        [JsonPropertyName("event")]
        public OrderUpdateType Event { get; set; }
        /// <summary>
        /// ["<c>order</c>"] Order data
        /// </summary>
        [JsonPropertyName("order")]
        public CoinExStreamOrder Order { get; set; } = null!;
    }
}
