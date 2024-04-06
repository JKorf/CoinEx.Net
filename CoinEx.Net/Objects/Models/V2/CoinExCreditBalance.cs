using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Credit account balance
    /// </summary>
    public record CoinExCreditBalance
    {
        /// <summary>
        /// Account assets
        /// </summary>
        [JsonPropertyName("equity")]
        public decimal Equity { get; set; }
        /// <summary>
        /// To be repaid
        /// </summary>
        [JsonPropertyName("repaid")]
        public decimal ToBeRepaid { get; set; }
        /// <summary>
        /// Current risk rate
        /// </summary>
        [JsonPropertyName("risk_rate")]
        public decimal RiskRate { get; set; }
        /// <summary>
        /// Withdrawal risk rate
        /// </summary>
        [JsonPropertyName("withdrawal_risk")]
        public decimal WithdrawalRiskRate { get; set; }
        /// <summary>
        /// Market value of available withdrawal
        /// </summary>
        [JsonPropertyName("withdrawal_value")]
        public decimal WithdrawalValueAvailable { get; set; }
    }
}
