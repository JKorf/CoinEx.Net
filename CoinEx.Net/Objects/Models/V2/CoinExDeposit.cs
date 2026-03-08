using CryptoExchange.Net.Converters.SystemTextJson;
using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Deposit info
    /// </summary>
    [SerializationModel]
    public record CoinExDeposit
    {
        /// <summary>
        /// ["<c>deposit_id</c>"] Deposit id
        /// </summary>
        [JsonPropertyName("deposit_id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>created_at</c>"] Creation time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ["<c>tx_id</c>"] Transaction id
        /// </summary>
        [JsonPropertyName("tx_id")]
        public string TransactionId { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>ccy</c>"] Asset
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>chain</c>"] Network
        /// </summary>
        [JsonPropertyName("chain")]
        public string Network { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>amount</c>"] Quantity deposited
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>actual_amount</c>"] Actual amount received
        /// </summary>
        [JsonPropertyName("actual_amount")]
        public decimal? QuantityCredited { get; set; }
        /// <summary>
        /// ["<c>to_address</c>"] Deposit address
        /// </summary>
        [JsonPropertyName("to_address")]
        public string DepositAddress { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>confirmations</c>"] Amount of confirmations
        /// </summary>
        [JsonPropertyName("confirmations")]
        public int Confirmations { get; set; }
        /// <summary>
        /// ["<c>status</c>"] Status of the deposit
        /// </summary>
        [JsonPropertyName("status")]
        public DepositStatus Status { get; set; }
        /// <summary>
        /// ["<c>tx_explorer_url</c>"] Blockchain explorer url for the transaction
        /// </summary>
        [JsonPropertyName("tx_explorer_url")]
        public string TransactionExplorerUrl { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>to_addr_explorer_url</c>"] Blockchain explorer url for the deposit address
        /// </summary>
        [JsonPropertyName("to_addr_explorer_url")]
        public string DepositAddressExplorerUrl { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>remark</c>"] Remark
        /// </summary>
        [JsonPropertyName("remark")]
        public string Remark { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>deposit_method</c>"] Deposit method
        /// </summary>
        [JsonPropertyName("deposit_method")]
        public MovementMethod DepositMethod { get; set; }
    }
}
