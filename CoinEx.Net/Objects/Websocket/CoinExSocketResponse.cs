using Newtonsoft.Json;
using System;

namespace CoinEx.Net.Objects.Websocket
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
