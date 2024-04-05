using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Transfer info
    /// </summary>
    public record CoinExTransfer
    {
        /// <summary>
        /// Margin symbol if either from account or to account was Margin
        /// </summary>
        [JsonPropertyName("margin_market")]
        public string? MarginSymbol { get; set; }
        /// <summary>
        /// Creation time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// From account type
        /// </summary>
        [JsonPropertyName("from_account_type")]
        public AccountType FromAccountType { get; set; }
        /// <summary>
        /// To account type
        /// </summary>
        [JsonPropertyName("to_account_type")]
        public AccountType ToAccountType { get; set; }
        /// <summary>
        /// Asset
        /// </summary>
        [JsonPropertyName("ccy")]
        public string? Asset { get; set; } = string.Empty;
        /// <summary>
        /// Transfer quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Transfer status
        /// </summary>
        [JsonPropertyName("status")]
        public TransferStatus Status { get; set; }
    }
}
