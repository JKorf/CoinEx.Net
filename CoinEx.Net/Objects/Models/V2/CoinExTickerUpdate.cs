using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    [SerializationModel]
    internal record CoinExTickerUpdateWrapper
    {
        [JsonPropertyName("state_list")]
        public CoinExTicker[] Tickers { get; set; } = Array.Empty<CoinExTicker>();

    }
}
