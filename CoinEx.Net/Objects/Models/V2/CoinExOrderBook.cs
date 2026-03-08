using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Order book info
    /// </summary>
    [SerializationModel]
    public record CoinExOrderBook
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>is_full</c>"] Is full order book
        /// </summary>
        [JsonPropertyName("is_full")]
        public bool IsFull { get; set; }

        /// <summary>
        /// ["<c>depth</c>"] The book data
        /// </summary>
        [JsonPropertyName("depth")]
        public CoinExOrderBookData Data { get; set; } = null!;
        /// <summary>
        /// ["<c>last</c>"] Last price, only for websocket updates
        /// </summary>
        [JsonPropertyName("last")]
        public decimal? LastPrice { get; set; }
        /// <summary>
        /// ["<c>updated_at</c>"] Update time, only for websocket updates
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// ["<c>checksum</c>"] Checksum, only for websocket updates
        /// </summary>
        [JsonPropertyName("checksum")]
        public long? Checksum { get; set; }
    }

    /// <summary>
    /// Order book data
    /// </summary>
    [SerializationModel]
    public record CoinExOrderBookData
    { 
        /// <summary>
        /// ["<c>asks</c>"] Asks list
        /// </summary>
        [JsonPropertyName("asks")]
        public CoinExOrderBookEntry[] Asks { get; set; } = Array.Empty<CoinExOrderBookEntry>();
        /// <summary>
        /// ["<c>bids</c>"] Bids list
        /// </summary>
        [JsonPropertyName("bids")]
        public CoinExOrderBookEntry[] Bids { get; set; } = Array.Empty<CoinExOrderBookEntry>();

        /// <summary>
        /// ["<c>last</c>"] Last price
        /// </summary>
        [JsonPropertyName("last")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// ["<c>updated_at</c>"] Update time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// ["<c>checksum</c>"] Checksum for validating the order book is correct
        /// </summary>
        [JsonPropertyName("checksum")]
        public long Checksum { get; set; }
    }

    /// <summary>
    /// Order book entry
    /// </summary>
    [JsonConverter(typeof(ArrayConverter<CoinExOrderBookEntry>))]
    [SerializationModel]
    public record CoinExOrderBookEntry : ISymbolOrderBookEntry
    {
        /// <summary>
        /// Price
        /// </summary>
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        [ArrayProperty(1)]
        public decimal Quantity { get; set; }
    }
}
