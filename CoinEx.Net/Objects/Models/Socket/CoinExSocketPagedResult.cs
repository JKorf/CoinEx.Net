using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models.Socket
{
    /// <summary>
    /// Paged result
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public record CoinExSocketPagedResult<T>
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
        public IEnumerable<T> Data { get; set; } = Array.Empty<T>();
    }
}
