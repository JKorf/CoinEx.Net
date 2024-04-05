using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Deposit and withdrawal configuration and info
    /// </summary>
    public record CoinExDepositWithdrawalConfig
    {
        /// <summary>
        /// Asset information
        /// </summary>
        [JsonPropertyName("asset")]
        public CoinExDepositWithdrawalAsset Asset { get; set; } = null!;
        /// <summary>
        /// Available networks
        /// </summary>
        [JsonPropertyName("chains")]
        public IEnumerable<CoinExNetwork> Networks { get; set; } = null!;
    }

    /// <summary>
    /// Asset infos
    /// </summary>
    public record CoinExDepositWithdrawalAsset
    {
        /// <summary>
        /// Asset name
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Is deposit enabled
        /// </summary>
        [JsonPropertyName("deposit_enabled")]
        public bool DepositEnabled { get; set; }
        /// <summary>
        /// Is withdrawal enabled
        /// </summary>
        [JsonPropertyName("withdraw_enabled")]
        public bool WithdrawEnabled { get; set; }
        /// <summary>
        /// Is inter user transfer enabled
        /// </summary>
        [JsonPropertyName("inter_transfer_enabled")]
        public bool InterTransferEnabled { get; set; }
        /// <summary>
        /// Is st
        /// </summary>
        [JsonPropertyName("is_st")]
        public bool IsSt { get; set; }
    }

    /// <summary>
    /// Network info
    /// </summary>
    public record CoinExNetwork
    {
        /// <summary>
        /// Network name
        /// </summary>
        [JsonPropertyName("chain")]
        public string Network { get; set; } = string.Empty;
        /// <summary>
        /// Min deposit quantity
        /// </summary>
        [JsonPropertyName("min_deposit_amount")]
        public decimal MinDepositQuantity { get; set; }
        /// <summary>
        /// Min withdraw quantity
        /// </summary>
        [JsonPropertyName("min_withdraw_amount")]
        public decimal MinWithdrawQuantity { get; set; }
        /// <summary>
        /// Is deposit enabled
        /// </summary>
        [JsonPropertyName("deposit_enabled")]
        public bool DepositEnabled { get; set; }
        /// <summary>
        /// Is withdrawal enabled
        /// </summary>
        [JsonPropertyName("withdraw_enabled")]
        public bool WithdrawEnabled { get; set; }
        /// <summary>
        /// Number of confirmations needed
        /// </summary>
        [JsonPropertyName("safe_confirmations")]
        public int? SafeConfirmations { get; set; }
        /// <summary>
        /// Number of confirmations before transaction is irreversable
        /// </summary>
        [JsonPropertyName("irreversible_confirmations")]
        public int? IrreversableConfirmations { get; set; }
        /// <summary>
        /// Deflation rate
        /// </summary>
        [JsonPropertyName("deflation_rate")]
        public decimal? DeflationRate { get; set; }
        /// <summary>
        /// Withdrawal fee
        /// </summary>
        [JsonPropertyName("withdrawal_fee")]
        public decimal? WithdrawalFee { get; set; }
        /// <summary>
        /// Withdrawal precision
        /// </summary>
        [JsonPropertyName("withdrawal_precision")]
        public int? WithdrawalPrecision { get; set; }
        /// <summary>
        /// Memo
        /// </summary>
        [JsonPropertyName("memo")]
        public string Memo { get; set; } = string.Empty;
        /// <summary>
        /// Is memo required for deposits
        /// </summary>
        [JsonPropertyName("is_memo_required_for_deposit")]
        public bool MemoRequired { get; set; }
        /// <summary>
        /// Blockchain explorer url
        /// </summary>
        [JsonPropertyName("explorer_asset_url")]
        public string ExplorerUrl { get; set; } = string.Empty;
    }
}
