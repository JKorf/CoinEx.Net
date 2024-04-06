using CoinEx.Net.Enums;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Liquidation record
    /// </summary>
    public record CoinExLiquidation
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Position side
        /// </summary>
        [JsonPropertyName("side")]
        public PositionSide Side { get; set; }
        /// <summary>
        /// Liquidation price
        /// </summary>
        [JsonPropertyName("liq_price")]
        public decimal LiquidationPrice { get; set; }
        /// <summary>
        /// Liquidation quantity
        /// </summary>
        [JsonPropertyName("liq_amount")]
        public decimal LiquidationQuantity { get; set; }
        /// <summary>
        /// Bankruptcy price
        /// </summary>
        [JsonPropertyName("bkr_price")]
        public decimal BankruptcyPrice { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreateTime { get; set; }
    }
}
