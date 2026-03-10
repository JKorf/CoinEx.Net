using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Type of contract
    /// </summary>
    [JsonConverter(typeof(EnumConverter<ContractType>))]
    public enum ContractType
    {
        /// <summary>
        /// ["<c>linear</c>"] Linear contract
        /// </summary>
        [Map("linear")]
        Linear,
        /// <summary>
        /// ["<c>inverse</c>"] Inverse contract
        /// </summary>
        [Map("inverse")]
        Inverse
    }
}
