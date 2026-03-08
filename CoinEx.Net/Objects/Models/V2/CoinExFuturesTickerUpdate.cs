using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    [SerializationModel]
    internal record CoinExFuturesTickerUpdateWrapper
    {
        [JsonPropertyName("state_list")]
        public CoinExFuturesTickerUpdate[] Tickers { get; set; } = Array.Empty<CoinExFuturesTickerUpdate>();
    }

    /// <summary>
    /// Futures ticker update
    /// </summary>
    [SerializationModel]
    public record CoinExFuturesTickerUpdate : CoinExFuturesTicker
    {
        /// <summary>
        /// ["<c>open_interest_size</c>"] Open interest size
        /// </summary>
        [JsonPropertyName("open_interest_size")]
        public decimal OpenInterestSize { get; set; }

        /// <summary>
        /// ["<c>latest_funding_rate</c>"] Last funding rate
        /// </summary>
        [JsonPropertyName("latest_funding_rate")]
        public decimal LastFundingRate { get; set; }

        /// <summary>
        /// ["<c>next_funding_rate</c>"] Next funding rate
        /// </summary>
        [JsonPropertyName("next_funding_rate")]
        public decimal NextFundingRate { get; set; }
        /// <summary>
        /// ["<c>latest_funding_time</c>"] Last funding time
        /// </summary>
        [JsonPropertyName("latest_funding_time")]
        public DateTime? LastFundingTime { get; set; }
        /// <summary>
        /// ["<c>next_funding_time</c>"] Next funding time
        /// </summary>
        [JsonPropertyName("next_funding_time")]
        public DateTime? NextFundingTime { get; set; }
        /// <summary>
        /// ["<c>insurance_fund_size</c>"] Insurance fund size
        /// </summary>
        [JsonPropertyName("insurance_fund_size")]
        public decimal InsuranceFundSize { get; set; }
    }
}
