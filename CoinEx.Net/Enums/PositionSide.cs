﻿using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Position side
    /// </summary>
    public enum PositionSide
    {
        /// <summary>
        /// Long position
        /// </summary>
        [Map("long")]
        Long,
        /// <summary>
        /// Short position
        /// </summary>
        [Map("short")]
        Short
    }
}
