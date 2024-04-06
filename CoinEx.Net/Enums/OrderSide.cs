using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{   
    /// <summary>
    /// Order side
    /// </summary>
    public enum OrderSide
    {
        /// <summary>
        /// Either (only usable for filtering)
        /// </summary>
        Either,
        /// <summary>
        /// Buy
        /// </summary>
        [Map("buy")]
        Buy,
        /// <summary>
        /// Sell
        /// </summary>
        [Map("sell")]
        Sell
    }
}
