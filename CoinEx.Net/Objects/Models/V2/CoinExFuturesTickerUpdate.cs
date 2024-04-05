using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    internal record CoinExFuturesTickerUpdateWrapper
    {
        [JsonPropertyName("state_list")]
        public IEnumerable<CoinExFuturesTickerUpdate> Tickers { get; set; } = Array.Empty<CoinExFuturesTickerUpdate>();
    }

    /// <summary>
    /// Futures ticker update
    /// </summary>
    public record CoinExFuturesTickerUpdate : CoinExFuturesTicker
    {
        /// <summary>
        /// Open interest size
        /// </summary>
        [JsonPropertyName("open_interest_size")]
        public decimal OpenInterestSize { get; set; }

        /// <summary>
        /// Last funding rate
        /// </summary>
        [JsonPropertyName("latest_funding_rate")]
        public decimal LastFundingRate { get; set; }

        /// <summary>
        /// Next funding rate
        /// </summary>
        [JsonPropertyName("next_funding_rate")]
        public decimal NextFundingRate { get; set; }
        /// <summary>
        /// Last funding time
        /// </summary>
        [JsonPropertyName("latest_funding_time")]
        public DateTime? LastFundingTime { get; set; }
        /// <summary>
        /// Next funding time
        /// </summary>
        [JsonPropertyName("next_funding_time")]
        public DateTime? NextFundingTime { get; set; }
    }
}
