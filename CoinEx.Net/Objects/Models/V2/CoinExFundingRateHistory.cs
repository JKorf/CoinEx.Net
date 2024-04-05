using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Historic funding rate info
    /// </summary>
    public record CoinExFundingRateHistory
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Funding time
        /// </summary>
        [JsonPropertyName("funding_time")]
        public DateTime? FundingTime { get; set; }
        /// <summary>
        /// Theoretical funding rate. The theoretical funding rate to be collected for the current period after calculation
        /// </summary>
        [JsonPropertyName("theoretical_funding_rate")]
        public decimal TheoreticalFundingrate { get; set; }
        /// <summary>
        /// Actual funding rate. The actual funding rate charged in the current period
        /// </summary>
        [JsonPropertyName("actual_funding_rate")]
        public decimal ActualFundingRate { get; set; }
    }
}
