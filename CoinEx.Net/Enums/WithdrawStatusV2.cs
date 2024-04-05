using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Withdrawal status
    /// </summary>
    public enum WithdrawStatusV2
    {
        /// <summary>
        /// Created
        /// </summary>
        [Map("created")]
        Created,
        /// <summary>
        /// To be audited
        /// </summary>
        [Map("audit_required")]
        AuditRequired,
        /// <summary>
        /// Approved
        /// </summary>
        [Map("audited")]
        Audited,
        /// <summary>
        /// Procesing
        /// </summary>
        [Map("processing")]
        Processing,
        /// <summary>
        /// Waiting for blockchain confirmation
        /// </summary>
        [Map("confirming")]
        Confirming,
        /// <summary>
        /// Finished
        /// </summary>
        [Map("finished")]
        Finished,
        /// <summary>
        /// Withdrawal canceld
        /// </summary>
        [Map("cancelled")]
        Canceled,
        /// <summary>
        /// Cancelation failed
        /// </summary>
        [Map("cancellation_failed")]
        CancelationFailed,
        /// <summary>
        /// Withdrawal failed
        /// </summary>
        [Map("failed")]
        Failed
    }
}
