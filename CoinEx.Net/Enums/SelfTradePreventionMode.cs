using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Self trade prevention mode
    /// </summary>
    [JsonConverter(typeof(EnumConverter<SelfTradePreventionMode>))]
    public enum SelfTradePreventionMode
    {
        /// <summary>
        /// Cancel the maker order
        /// </summary>
        [Map("cm")]
        CancelMaker,
        /// <summary>
        /// Cancel the taker order
        /// </summary>
        [Map("ct")]
        CancelTaker,
        /// <summary>
        /// Cancel both orders
        /// </summary>
        [Map("both")]
        CancelBoth
    }
}
