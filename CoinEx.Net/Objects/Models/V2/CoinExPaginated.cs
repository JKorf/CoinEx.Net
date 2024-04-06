using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Paginated result
    /// </summary>
    public record CoinExPaginated<T>
    {
        /// <summary>
        /// Total results
        /// </summary>
        public int? Total { get; set; }
        /// <summary>
        /// Has next page
        /// </summary>
        public bool HasNext { get; set; }
        /// <summary>
        /// Page items
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
    }
}
