using CoinEx.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Stop order update
    /// </summary>
    public record CoinExStopOrderUpdate
    {
        /// <summary>
        /// Event that triggered the update
        /// </summary>
        [JsonPropertyName("event")]
        public StopOrderUpdateType Event { get; set; } // TODO CHeck if int or string return
        /// <summary>
        /// Order data
        /// </summary>
        [JsonPropertyName("order")]
        public CoinExStopOrder Order { get; set; } = null!; // TODO check if model is indeed slightly different
    }
}
