using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Websocket
{
    internal class CoinExSocketRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("params")]
        public object[] Parameters { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }

        public CoinExSocketRequest() { }

        public CoinExSocketRequest(string subject, string action, params object[] parameters)
        {
            Method = $"{subject}.{action}";
            Parameters = parameters;
        }
    }
}
