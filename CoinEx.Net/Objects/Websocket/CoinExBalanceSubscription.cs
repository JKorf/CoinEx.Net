using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExBalanceSubscription: CoinExSubscription
    {
        public Action<Dictionary<string, CoinExBalance>> Handler { get; set; }
    }
}
