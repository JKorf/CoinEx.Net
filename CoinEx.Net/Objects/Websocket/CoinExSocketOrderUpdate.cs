using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    /// <summary>
    /// Order update
    /// </summary>
    public class CoinExSocketOrderUpdate
    {
        /// <summary>
        /// The type of update
        /// </summary>
        public UpdateType UpdateType { get; set; }
        /// <summary>
        /// The order info
        /// </summary>
        public CoinExSocketOrder Order { get; set; }
    }
}
