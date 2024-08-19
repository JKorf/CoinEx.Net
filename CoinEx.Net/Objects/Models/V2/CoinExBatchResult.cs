using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Batch operation result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record CoinExBatchResult<T>
    {
        /// <summary>
        /// Result code
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }
        /// <summary>
        /// Result message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The result data, only available when Success is true
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        public bool Success => Code == 0;
    }
}
