using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Role of a transaction
    /// </summary>
    [JsonConverter(typeof(EnumConverter<TransactionRole>))]
    public enum TransactionRole
    {
        /// <summary>
        /// Maker of a new order book entry
        /// </summary>
        [Map("maker")]
        Maker,
        /// <summary>
        /// Taker of an existing order book entry
        /// </summary>
        [Map("taker")]
        Taker
    }
}
