using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketRequest
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
