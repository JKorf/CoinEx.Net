using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Account type
    /// </summary>
    [JsonConverter(typeof(EnumConverter<AccountType>))]
    public enum AccountType
    {
        /// <summary>
        /// ["<c>SPOT</c>"] Spot account
        /// </summary>
        [Map("SPOT")]
        Spot,
        /// <summary>
        /// ["<c>MARGIN</c>"] Margin account
        /// </summary>
        [Map("MARGIN")]
        Margin,
        /// <summary>
        /// ["<c>FUTURES</c>"] Futures account
        /// </summary>
        [Map("FUTURES")]
        Futures
    }
}
