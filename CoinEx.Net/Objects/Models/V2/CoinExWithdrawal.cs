using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Withdrawal info
    /// </summary>
    public record CoinExWithdrawal
    {
        /// <summary>
        /// Withdrawal id
        /// </summary>
        [JsonPropertyName("withdraw_id")]
        public long Id { get; set; }
        /// <summary>
        /// Creation time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Asset
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Network
        /// </summary>
        [JsonPropertyName("chain")]
        public string Network { get; set; } = string.Empty;
        /// <summary>
        /// Memo
        /// </summary>
        [JsonPropertyName("memo")]
        public string Memo { get; set; } = string.Empty;
        /// <summary>
        /// Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Actual withdrawal quantity
        /// </summary>
        [JsonPropertyName("actual_amount")]
        public decimal ActualQuantity { get; set; }
        /// <summary>
        /// Fee
        /// </summary>
        [JsonPropertyName("tx_fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// Transaction id
        /// </summary>
        [JsonPropertyName("tx_id")]
        public string TransactionId { get; set; } = string.Empty;
        /// <summary>
        /// Destination address
        /// </summary>
        [JsonPropertyName("to_address")]
        public string ToAddress { get; set; } = string.Empty;
        /// <summary>
        /// Number of confirmations
        /// </summary>
        [JsonPropertyName("confirmation")]
        public int Confirmations { get; set; }
        /// <summary>
        /// Blockchain explorer url for the transaction
        /// </summary>
        [JsonPropertyName("explorer_tx_url")]
        public string TransactionExplorerUrl { get; set; } = string.Empty;
        /// <summary>
        /// Blockchain explorer url for the deposit address
        /// </summary>
        [JsonPropertyName("explorer_address_url")]
        public string WithdrawalAddressExplorerUrl { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        [JsonPropertyName("status")]
        public WithdrawStatusV2 Status { get; set; }
        /// <summary>
        /// Remark
        /// </summary>
        [JsonPropertyName("remark")]
        public string? Remark { get; set; } = string.Empty;
    }
}
