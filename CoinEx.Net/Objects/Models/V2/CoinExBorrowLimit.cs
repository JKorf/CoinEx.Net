using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Borrow limit info
    /// </summary>
    [SerializationModel]
    public record CoinExBorrowLimit
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>ccy</c>"] Asset
        /// </summary>
        [JsonPropertyName("ccy")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// ["<c>daily_interest_rate</c>"] Daily interest rate
        /// </summary>
        [JsonPropertyName("daily_interest_rate")]
        public decimal DailyInterestRate { get; set; }
        /// <summary>
        /// ["<c>leverage</c>"] Max leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public decimal Leverage { get; set; }
        /// <summary>
        /// ["<c>min_amount</c>"] Min amount borrowable
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinBorrowable { get; set; }
        /// <summary>
        /// ["<c>max_amount</c>"] Max amount borrowable
        /// </summary>
        [JsonPropertyName("max_amount")]
        public decimal MaxBorrowable { get; set; }
    }
}
