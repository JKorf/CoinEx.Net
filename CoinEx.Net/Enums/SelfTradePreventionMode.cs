using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Self trade prevention mode
    /// </summary>
    public enum SelfTradePreventionMode
    {
        /// <summary>
        /// Cancel the maker order
        /// </summary>
        [Map("cm")]
        CancelMaker,
        /// <summary>
        /// Cancel the taker order
        /// </summary>
        [Map("ct")]
        CancelTaker,
        /// <summary>
        /// Cancel both orders
        /// </summary>
        [Map("both")]
        CancelBoth
    }
}
