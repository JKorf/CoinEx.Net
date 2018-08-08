using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExDepthSubscription: CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, bool, CoinExSocketMarketDepth> Handler { get; set; }
    }
}
