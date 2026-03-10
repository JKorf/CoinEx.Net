using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Order type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderTypeV2>))]
    public enum OrderTypeV2
    {
        /// <summary>
        /// ["<c>limit</c>"] Limit order
        /// </summary>
        [Map("limit")]
        Limit,
        /// <summary>
        /// ["<c>market</c>"] Market order
        /// </summary>
        [Map("market")]
        Market,
        /// <summary>
        /// ["<c>maker_only</c>"] Post only
        /// </summary>
        [Map("maker_only")]
        PostOnly,
        /// <summary>
        /// ["<c>ioc</c>"] Immediate or cancel
        /// </summary>
        [Map("ioc")]
        ImmediateOrCancel,
        /// <summary>
        /// ["<c>fok</c>"] Fill or kill
        /// </summary>
        [Map("fok")]
        FillOrKill
    }
}
