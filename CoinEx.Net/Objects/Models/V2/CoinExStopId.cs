using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Stop order id
    /// </summary>
    public record CoinExStopId
    {
        /// <summary>
        /// Stop order id
        /// </summary>
        [JsonPropertyName("stop_id")]
        public long StopOrderId { get; set; }
    }
}
