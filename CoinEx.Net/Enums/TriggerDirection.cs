using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Trigger direction
    /// </summary>
    public enum TriggerDirection
    {
        /// <summary>
        /// Should trigger when the price is higher than the trigger price
        /// </summary>
        [Map("higher")]
        Higher,
        /// <summary>
        /// Should trigger when the price is lower than the trigger price
        /// </summary>
        [Map("lower")]
        Lower
    }
}
