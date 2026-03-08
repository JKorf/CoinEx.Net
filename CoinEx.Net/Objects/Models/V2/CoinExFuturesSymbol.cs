using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Text.Json.Serialization;
using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Futures symbol info
    /// </summary>
    [SerializationModel]
    public record CoinExFuturesSymbol
    {
        /// <summary>
        /// ["<c>market</c>"] Symbol name
        /// </summary>
        [JsonPropertyName("market")]
        public string Symbol { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>contract_type</c>"] Contract type
        /// </summary>
        [JsonPropertyName("contract_type")]
        public ContractType ContractType { get; set; }
        /// <summary>
        /// ["<c>taker_fee_rate</c>"] Taker fee rate
        /// </summary>
        [JsonPropertyName("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        /// <summary>
        /// ["<c>maker_fee_rate</c>"] Maker fee rate
        /// </summary>
        [JsonPropertyName("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }
        /// <summary>
        /// ["<c>min_amount</c>"] Min order quantity
        /// </summary>
        [JsonPropertyName("min_amount")]
        public decimal MinOrderQuantity { get; set; }
        /// <summary>
        /// ["<c>base_ccy</c>"] Base asset
        /// </summary>
        [JsonPropertyName("base_ccy")]
        public string BaseAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>quote_ccy</c>"] Quote asset
        /// </summary>
        [JsonPropertyName("quote_ccy")]
        public string QuoteAsset { get; set; } = string.Empty;
        /// <summary>
        /// ["<c>base_ccy_precision</c>"] Base asset precision
        /// </summary>
        [JsonPropertyName("base_ccy_precision")]
        public int QuantityPrecision { get; set; }
        /// <summary>
        /// ["<c>quote_ccy_precision</c>"] Quote asset precision
        /// </summary>
        [JsonPropertyName("quote_ccy_precision")]
        public int PricePrecision { get; set; }
        /// <summary>
        /// ["<c>leverage</c>"] Leverage
        /// </summary>
        [JsonPropertyName("leverage")]
        public int[] Leverage { get; set; } = Array.Empty<int>();
        /// <summary>
        /// ["<c>open_interest_volume</c>"] Open interest volume
        /// </summary>
        [JsonPropertyName("open_interest_volume")]
        public decimal OpenInterestVolume { get; set; }
        /// <summary>
        /// ["<c>is_market_available</c>"] Is trading available
        /// </summary>
        [JsonPropertyName("is_market_available")]
        public bool TradingAvailable { get; set; }
        /// <summary>
        /// ["<c>is_copy_trading_available</c>"] Is copy trading available
        /// </summary>
        [JsonPropertyName("is_copy_trading_available")]
        public bool CopyTradingAvailable { get; set; }
        /// <summary>
        /// ["<c>tick_size</c>"] Tick size
        /// </summary>
        [JsonPropertyName("tick_size")]
        public decimal TickSize { get; set; }
    }
}
