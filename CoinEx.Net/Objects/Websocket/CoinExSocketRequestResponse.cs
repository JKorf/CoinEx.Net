using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Websocket
{
    internal class CoinExSocketRequestResponse<T>
    {
        [JsonProperty("error")]
        public CoinExSocketError Error { get; set; }
        [JsonProperty("result")]
        public T Result { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
