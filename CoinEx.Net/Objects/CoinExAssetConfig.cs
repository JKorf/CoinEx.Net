using Newtonsoft.Json;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Asset config
    /// </summary>
    public class CoinExAssetConfig
    {
        /// <summary>
        /// Asset code
        /// </summary>
        [JsonProperty("asset")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Withdraw and Deposit asset chain
        /// </summary>
        [JsonProperty("chain")]
        public string Chain { get; set; } = string.Empty;
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
        /// Minimal deposit amount
        /// </summary>
        [JsonProperty("deposit_least_amount")]
        public decimal MinDeposit { get; set; }
        /// <summary>
        /// Minimal withdrawal amount
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
