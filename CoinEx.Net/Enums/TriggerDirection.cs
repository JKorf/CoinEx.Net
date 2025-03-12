using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Trigger direction
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TriggerDirection>))]
    public enum TriggerDirection
    {
        /// <summary>
        /// Should trigger when the price is higher than the trigger price
        /// </summary>
        [Map("higher")]
        Higher,
        /// <summary>
        /// Should trigger when the price is lower than the trigger price
        /// </summary>
        [Map("lower")]
        Lower
    }
}
