using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    internal record CoinExTickerUpdateWrapper
    {
        [JsonPropertyName("state_list")]
        public IEnumerable<CoinExTicker> Tickers { get; set; } = Array.Empty<CoinExTicker>();

    }
}
