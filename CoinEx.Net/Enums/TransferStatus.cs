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
        /// ["<c>created</c>"] Created
        /// </summary>
        [Map("created")]
        Created,
        /// <summary>
        /// ["<c>deducted</c>"] Asset deducted
        /// </summary>
        [Map("deducted")]
        Deducted,
        /// <summary>
        /// ["<c>failed</c>"] Failed to transfer
        /// </summary>
        [Map("failed")]
        Failed,
        /// <summary>
        /// ["<c>finished</c>"] Transfer completed
        /// </summary>
        [Map("finished")]
        Finished
    }
}
