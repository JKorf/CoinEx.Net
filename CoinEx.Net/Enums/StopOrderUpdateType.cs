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
        /// ["<c>put</c>"] Order created
        /// </summary>
        [Map("put")]
        Put,
        /// <summary>
        /// ["<c>active</c>"] Order active
        /// </summary>
        [Map("active")]
        Active,
        /// <summary>
        /// ["<c>cancel</c>"] Order canceled
        /// </summary>
        [Map("cancel")]
        Cancel
    }
}
