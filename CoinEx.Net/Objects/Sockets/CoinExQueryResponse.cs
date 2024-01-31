using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Sockets
{
    internal class CoinExQueryResponse<T>
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("error")]
        public CoinExQueryError? Error { get; set; }
        [JsonProperty("result")]
        public T Result { get; set; } = default!;
    }

    internal class CoinExQueryError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
