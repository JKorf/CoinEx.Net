using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Transaction type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TransactionType>))]
    public enum TransactionType
    {
        /// <summary>
        /// ["<c>deposit</c>"] Deposit
        /// </summary>
        [Map("deposit")]
        Deposit,
        /// <summary>
        /// ["<c>withdraw</c>"] Withdrawal
        /// </summary>
        [Map("withdraw")]
        Withdrawal,
        /// <summary>
        /// ["<c>trade</c>"] Trade
        /// </summary>
        [Map("trade")]
        Trade,
        /// <summary>
        /// ["<c>maker_cash_back</c>"] Maker cashback
        /// </summary>
        [Map("maker_cash_back")]
        MakerCashback
    }
}
