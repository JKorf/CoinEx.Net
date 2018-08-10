using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CoinEx.Net.Objects.Websocket
{
    internal class CoinExSocketRequest
    {
        [JsonProperty("method")]
        public string Method { get; }
        [JsonIgnore]
        public string Subject { get; }
        [JsonIgnore]
        public string Action { get; }
        [JsonProperty("params")]
        public List<object> Parameters { get; }
        [JsonProperty("id")]
        public int Id { get; set; }

        public CoinExSocketRequest(string subject, string action, params object[] parameters)
        {
            Method = $"{subject}.{action}";
            Parameters = parameters.ToList();
        }
    }
}
