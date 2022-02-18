namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Role of a transaction
    /// </summary>
    public enum TransactionRole
    {
        /// <summary>
        /// Maker of a new order book entry
        /// </summary>
        Maker,
        /// <summary>
        /// Taker of an existing order book entry
        /// </summary>
        Taker
    }
}
