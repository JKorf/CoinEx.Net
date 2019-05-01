using CryptoExchange.Net.Sockets;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Websocket
{
    internal class CoinExSocketRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonIgnore]
        public string Subject { get; set; }
        [JsonProperty("params")]
        public object[] Parameters { get; set; }
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
