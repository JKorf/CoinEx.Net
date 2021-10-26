namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Status of a withdrawal
    /// </summary>
    public enum WithdrawStatus
    {
        /// <summary>
        /// Under audit
        /// </summary>
        Audit,
        /// <summary>
        /// Passed audit
        /// </summary>
        Pass,
        /// <summary>
        /// Processing
        /// </summary>
        Processing,
        /// <summary>
        /// Confirming
        /// </summary>
        Confirming,
        /// <summary>
        /// Not passed audit
        /// </summary>
        NotPass,
        /// <summary>
        /// Canceled
        /// </summary>
        Cancel,
        /// <summary>
        /// Finished
        /// </summary>
        Finish,
        /// <summary>
        /// Failed
        /// </summary>
        Fail
    }
}
