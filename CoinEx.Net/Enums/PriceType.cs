using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Price type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<PriceType>))]
    public enum PriceType
    {
        /// <summary>
        /// ["<c>latest_price</c>"] Last price
        /// </summary>
        [Map("latest_price")]
        LastPrice,
        /// <summary>
        /// ["<c>mark_price</c>"] Mark price
        /// </summary>
        [Map("mark_price")]
        MarkPrice,
        /// <summary>
        /// ["<c>index_price</c>"] Index price
        /// </summary>
        [Map("index_price")]
        IndexPrice
    }
}
