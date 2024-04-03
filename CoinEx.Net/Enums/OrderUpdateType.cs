using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Order update type
    /// </summary>
    public enum OrderUpdateType
    {
        /// <summary>
        /// Order created
        /// </summary>
        [Map("put")]
        Put,
        /// <summary>
        /// Order updated
        /// </summary>
        [Map("update")]
        Update,
        /// <summary>
        /// Order finished
        /// </summary>
        [Map("finish")]
        Finish
    }
}
