using CoinEx.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    internal class CoinExSubscription
    {
        public int StreamId { get; set; }
    }

    internal class CoinExTransactionsSubscription : CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExSocketMarketTransaction[]> Handler { get; set; }
    }

    internal class CoinExStateSubscription : CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExSocketMarketState> Handler { get; set; }
    }

    internal class CoinExStateMultiSubscription : CoinExSubscription
    {
        public Action<Dictionary<string, CoinExSocketMarketState>> Handler { get; set; }
    }

    internal class CoinExDepthSubscription : CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, bool, CoinExSocketMarketDepth> Handler { get; set; }
    }

    internal class CoinExBalanceSubscription : CoinExSubscription
    {
        public Action<Dictionary<string, CoinExBalance>> Handler { get; set; }
    }

    internal class CoinExKlineSubscription : CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExKline[]> Handler { get; set; }
    }

    internal class CoinExOrderSubscription : CoinExSubscription
    {
        public Action<UpdateType, CoinExSocketOrder> Handler { get; set; }
    }
}
