using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Position update type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<PositionUpdateType>))]
    public enum PositionUpdateType
    {
        /// <summary>
        /// ["<c>update</c>"] Update
        /// </summary>
        [Map("update")]
        Update,
        /// <summary>
        /// ["<c>close</c>"] Close
        /// </summary>
        [Map("close")]
        Close,
        /// <summary>
        /// ["<c>sys_close</c>"] System closing
        /// </summary>
        [Map("sys_close")]
        SystemClose,
        /// <summary>
        /// ["<c>adl</c>"] Auto delveraging
        /// </summary>
        [Map("adl")]
        AutoDeleverage,
        /// <summary>
        /// ["<c>liq</c>"] Liquidation
        /// </summary>
        [Map("liq")]
        Liquidation,
        /// <summary>
        /// ["<c>alert</c>"] Alert
        /// </summary>
        [Map("alert")]
        Alert
    }
}
