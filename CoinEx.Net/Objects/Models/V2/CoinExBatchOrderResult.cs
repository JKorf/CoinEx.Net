using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Batch operation result
    /// </summary>
    [SerializationModel]
    public record CoinExBatchOrderResult: CoinExOrder
    {
        /// <summary>
        /// ["<c>code</c>"] Result code
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }
        /// <summary>
        /// ["<c>message</c>"] Result message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        public bool Success => Code == 0;
    }
}
