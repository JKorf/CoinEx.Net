using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Stop order update type
    /// </summary>
    public enum StopOrderUpdateType
    {
        /// <summary>
        /// Order created
        /// </summary>
        [Map("put")]
        Put,
        /// <summary>
        /// Order active
        /// </summary>
        [Map("active")]
        Active,
        /// <summary>
        /// Order canceled
        /// </summary>
        [Map("cancel")]
        Cancel
    }
}
