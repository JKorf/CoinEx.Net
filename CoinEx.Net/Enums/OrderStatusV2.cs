using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Order status
    /// </summary>
    [JsonConverter(typeof(EnumConverter<OrderStatusV2>))]
    public enum OrderStatusV2
    {
        /// <summary>
        /// ["<c>open</c>"] Open
        /// </summary>
        [Map("open")]
        Open,
        /// <summary>
        /// ["<c>part_filled</c>"] Partially filled
        /// </summary>
        [Map("part_filled")]
        PartiallyFilled,
        /// <summary>
        /// ["<c>filled</c>"] Fully filled
        /// </summary>
        [Map("filled")]
        Filled,
        /// <summary>
        /// ["<c>part_canceled</c>"] Partially filled, partially canceled
        /// </summary>
        [Map("part_canceled")]
        PartiallyCanceled,
        /// <summary>
        /// ["<c>canceled</c>"] Fully canceled
        /// </summary>
        [Map("canceled")]
        Canceled
    }
}
