using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Sockets.V2
{
    internal class CoinExSocketResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
