using System;
using System.Text.Json.Serialization;
using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Position funding rate history
    /// </summary>
    public record CoinExPositionFundingRate
    {
        /// <summary>
        /// Position id
        /// </summary>
        [JsonPropertyName("position_id")]
        public long Id { get; set; }
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Account type
        /// </summary>
        [JsonPropertyName("market_type")]
        public AccountType AccountType { get; set; }
        /// <summary>
        /// Position side
        /// </summary>
        [JsonPropertyName("side")]
        public PositionSide Side { get; set; }
        /// <summary>
        /// Margin mode
        /// </summary>
        [JsonPropertyName("margin_mode")]
        public MarginMode MarginMode { get; set; }
        /// <summary>
        /// Open interest
        /// </summary>
        [JsonPropertyName("open_interest")]
        public decimal OpenInterest { get; set; }
        /// <summary>
        /// Settlement price
        /// </summary>
        [JsonPropertyName("settle_price")]
        public decimal SettlePrice { get; set; }
        /// <summary>
        /// Funding rate
        /// </summary>
        [JsonPropertyName("funding_rate")]
        public decimal FundingRate { get; set; }
        /// <summary>
        /// Funding value
        /// </summary>
        [JsonPropertyName("funding_value")]
        public decimal FundingValue { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
    }
}
