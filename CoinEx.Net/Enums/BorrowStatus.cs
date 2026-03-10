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
        /// ["<c>loan</c>"] Borrowing
        /// </summary>
        [Map("loan")]
        Loan,
        /// <summary>
        /// ["<c>debt</c>"] In debt
        /// </summary>
        [Map("debt")]
        Debt,
        /// <summary>
        /// ["<c>liquidated</c>"] Forcefully liquidated
        /// </summary>
        [Map("liquidated")]
        Liquidated,
        /// <summary>
        /// ["<c>finish</c>"] Has been repaid
        /// </summary>
        [Map("finish")]
        Finish
    }
}
