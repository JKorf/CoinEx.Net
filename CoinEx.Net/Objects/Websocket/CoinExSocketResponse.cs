using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Websocket
{
    internal class CoinExSocketResponse
    {
        [JsonProperty("method")]
        public string Method { get; set; } = "";
        [JsonProperty("params")]
        public object[] Parameters { get; set; } = new object[0];
        [JsonProperty("id")]
        public int? Id { get; set; }
    }
}
