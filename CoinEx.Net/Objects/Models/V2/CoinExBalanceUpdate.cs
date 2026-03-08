using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    [SerializationModel]
    internal record CoinExBalanceUpdateWrapper
    {
        [JsonPropertyName("balance_list")]
        public CoinExBalanceUpdate[] Balances { get; set; } = Array.Empty<CoinExBalanceUpdate>();
    }

    /// <summary>
    /// Balance update
    /// </summary>
    [SerializationModel]
    public record CoinExBalanceUpdate
    {
        /// <summary>
        /// ["<c>margin_market</c>"] Margin symbol
        /// </summary>
        [JsonPropertyName("margin_market")]
        public string? MarginSymbol { get; set; }
        /// <summary>
        /// ["<c>ccy</c>"] Asset name
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>available</c>"] Available amount
        /// </summary>
        [JsonPropertyName("available")]
        public decimal Available { get; set; }
        /// <summary>
        /// ["<c>frozen</c>"] Frozen amount
        /// </summary>
        [JsonPropertyName("frozen")]
        public decimal Frozen { get; set; }
        /// <summary>
        /// ["<c>updated_at</c>"] Update time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdateTime { get; set; }
    }
}
