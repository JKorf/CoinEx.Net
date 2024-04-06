using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Transfer status
    /// </summary>
    public enum TransferStatus
    {
        /// <summary>
        /// Created
        /// </summary>
        [Map("created")]
        Created,
        /// <summary>
        /// Asset deducted
        /// </summary>
        [Map("deducted")]
        Deducted,
        /// <summary>
        /// Failed to transfer
        /// </summary>
        [Map("failed")]
        Failed,
        /// <summary>
        /// Transfer completed
        /// </summary>
        [Map("finished")]
        Finished
    }
}
