using System;
using System.Text.Json.Serialization;
using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Borrow record
    /// </summary>
    public record CoinExBorrow
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonPropertyName("borrow_id")]
        public long BorrowId { get; set; }
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
        /// Expire time
        /// </summary>
        [JsonPropertyName("expired_at")]
        public DateTime ExireTime { get; set; }
        /// <summary>
        /// Borrow amount
        /// </summary>
        [JsonPropertyName("borrow_amount")]
        public decimal BorrowQuantity { get; set; }
        /// <summary>
        /// Amount to repay
        /// </summary>
        [JsonPropertyName("to_repaid_amount")]
        public decimal ToRepayQuantity { get; set; }
        /// <summary>
        /// Borrow status
        /// </summary>
        [JsonPropertyName("status")]
        public BorrowStatus Status { get; set; }
        /// <summary>
        /// Is auto renewing
        /// </summary>
        [JsonPropertyName("is_auto_renew")]
        public bool? IsAutoRenew { get; set; }
    }
}
