using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Position update type
    /// </summary>
    public enum PositionUpdateType
    {
        /// <summary>
        /// Update
        /// </summary>
        [Map("update")]
        Update,
        /// <summary>
        /// Close
        /// </summary>
        [Map("close")]
        Close,
        /// <summary>
        /// System closing
        /// </summary>
        [Map("sys_close")]
        SystemClose,
        /// <summary>
        /// Auto delveraging
        /// </summary>
        [Map("adl")]
        AutoDeleverage,
        /// <summary>
        /// Liquidation
        /// </summary>
        [Map("liq")]
        Liquidation,
        /// <summary>
        /// Alert
        /// </summary>
        [Map("alert")]
        Alert
    }
}
