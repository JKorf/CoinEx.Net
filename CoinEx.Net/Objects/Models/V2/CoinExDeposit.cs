using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Deposit info
    /// </summary>
    public record CoinExDeposit
    {
        /// <summary>
        /// Deposit id
        /// </summary>
        [JsonPropertyName("deposit_id")]
        public long Id { get; set; }
        /// <summary>
        /// Creation time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Transaction id
        /// </summary>
        [JsonPropertyName("tx_id")]
        public string TransactionId { get; set; } = string.Empty;
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
        /// Quantity deposited
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// Actual amount received
        /// </summary>
        [JsonPropertyName("actual_amount")]
        public decimal? QuantityCredited { get; set; }
        /// <summary>
        /// Deposit address
        /// </summary>
        [JsonPropertyName("to_address")]
        public string DepositAddress { get; set; } = string.Empty;
        /// <summary>
        /// Amount of confirmations
        /// </summary>
        [JsonPropertyName("confirmations")]
        public int Confirmations { get; set; }
        /// <summary>
        /// Status of the deposit
        /// </summary>
        [JsonPropertyName("status")]
        public DepositStatus Status { get; set; }
        /// <summary>
        /// Blockchain explorer url for the transaction
        /// </summary>
        [JsonPropertyName("tx_explorer_url")]
        public string TransactionExplorerUrl { get; set; } = string.Empty;
        /// <summary>
        /// Blockchain explorer url for the deposit address
        /// </summary>
        [JsonPropertyName("to_addr_explorer_url")]
        public string DepositAddressExplorerUrl { get; set; } = string.Empty;
        /// <summary>
        /// Remark
        /// </summary>
        [JsonPropertyName("remark")]
        public string Remark { get; set; } = string.Empty;
        /// <summary>
        /// Deposit method
        /// </summary>
        [JsonPropertyName("deposit_method")]
        public MovementMethod DepositMethod { get; set; }
    }
}
