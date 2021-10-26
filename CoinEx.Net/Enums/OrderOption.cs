namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Options when placing an order
    /// </summary>
    public enum OrderOption
    {
        /// <summary>
        /// Normal order
        /// </summary>
        Normal,
        /// <summary>
        /// Immediate or cancel order
        /// </summary>
        ImmediateOrCancel,
        /// <summary>
        /// Fill or kill order
        /// </summary>
        FillOrKill,
        /// <summary>
        /// Maker only order
        /// </summary>
        MakerOnly
    }
}
