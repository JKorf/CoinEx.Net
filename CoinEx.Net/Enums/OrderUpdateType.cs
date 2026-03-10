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
        /// ["<c>put</c>"] Order created
        /// </summary>
        [Map("put")]
        Put,
        /// <summary>
        /// ["<c>update</c>"] Order updated
        /// </summary>
        [Map("update")]
        Update,
        /// <summary>
        /// ["<c>modify</c>"] Order was edited
        /// </summary>
        [Map("modify")]
        Edit,
        /// <summary>
        /// ["<c>finish</c>"] Order finished
        /// </summary>
        [Map("finish")]
        Finish,
    }
}
