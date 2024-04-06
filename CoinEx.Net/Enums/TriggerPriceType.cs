using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Trigger price type
    /// </summary>
    public enum TriggerPriceType
    {
        /// <summary>
        /// Last price
        /// </summary>
        [Map("latest_price")]
        LastPrice,
        /// <summary>
        /// Mark price
        /// </summary>
        [Map("mark_price")]
        MarkPrice,
        /// <summary>
        /// Index price
        /// </summary>
        [Map("index_price")]
        IndexPrice
    }
}
