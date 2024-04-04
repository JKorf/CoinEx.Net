﻿using CoinEx.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    /// <summary>
    /// Order update
    /// </summary>
    public record CoinExOrderUpdate
    {
        /// <summary>
        /// Event that triggered the update
        /// </summary>
        [JsonPropertyName("event")]
        public OrderUpdateType Event { get; set; }
        /// <summary>
        /// Order data
        /// </summary>
        [JsonPropertyName("order")]
        public CoinExOrder Order { get; set; } = null!; // TODO check if model is indeed slightly different
    }
}