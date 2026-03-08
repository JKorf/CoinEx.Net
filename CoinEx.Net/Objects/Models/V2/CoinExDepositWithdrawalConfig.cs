using CryptoExchange.Net.Converters.SystemTextJson;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Deposit and withdrawal configuration and info
    /// </summary>
    [SerializationModel]
    public record CoinExDepositWithdrawalConfig
    {
        /// <summary>
        /// ["<c>asset</c>"] Asset information
        /// </summary>
        [JsonPropertyName("asset")]
        public CoinExDepositWithdrawalAsset Asset { get; set; } = null!;
        /// <summary>
        /// ["<c>chains</c>"] Available networks
        /// </summary>
        [JsonPropertyName("chains")]
        public CoinExNetwork[] Networks { get; set; } = null!;
    }

    /// <summary>
    /// Asset infos
    /// </summary>
    [SerializationModel]
    public record CoinExDepositWithdrawalAsset
    {
        /// <summary>
        /// ["<c>ccy</c>"] Asset name
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>deposit_enabled</c>"] Is deposit enabled
        /// </summary>
        [JsonPropertyName("deposit_enabled")]
        public bool DepositEnabled { get; set; }
        /// <summary>
        /// ["<c>withdraw_enabled</c>"] Is withdrawal enabled
        /// </summary>
        [JsonPropertyName("withdraw_enabled")]
        public bool WithdrawEnabled { get; set; }
        /// <summary>
        /// ["<c>inter_transfer_enabled</c>"] Is inter user transfer enabled
        /// </summary>
        [JsonPropertyName("inter_transfer_enabled")]
        public bool InterTransferEnabled { get; set; }
        /// <summary>
        /// ["<c>is_st</c>"] Is st
        /// </summary>
        [JsonPropertyName("is_st")]
        public bool IsSt { get; set; }
    }

    /// <summary>
    /// Network info
    /// </summary>
    [SerializationModel]
    public record CoinExNetwork
    {
        /// <summary>
        /// ["<c>chain</c>"] Network name
        /// </summary>
        [JsonPropertyName("chain")]
        public string Network { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>min_deposit_amount</c>"] Min deposit quantity
        /// </summary>
        [JsonPropertyName("min_deposit_amount")]
        public decimal MinDepositQuantity { get; set; }
        /// <summary>
        /// ["<c>min_withdraw_amount</c>"] Min withdraw quantity
        /// </summary>
        [JsonPropertyName("min_withdraw_amount")]
        public decimal MinWithdrawQuantity { get; set; }
        /// <summary>
        /// ["<c>deposit_enabled</c>"] Is deposit enabled
        /// </summary>
        [JsonPropertyName("deposit_enabled")]
        public bool DepositEnabled { get; set; }
        /// <summary>
        /// ["<c>deposit_delay_minutes</c>"] Deposit delay
        /// </summary>
        [JsonPropertyName("deposit_delay_minutes")]
        public int? DepositDelayInMinutes { get; set; }
        /// <summary>
        /// ["<c>withdraw_enabled</c>"] Is withdrawal enabled
        /// </summary>
        [JsonPropertyName("withdraw_enabled")]
        public bool WithdrawEnabled { get; set; }
        /// <summary>
        /// ["<c>safe_confirmations</c>"] Number of confirmations needed
        /// </summary>
        [JsonPropertyName("safe_confirmations")]
        public int? SafeConfirmations { get; set; }
        /// <summary>
        /// ["<c>irreversible_confirmations</c>"] Number of confirmations before transaction is irreversable
        /// </summary>
        [JsonPropertyName("irreversible_confirmations")]
        public int? IrreversableConfirmations { get; set; }
        /// <summary>
        /// ["<c>deflation_rate</c>"] Deflation rate
        /// </summary>
        [JsonPropertyName("deflation_rate")]
        public decimal? DeflationRate { get; set; }
        /// <summary>
        /// ["<c>withdrawal_fee</c>"] Withdrawal fee
        /// </summary>
        [JsonPropertyName("withdrawal_fee")]
        public decimal? WithdrawalFee { get; set; }
        /// <summary>
        /// ["<c>withdrawal_precision</c>"] Withdrawal precision
        /// </summary>
        [JsonPropertyName("withdrawal_precision")]
        public int? WithdrawalPrecision { get; set; }
        /// <summary>
        /// ["<c>memo</c>"] Memo
        /// </summary>
        [JsonPropertyName("memo")]
        public string Memo { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>is_memo_required_for_deposit</c>"] Is memo required for deposits
        /// </summary>
        [JsonPropertyName("is_memo_required_for_deposit")]
        public bool MemoRequired { get; set; }
        /// <summary>
        /// ["<c>explorer_asset_url</c>"] Blockchain explorer url
        /// </summary>
        [JsonPropertyName("explorer_asset_url")]
        public string ExplorerUrl { get; set; } = string.Empty;
    }
}
