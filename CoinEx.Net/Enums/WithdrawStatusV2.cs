using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Withdrawal status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<WithdrawStatusV2>))]
    public enum WithdrawStatusV2
    {
        /// <summary>
        /// ["<c>created</c>"] Created
        /// </summary>
        [Map("created")]
        Created,
        /// <summary>
        /// ["<c>audit_required</c>"] To be audited
        /// </summary>
        [Map("audit_required")]
        AuditRequired,
        /// <summary>
        /// ["<c>audited</c>"] Approved
        /// </summary>
        [Map("audited")]
        Audited,
        /// <summary>
        /// ["<c>processing</c>"] Procesing
        /// </summary>
        [Map("processing")]
        Processing,
        /// <summary>
        /// ["<c>confirming</c>"] Waiting for blockchain confirmation
        /// </summary>
        [Map("confirming")]
        Confirming,
        /// <summary>
        /// ["<c>finished</c>"] Finished
        /// </summary>
        [Map("finished")]
        Finished,
        /// <summary>
        /// ["<c>cancelled</c>"] Withdrawal canceld
        /// </summary>
        [Map("cancelled")]
        Canceled,
        /// <summary>
        /// ["<c>cancellation_failed</c>"] Cancelation failed
        /// </summary>
        [Map("cancellation_failed")]
        CancelationFailed,
        /// <summary>
        /// ["<c>failed</c>"] Withdrawal failed
        /// </summary>
        [Map("failed")]
        Failed
    }
}
