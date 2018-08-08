using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExKlineSubscription: CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExKline[]> Handler { get; set; }
    }
}
