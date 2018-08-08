using CoinEx.Net.Converters;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace CoinEx.Net.Objects
{
    public class CoinExWithdrawal
    {
        [JsonProperty("actual_amount"), JsonConverter(typeof(DecimalConverter))]
        public decimal ActualAmount { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Amount { get; set; }
        [JsonProperty("coin_address")]
        public string CoinAddress { get; set; }
        [JsonProperty("coin_type")]
        public string CoinType { get; set; }
        [JsonProperty("coin_withdraw_id")]
        public long CoinWithdrawalId { get; set; }
        public int Confirmations { get; set; }
        [JsonProperty("create_time"), JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime CreateTime { get; set; }
        [JsonConverter(typeof(WithdrawStatusConverter))]
        public WithdrawStatus Status { get; set; }
        [JsonProperty("tx_fee"), JsonConverter(typeof(DecimalConverter))]
        public decimal TransactionFee { get; set; }
        [JsonProperty("tx_id")]
        public string TransactionId { get; set; }
    }
}
