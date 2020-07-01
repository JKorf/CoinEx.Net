using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Paged result
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
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
        public IEnumerable<T> Data { get; set; } = new List<T>();
        /// <summary>
        /// Whether there is a next page
        /// </summary>
        [JsonProperty("has_next")]
        public bool HasNext { get; set; }
    }
}
