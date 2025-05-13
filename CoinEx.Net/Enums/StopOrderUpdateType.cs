using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Stop order update type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<StopOrderUpdateType>))]
    public enum StopOrderUpdateType
    {
        /// <summary>
        /// Order created
        /// </summary>
        [Map("put")]
        Put,
        /// <summary>
        /// Order active
        /// </summary>
        [Map("active")]
        Active,
        /// <summary>
        /// Order canceled
        /// </summary>
        [Map("cancel")]
        Cancel
    }
}
