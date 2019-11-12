using System;
using System.Collections.Generic;
using System.Text;
using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects
{ 
    /// <summary>
    /// Deposit info
    /// </summary>
    public class CoinExDeposit
    {
        /// <summary>
        /// The actual amount of the deposit
        /// </summary>
        [JsonProperty("actual_amount")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal ActualAmount { get; set; }

        /// <summary>
        /// The display for the deposit
        /// </summary>
        [JsonProperty("actual_amount_display")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal ActualAmountDisplay { get; set; }
        /// <summary>
        /// Depositor
        /// </summary>
        [JsonProperty("add_explorer")]
        public string AddExplorer { get; set; } = "";
        /// <summary>
        /// The total amount of the deposit
        /// </summary>
        [JsonProperty("amount")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        /// <summary>
        /// The display for the amount
        /// </summary>
        [JsonProperty("amount_display")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal AmountDisplay { get; set; }
        /// <summary>
        /// Deposit add displayed
        /// </summary>
        [JsonProperty("coin_address")]
        public string CoinAddress { get; set; } = "";
        /// <summary>
        /// Deposit add displayed
        /// </summary>
        [JsonProperty("coin_address_display")]
        public string CoinAddressDisplay { get; set; } = "";
        /// <summary>
        /// Deposit ID
        /// </summary>
        [JsonProperty("coin_deposit_id")]
        public long CoinDepositId { get; set; }
        /// <summary>
        /// Deposit ID
        /// </summary>
        [JsonProperty("coin_type")]
        public string CoinType { get; set; } = "";
        /// <summary>
        /// Deposit ID
        /// </summary>
        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }
        /// <summary>
        /// Deposit ID
        /// </summary>
        [JsonProperty("create_time")]
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Explorer
        /// </summary>
        [JsonProperty("explorer")]
        public string Explorer { get; set; } = "";
        /// <summary>
        /// Remarks
        /// </summary>
        [JsonProperty("remark")]
        public string Remark { get; set; } = "";
        /// <summary>
        /// Status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } = "";
        /// <summary>
        /// Status Displayed
        /// </summary>
        [JsonProperty("status_display")]
        public string StatusDisplay { get; set; } = "";
        /// <summary>
        /// transfer method
        /// </summary>
        [JsonProperty("transfer_method")]
        public string TransferMethod { get; set; } = "";
        /// <summary>
        /// The transaction id of the withdrawal
        /// </summary>
        [JsonProperty("tx_id")]
        public string TxId { get; set; } = "";
        /// <summary>
        /// The transaction id of the withdrawal
        /// </summary>
        [JsonProperty("tx_id_display")]
        public string TxIdDisplay { get; set; } = "";
    }
}
