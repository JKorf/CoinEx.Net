using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Order status
    /// </summary>
    public enum OrderStatusV2
    {
        /// <summary>
        /// Open
        /// </summary>
        [Map("open")]
        Open,
        /// <summary>
        /// Partially filled
        /// </summary>
        [Map("part_filled")]
        PartiallyFilled,
        /// <summary>
        /// Fully filled
        /// </summary>
        [Map("filled")]
        Filled,
        /// <summary>
        /// Partially filled, partially canceled
        /// </summary>
        [Map("part_canceled")]
        PartiallyCanceled,
        /// <summary>
        /// Fully canceled
        /// </summary>
        [Map("canceled")]
        Canceled
    }
}
