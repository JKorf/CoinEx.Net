using CoinEx.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSubscription
    {
        public int StreamId { get; set; }
    }

    public class CoinExTransactionsSubscription : CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExSocketMarketTransaction[]> Handler { get; set; }
    }

    public class CoinExStateSubscription : CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExSocketMarketState> Handler { get; set; }
    }

    public class CoinExStateMultiSubscription : CoinExSubscription
    {
        public Action<Dictionary<string, CoinExSocketMarketState>> Handler { get; set; }
    }

    public class CoinExDepthSubscription : CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, bool, CoinExSocketMarketDepth> Handler { get; set; }
    }

    public class CoinExBalanceSubscription : CoinExSubscription
    {
        public Action<Dictionary<string, CoinExBalance>> Handler { get; set; }
    }

    public class CoinExKlineSubscription : CoinExSubscription
    {
        public string Market { get; set; }
        public Action<string, CoinExKline[]> Handler { get; set; }
    }

    public class CoinExOrderSubscription : CoinExSubscription
    {
        public Action<UpdateType, CoinExSocketOrder> Handler { get; set; }
    }
}
