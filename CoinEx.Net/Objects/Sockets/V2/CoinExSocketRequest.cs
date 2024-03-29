using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Sockets.V2
{
    internal class CoinExSocketRequest
    {
        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;
        [JsonPropertyName("params")]
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
