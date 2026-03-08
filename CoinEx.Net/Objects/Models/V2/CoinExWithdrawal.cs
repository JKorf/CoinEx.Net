using CryptoExchange.Net.Converters.SystemTextJson;
using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Withdrawal info
    /// </summary>
    [SerializationModel]
    public record CoinExWithdrawal
    {
        /// <summary>
        /// ["<c>withdraw_id</c>"] Withdrawal id
        /// </summary>
        [JsonPropertyName("withdraw_id")]
        public long Id { get; set; }
        /// <summary>
        /// ["<c>created_at</c>"] Creation time
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
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
        /// ["<c>memo</c>"] Memo
        /// </summary>
        [JsonPropertyName("memo")]
        public string Memo { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>amount</c>"] Quantity
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// ["<c>fee_ccy</c>"] Fee asset
        /// </summary>
        [JsonPropertyName("fee_ccy")]
        public string FeeAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>actual_amount</c>"] Actual withdrawal quantity
        /// </summary>
        [JsonPropertyName("actual_amount")]
        public decimal ActualQuantity { get; set; }
        /// <summary>
        /// ["<c>withdraw_method</c>"] Withdraw method
        /// </summary>
        [JsonPropertyName("withdraw_method")]
        public string WithdrawMethod { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>tx_fee</c>"] Fee
        /// </summary>
        [JsonPropertyName("tx_fee")]
        public decimal Fee { get; set; }
        /// <summary>
        /// ["<c>tx_id</c>"] Transaction id
        /// </summary>
        [JsonPropertyName("tx_id")]
        public string TransactionId { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>to_address</c>"] Destination address
        /// </summary>
        [JsonPropertyName("to_address")]
        public string ToAddress { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>confirmations</c>"] Number of confirmations
        /// </summary>
        [JsonPropertyName("confirmations")]
        public int Confirmations { get; set; }
        /// <summary>
        /// ["<c>explorer_tx_url</c>"] Blockchain explorer url for the transaction
        /// </summary>
        [JsonPropertyName("explorer_tx_url")]
        public string TransactionExplorerUrl { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>explorer_address_url</c>"] Blockchain explorer url for the deposit address
        /// </summary>
        [JsonPropertyName("explorer_address_url")]
        public string WithdrawalAddressExplorerUrl { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>status</c>"] Status
        /// </summary>
        [JsonPropertyName("status")]
        public WithdrawStatusV2 Status { get; set; }
        /// <summary>
        /// ["<c>remark</c>"] Remark
        /// </summary>
        [JsonPropertyName("remark")]
        public string? Remark { get; set; } = string.Empty;
    }
}
