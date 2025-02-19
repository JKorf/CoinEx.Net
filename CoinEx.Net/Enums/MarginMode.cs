using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Margin mode
    /// </summary>
    public enum MarginMode
    {
        /// <summary>
        /// Isolated margin mode
        /// </summary>
        [Map("isolated")]
        Isolated,
        /// <summary>
        /// Cross margin mode
        /// </summary>
        [Map("cross")]
        Cross
    }
}
