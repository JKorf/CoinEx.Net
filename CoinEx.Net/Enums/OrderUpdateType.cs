using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Order update type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderUpdateType>))]
    public enum OrderUpdateType
    {
        /// <summary>
        /// Order created
        /// </summary>
        [Map("put")]
        Put,
        /// <summary>
        /// Order updated
        /// </summary>
        [Map("update")]
        Update,
        /// <summary>
        /// Order was edited
        /// </summary>
        [Map("modify")]
        Edit,
        /// <summary>
        /// Order finished
        /// </summary>
        [Map("finish")]
        Finish,
    }
}
