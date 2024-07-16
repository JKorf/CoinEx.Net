using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Asset info
    /// </summary>
    public record CoinExAsset
    {
        /// <summary>
        /// Short name
        /// </summary>
        [JsonPropertyName("short_name")]
        public string ShortName { get; set; } = string.Empty;
        /// <summary>
        /// Full name
        /// </summary>
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// Website url
        /// </summary>
        [JsonPropertyName("website_url")]
        public string WebsiteUrl { get; set; } = string.Empty;
        /// <summary>
        /// White paper url
        /// </summary>
        [JsonPropertyName("white_paper_url")]
        public string WhitePaperUrl { get; set; } = string.Empty;
        /// <summary>
        /// Network info
        /// </summary>
        [JsonPropertyName("chain_info")]
        public IEnumerable<CoinExAssetNetwork> Networks { get; set; } = Array.Empty<CoinExAssetNetwork>();
    }

    /// <summary>
    /// Asset network info
    /// </summary>
    public record CoinExAssetNetwork
    {
        /// <summary>
        /// Network name
        /// </summary>
        [JsonPropertyName("chain_name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Identity
        /// </summary>
        [JsonPropertyName("identity")]
        public string? Identity { get; set; }
        /// <summary>
        /// Explorer url
        /// </summary>
        [JsonPropertyName("explorer_url")]
        public string ExplorerUrl { get; set; } = string.Empty;
    }


}
