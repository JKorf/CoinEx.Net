using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Order book info
    /// </summary>
    public record CoinExOrderBook
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Is full order book
        /// </summary>
        [JsonPropertyName("is_full")]
        public bool IsFull { get; set; }

        /// <summary>
        /// The book data
        /// </summary>
        [JsonPropertyName("depth")]
        public CoinExOrderBookData Data { get; set; } = null!;
    }

    /// <summary>
    /// Order book data
    /// </summary>
    public record CoinExOrderBookData
    { 
        /// <summary>
        /// Asks list
        /// </summary>
        [JsonPropertyName("asks")]
        public IEnumerable<CoinExOrderBookEntry> Asks { get; set; } = Array.Empty<CoinExOrderBookEntry>();
        /// <summary>
        /// Bids list
        /// </summary>
        [JsonPropertyName("bids")]
        public IEnumerable<CoinExOrderBookEntry> Bids { get; set; } = Array.Empty<CoinExOrderBookEntry>();

        /// <summary>
        /// Last price
        /// </summary>
        [JsonPropertyName("last")]
        public decimal LastPrice { get; set; }
        /// <summary>
        /// Update time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// Checksum for validating the order book is correct
        /// </summary>
        [JsonPropertyName("checksum")]
        public long Checksum { get; set; }
    }

    /// <summary>
    /// Order book entry
    /// </summary>
    [JsonConverter(typeof(CryptoExchange.Net.Converters.SystemTextJson.ArrayConverter))]
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
