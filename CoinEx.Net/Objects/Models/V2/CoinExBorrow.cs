using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Borrow record
    /// </summary>
    [SerializationModel]
    public record CoinExBorrow
    {
        /// <summary>
        /// ["<c>borrow_id</c>"] Id
        /// </summary>
        [JsonPropertyName("borrow_id")]
        public long BorrowId { get; set; }
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
        /// ["<c>expired_at</c>"] Expire time
        /// </summary>
        [JsonPropertyName("expired_at")]
        public DateTime ExireTime { get; set; }
        /// <summary>
        /// ["<c>borrow_amount</c>"] Borrow amount
        /// </summary>
        [JsonPropertyName("borrow_amount")]
        public decimal BorrowQuantity { get; set; }
        /// <summary>
        /// ["<c>to_repaid_amount</c>"] Amount to repay
        /// </summary>
        [JsonPropertyName("to_repaid_amount")]
        public decimal ToRepayQuantity { get; set; }
        /// <summary>
        /// ["<c>status</c>"] Borrow status
        /// </summary>
        [JsonPropertyName("status")]
        public BorrowStatus Status { get; set; }
        /// <summary>
        /// ["<c>is_auto_renew</c>"] Is auto renewing
        /// </summary>
        [JsonPropertyName("is_auto_renew")]
        public bool? IsAutoRenew { get; set; }
    }
}
