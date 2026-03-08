using CryptoExchange.Net.Converters.SystemTextJson;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Trading fee info
    /// </summary>
    [SerializationModel]
    public record CoinExTradeFee
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>maker_rate</c>"] Fee for maker trades
        /// </summary>
        [JsonPropertyName("maker_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// ["<c>taker_rate</c>"] Fee for taker trades
        /// </summary>
        [JsonPropertyName("taker_rate")]
        public decimal TakerFeeRate { get; set; }
    }
}
