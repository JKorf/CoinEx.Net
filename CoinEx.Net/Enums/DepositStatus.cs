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
        /// Currently processing
        /// </summary>
        [Map("processing")]
        Processing,
        /// <summary>
        /// Awaiting blockchain confirmation
        /// </summary>
        [Map("confirming")]
        Confirming,
        /// <summary>
        /// Canceled
        /// </summary>
        [Map("cancelled")]
        Canceled,
        /// <summary>
        /// Finished
        /// </summary>
        [Map("finished")]
        Finished,
        /// <summary>
        /// Deposit amount was too small
        /// </summary>
        [Map("too_small")]
        TooSmall,
        /// <summary>
        /// Exception
        /// </summary>
        [Map("exception")]
        Exception
    }
}
