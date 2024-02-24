using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Sockets
{
    internal class CoinExSocketRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; } = string.Empty;
        [JsonProperty("params")]
        public IEnumerable<object> Parameters { get; set; } = Array.Empty<object>();
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
