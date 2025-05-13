using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Margin mode
    /// </summary>
    [JsonConverter(typeof(EnumConverter<MarginMode>))]
    public enum MarginMode
    {
        /// <summary>
        /// Isolated margin mode
        /// </summary>
        [Map("isolated")]
        Isolated,
        /// <summary>
        /// Cross margin mode
        /// </summary>
        [Map("cross")]
        Cross
    }
}
