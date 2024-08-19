using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Batch operation result
    /// </summary>
    public record CoinExBatchOrderResult: CoinExOrder
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
        /// Whether the operation was successful
        /// </summary>
        public bool Success => Code == 0;
    }
}
