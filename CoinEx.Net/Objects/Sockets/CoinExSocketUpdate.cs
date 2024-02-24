using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Sockets
{
    internal class CoinExSocketUpdate<T>
    {
        [JsonProperty("method")]
        public string Method { get; set; } = string.Empty;
        [JsonProperty("params")]
        public T Data { get; set; } = default!;
    }
}
