using System;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Deposit address
    /// </summary>
    public record CoinExDepositAddress
    {
        /// <summary>
        /// The address
        /// </summary>
        [JsonProperty("coin_address")]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// Time the address was created
        /// </summary>
        [JsonProperty("create_time")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Deposit address id
        /// </summary>
        [JsonProperty("deposit_address_id")]
        public int Id { get; set; }
        /// <summary>
        /// Is bitcoin cash
        /// </summary>
        [JsonProperty("is_bitcoin_cash")]
        public bool IsBitcoinCash { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
