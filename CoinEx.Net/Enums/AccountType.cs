using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Account type
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// Spot account
        /// </summary>
        [Map("SPOT")]
        Spot,
        /// <summary>
        /// Margin account
        /// </summary>
        [Map("MARGIN")]
        Margin,
        /// <summary>
        /// Futures account
        /// </summary>
        [Map("FUTURES")]
        Futures
    }
}
