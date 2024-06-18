using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Asset config
    /// </summary>
    public record CoinExAssetConfig
    {
        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;
        /// <summary>
        /// Network
        /// </summary>
        [JsonProperty("chain")]
        public string Network { get; set; } = string.Empty;
        /// <summary>
        /// Deposit is enabled
        /// </summary>
        [JsonProperty("can_deposit")]
        public bool CanDeposit { get; set; }
        /// <summary>
        /// Withdraw is enabled
        /// </summary>
        [JsonProperty("can_withdraw")]
        public bool CanWithdraw { get; set; }
        /// <summary>
        /// Minimal deposit quantity
        /// </summary>
        [JsonProperty("deposit_least_amount")]
        public decimal MinDeposit { get; set; }
        /// <summary>
        /// Minimal withdrawal quantity
        /// </summary>
        [JsonProperty("withdraw_least_amount")]
        public decimal MinWithdraw { get; set; }
        /// <summary>
        /// Withdraw fee
        /// </summary>
        [JsonProperty("withdraw_tx_fee")]
        public decimal WithdrawFee { get; set; }
    }
}
