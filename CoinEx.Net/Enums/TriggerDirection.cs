using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

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
