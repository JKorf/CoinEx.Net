using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Deposit status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<DepositStatus>))]
    public enum DepositStatus
    {
        /// <summary>
        /// ["<c>processing</c>"] Currently processing
        /// </summary>
        [Map("processing")]
        Processing,
        /// <summary>
        /// ["<c>confirming</c>"] Awaiting blockchain confirmation
        /// </summary>
        [Map("confirming")]
        Confirming,
        /// <summary>
        /// ["<c>cancelled</c>"] Canceled
        /// </summary>
        [Map("cancelled")]
        Canceled,
        /// <summary>
        /// ["<c>finished</c>"] Finished
        /// </summary>
        [Map("finished")]
        Finished,
        /// <summary>
        /// ["<c>too_small</c>"] Deposit amount was too small
        /// </summary>
        [Map("too_small")]
        TooSmall,
        /// <summary>
        /// ["<c>exception</c>"] Exception
        /// </summary>
        [Map("exception")]
        Exception
    }
}
