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
        /// ["<c>cm</c>"] Cancel the maker order
        /// </summary>
        [Map("cm")]
        CancelMaker,
        /// <summary>
        /// ["<c>ct</c>"] Cancel the taker order
        /// </summary>
        [Map("ct")]
        CancelTaker,
        /// <summary>
        /// ["<c>both</c>"] Cancel both orders
        /// </summary>
        [Map("both")]
        CancelBoth
    }
}
