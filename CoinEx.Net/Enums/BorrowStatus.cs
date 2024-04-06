using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Borrow status
    /// </summary>
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
