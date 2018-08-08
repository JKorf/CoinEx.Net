using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExTransactionsSubscription: CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExSocketMarketTransaction[]> Handler { get; set; }
    }
}
