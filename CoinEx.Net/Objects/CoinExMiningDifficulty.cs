using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects
{
    public class CoinExMiningDifficulty
    {
        public string Difficulty { get; set; }
        public string Prediction { get; set; }
        [JsonConverter(typeof(TimestampSecondsConverter))]
        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }
    }
}
