using Newtonsoft.Json;

namespace CoinEx.Net.Objects
{
    public class CoinExPagedResult<T>
    {
        public int Count { get; set; }
        [JsonProperty("curr_page")]
        public int CurrentPage { get; set; }
        public T Data { get; set; }
        [JsonProperty("has_next")]
        public bool HasNext { get; set; }
    }
}
