using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExStateSubscription: CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExMarketStatus> Handler { get; set; }
    }

    public class CoinExStateMultiSubscription : CoinExSubscription
    {
        public Action<Dictionary<string, CoinExMarketStatus>> Handler { get; set; }
    }
}
