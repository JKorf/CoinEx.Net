using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    [SerializationModel]
    internal record CoinExServerTime
    {
        [JsonPropertyName("timestamp")]
        public DateTime ServerTime { get; set; }
    }
}
