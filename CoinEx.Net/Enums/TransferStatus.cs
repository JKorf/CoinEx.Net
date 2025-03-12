using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Transfer status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TransferStatus>))]
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
