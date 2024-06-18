using CoinEx.Net.Converters;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Symbol Info
    /// </summary>
    public record CoinExSymbol
    {
        /// <summary>
        /// The name of the symbol
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }= string.Empty;

        /// <summary>
        /// The minimum quantity that can be traded
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("min_amount")]
        public decimal MinQuantity { get; set; }

        /// <summary>
        /// The fee for the maker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("maker_fee_rate")]
        public decimal MakerFeeRate { get; set; }

        /// <summary>
        /// The fee for the taker
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("taker_fee_rate")]
        public decimal TakerFeeRate { get; set; }
        
        /// <summary>
        /// The asset being that is being traded against
        /// </summary>
        [JsonProperty("pricing_name")]
        public string PricingName { get; set; }= string.Empty;

        /// <summary>
        /// The number of decimals for the price
        /// </summary>
        [JsonConverter(typeof(IntConverter))]
        [JsonProperty("pricing_decimal")]
        public int PricingDecimal { get; set; }

        /// <summary>
        /// The asset being traded
        /// </summary>
        [JsonProperty("trading_name")]
        public string TradingName { get; set; } = string.Empty;
        
        /// <summary>
        /// The number of decimals for the price
        /// </summary>
        [JsonProperty("trading_decimal")]
        //[JsonConverter(typeof(IntConverter))]
        public int TradingDecimal { get; set; }
    }
}
