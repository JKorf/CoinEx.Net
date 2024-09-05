using System;
using System.Text.Json.Serialization;
using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Futures symbol info
    /// </summary>
    public record CoinExFuturesSymbol
    {
        /// <summary>
        /// Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// Contract type
        /// </summary>
        [JsonPropertyName("contract_type")]
        public ContractType ContractType { get; set; }
        /// <summary>
        /// Taker fee rate
        /// </summary>
        [JsonPropertyName("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// Maker fee rate
        /// </summary>
        [JsonPropertyName("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// Min order quantity
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinOrderQuantity { get; set; }
        /// <summary>
        /// Base asset
        /// </summary>
        [JsonPropertyName("base_ccy")]
        public string BaseAsset { get; set; } = string.Empty;
        /// <summary>
        /// Quote asset
        /// </summary>
        [JsonPropertyName("quote_ccy")]
        public string QuoteAsset { get; set; } = string.Empty;
        /// <summary>
        /// Base asset precision
        /// </summary>
        [JsonPropertyName("base_ccy_precision")]
        public int QuantityPrecision { get; set; }
        /// <summary>
        /// Quote asset precision
        /// </summary>
        [JsonPropertyName("quote_ccy_precision")]
        public int PricePrecision { get; set; }
        /// <summary>
        /// Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public int[] Leverage { get; set; } = Array.Empty<int>();
        /// <summary>
        /// Open interest volume
        /// </summary>
        [JsonPropertyName("open_interest_volume")]
        public decimal OpenInterestVolume { get; set; }
        /// <summary>
        /// Is trading available
        /// </summary>
        [JsonPropertyName("is_market_available")]
        public bool TradingAvailable { get; set; }
        /// <summary>
        /// Is copy trading available
        /// </summary>
        [JsonPropertyName("is_copy_trading_available")]
        public bool CopyTradingAvailable { get; set; }
    }
}
