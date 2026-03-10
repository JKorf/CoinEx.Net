using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Position side
    /// </summary>
    [JsonConverter(typeof(EnumConverter<PositionSide>))]
    public enum PositionSide
    {
        /// <summary>
        /// ["<c>long</c>"] Long position
        /// </summary>
        [Map("long")]
        Long,
        /// <summary>
        /// ["<c>short</c>"] Short position
        /// </summary>
        [Map("short")]
        Short
    }
}
