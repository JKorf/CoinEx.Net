﻿using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Deposit/Withdrawal method
    /// </summary>
    public enum MovementMethod
    {
        /// <summary>
        /// On chain
        /// </summary>
        [Map("on_chain")]
        OnChain,
        /// <summary>
        /// Between users
        /// </summary>
        [Map("inter_user")]
        InterUser
    }
}
