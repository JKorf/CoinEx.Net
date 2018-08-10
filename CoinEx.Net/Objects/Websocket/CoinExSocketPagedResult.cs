using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketPagedResult<T>
    {
        /// <summary>
        /// The number of results
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// The offset in the list
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// The total number of results
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// The data
        /// </summary>
        [JsonProperty("records")]
        public T[] Data { get; set; }
    }
}
