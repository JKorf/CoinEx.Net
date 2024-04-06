using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    internal record CoinExServerTime
    {
        [JsonPropertyName("timestamp")]
        public DateTime ServerTime { get; set; }
    }
}
