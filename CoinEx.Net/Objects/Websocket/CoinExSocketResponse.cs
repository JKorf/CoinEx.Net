using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketResponse
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("params")]
        public object[] Parameters { get; set; }
        [JsonProperty("id")]
        public int? Id { get; set; }
    }
}
