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
        /// Linear contract
        /// </summary>
        [Map("linear")]
        Linear,
        /// <summary>
        /// Inverse contract
        /// </summary>
        [Map("inverse")]
        Inverse
    }
}
