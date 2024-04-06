using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Trading fee info
    /// </summary>
    public record CoinExTradeFee
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Fee for maker trades
        /// </summary>
        [JsonPropertyName("maker_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// Fee for taker trades
        /// </summary>
        [JsonPropertyName("taker_rate")]
        public decimal TakerFeeRate { get; set; }
    }
}
