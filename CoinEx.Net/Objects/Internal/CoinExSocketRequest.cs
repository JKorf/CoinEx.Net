using System;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Internal
{
    internal class CoinExSocketRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; } = string.Empty;
        [JsonIgnore]
        public string Subject { get; set; } = string.Empty;
        [JsonProperty("params")]
        public object[] Parameters { get; set; } = Array.Empty<object>();
        [JsonProperty("id")]
        public int Id { get; set; }

        public CoinExSocketRequest() { }

        public CoinExSocketRequest(int id, string subject, string action, params object[] parameters)
        {
            Id = id;
            Subject = subject;
            Method = $"{subject}.{action}";
            Parameters = parameters;
        }
    }
}
