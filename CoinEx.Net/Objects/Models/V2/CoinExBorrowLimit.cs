using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Borrow limit info
    /// </summary>
    public record CoinExBorrowLimit
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Asset
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Daily interest rate
        /// </summary>
        [JsonPropertyName("daily_interest_rate")]
        public decimal DailyInterestRate { get; set; }
        /// <summary>
        /// Max leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// Min amount borrowable
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinBorrowable { get; set; }
        /// <summary>
        /// Max amount borrowable
        /// </summary>
        [JsonPropertyName("max_amount")]
        public decimal MaxBorrowable { get; set; }
    }
}
