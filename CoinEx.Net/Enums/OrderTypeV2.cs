using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Order type
    /// </summary>
    public enum OrderTypeV2
    {
        /// <summary>
        /// Limit order
        /// </summary>
        [Map("limit")]
        Limit,
        /// <summary>
        /// Market order
        /// </summary>
        [Map("market")]
        Market,
        /// <summary>
        /// Post only
        /// </summary>
        [Map("maker_only")]
        PostOnly,
        /// <summary>
        /// Immediate or cancel
        /// </summary>
        [Map("ioc")]
        ImmediateOrCancel,
        /// <summary>
        /// Fill or kill
        /// </summary>
        [Map("fok")]
        FillOrKill
    }
}
