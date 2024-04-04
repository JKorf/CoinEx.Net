using CryptoExchange.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Type of contract
    /// </summary>
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
