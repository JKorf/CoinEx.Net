using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Deposit/Withdrawal method
    /// </summary>
    [JsonConverter(typeof(EnumConverter<MovementMethod>))]
    public enum MovementMethod
    {
        /// <summary>
        /// On chain
        /// </summary>
        [Map("on_chain")]
        OnChain,
        /// <summary>
        /// Between users
        /// </summary>
        [Map("inter_user")]
        InterUser
    }
}
