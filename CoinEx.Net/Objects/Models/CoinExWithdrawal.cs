using System;
using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Models
{
    /// <summary>
    /// Withdrawal info
    /// </summary>
    public record CoinExWithdrawal
    {
        /// <summary>
        /// The actual quantity of the withdrawal, i.e. the quantity which will be transferred to the destination address
        /// </summary>
        [JsonProperty("actual_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal ActualQuantity { get; set; }
        /// <summary>
        /// The total quantity of the withdrawal
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        [JsonProperty("amount")]
        public decimal Quantity { get; set; }
        /// <summary>
        /// The destination address of the withdrawal
        /// </summary>
        [JsonProperty("coin_address")]
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// The name of the asset of the withdrawal
        /// </summary>
        [JsonProperty("coin_type")]
        public string CoinType { get; set; } = string.Empty;
        /// <summary>
        /// The id of this withdrawal
        /// </summary>
        [JsonProperty("coin_withdraw_id")]
        public long Id { get; set; }
        /// <summary>
        /// The current number of confirmations
        /// </summary>
        public int Confirmations { get; set; }
        /// <summary>
        /// The time the withdrawal was created
        /// </summary>
        [JsonProperty("create_time"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// The status of the withdrawal
        /// </summary>
        [JsonConverter(typeof(WithdrawStatusConverter))]
        public WithdrawStatus Status { get; set; }
        /// <summary>
        /// The fee for the withdrawal
        /// </summary>
        [JsonProperty("tx_fee"), JsonConverter(typeof(DecimalConverter))]
        public decimal TransactionFee { get; set; }
        /// <summary>
        /// The transaction id of the withdrawal
        /// </summary>
        [JsonProperty("tx_id")]
        public string TransactionId { get; set; } = string.Empty;
    }
}
