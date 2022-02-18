using System;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Internal
{
    internal class CoinExSocketResponse
    {
        [JsonProperty("method")]
        public string Method { get; set; } = string.Empty;
        [JsonProperty("params")]
        public object[] Parameters { get; set; } = Array.Empty<object>();
        [JsonProperty("id")]
        public int? Id { get; set; }
    }
}
