using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketMarketDepth
    {
        public CoinExDepthEntry[] Asks { get; set; }
        public CoinExDepthEntry[] Bids { get; set; }
    }
}
