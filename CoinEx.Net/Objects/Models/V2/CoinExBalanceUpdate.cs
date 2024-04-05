using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    internal record CoinExBalanceUpdateWrapper
    {
        [JsonPropertyName("balance_list")]
        public IEnumerable<CoinExBalanceUpdate> Balances { get; set; } = Array.Empty<CoinExBalanceUpdate>();
    }

    /// <summary>
    /// Balance update
    /// </summary>
    public record CoinExBalanceUpdate
    {
        /// <summary>
        /// Margin symbol
        /// </summary>
        [JsonPropertyName("margin_market")]
        public string? MarginSymbol { get; set; }
        /// <summary>
        /// Asset name
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Available amount
        /// </summary>
        [JsonPropertyName("available")]
        public decimal Available { get; set; }
        /// <summary>
        /// Frozen amount
        /// </summary>
        [JsonPropertyName("frozen")]
        public decimal Frozen { get; set; }
        /// <summary>
        /// Update time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdateTime { get; set; }
    }
}
