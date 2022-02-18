using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Internal
{
    internal class CoinExSocketRequestResponse<T>
    {
        [JsonProperty("error")]
        public CoinExSocketError? Error { get; set; }

        [JsonProperty("result")] 
        public T Result { get; set; } = default!;
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
