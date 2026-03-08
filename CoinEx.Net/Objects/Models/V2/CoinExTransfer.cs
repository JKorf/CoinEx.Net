using CryptoExchange.Net.Converters.SystemTextJson;
using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Transfer info
    /// </summary>
    [SerializationModel]
    public record CoinExTransfer
    {
        /// <summary>
        /// ["<c>margin_market</c>"] Margin symbol if either from account or to account was Margin
        /// </summary>
        [JsonPropertyName("margin_market")]
        public string? MarginSymbol { get; set; }
        /// <summary>
        /// ["<c>created_at</c>"] Creation time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>from_account_type</c>"] From account type
        /// </summary>
        [JsonPropertyName("from_account_type")]
        public AccountType FromAccountType { get; set; }
        /// <summary>
        /// ["<c>to_account_type</c>"] To account type
        /// </summary>
        [JsonPropertyName("to_account_type")]
        public AccountType ToAccountType { get; set; }
        /// <summary>
        /// ["<c>ccy</c>"] Asset
        /// </summary>
        [JsonPropertyName("ccy")]
        public string? Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>amount</c>"] Transfer quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>status</c>"] Transfer status
        /// </summary>
        [JsonPropertyName("status")]
        public TransferStatus Status { get; set; }
    }
}
