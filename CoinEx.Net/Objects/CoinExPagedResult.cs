using Newtonsoft.Json;

namespace CoinEx.Net.Objects
{
    public class CoinExPagedResult<T>
    {
        /// <summary>
        /// The total number of results
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// The page currently returned
        /// </summary>
        [JsonProperty("curr_page")]
        public int CurrentPage { get; set; }
        /// <summary>
        /// The results
        /// </summary>
        public T[] Data { get; set; }
        /// <summary>
        /// Whether there is a next page
        /// </summary>
        [JsonProperty("has_next")]
        public bool HasNext { get; set; }
    }
}
