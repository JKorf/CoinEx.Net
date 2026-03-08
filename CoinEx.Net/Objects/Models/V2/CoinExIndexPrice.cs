using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Index price
    /// </summary>
    [SerializationModel]
    public record CoinExIndexPrice
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>created_at</c>"] Timestamp
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// ["<c>price</c>"] Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// ["<c>sources</c>"] Index sources
        /// </summary>
        [JsonPropertyName("sources")]
        public CoinExIndexPriceSource[] Sources { get; set; } = Array.Empty<CoinExIndexPriceSource>();
    }

    /// <summary>
    /// Index price source
    /// </summary>
    [SerializationModel]
    public record CoinExIndexPriceSource
    {
        /// <summary>
        /// ["<c>exchange</c>"] Exchange
        /// </summary>
        [JsonPropertyName("exchange")]
        public string Exchange { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>created_at</c>"] Timestamp
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// ["<c>index_weight</c>"] Weight of the source
        /// </summary>
        [JsonPropertyName("index_weight")]
        public decimal Weight { get; set; }
        /// <summary>
        /// ["<c>index_price</c>"] Price
        /// </summary>
        [JsonPropertyName("index_price")]
        public decimal? Price { get; set; }
    }
}
