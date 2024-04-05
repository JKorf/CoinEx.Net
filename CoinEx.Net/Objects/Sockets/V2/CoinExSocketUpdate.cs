using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Sockets.V2
{
    internal class CoinExSocketUpdate<T>
    {
        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
    }
}
