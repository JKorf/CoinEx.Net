using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Borrow status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<BorrowStatus>))]
    public enum BorrowStatus
    {
        /// <summary>
        /// Borrowing
        /// </summary>
        [Map("loan")]
        Loan,
        /// <summary>
        /// In debt
        /// </summary>
        [Map("debt")]
        Debt,
        /// <summary>
        /// Forcefully liquidated
        /// </summary>
        [Map("liquidated")]
        Liquidated,
        /// <summary>
        /// Has been repaid
        /// </summary>
        [Map("finish")]
        Finish
    }
}
