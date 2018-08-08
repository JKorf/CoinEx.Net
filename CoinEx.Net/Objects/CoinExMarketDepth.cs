using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects
{
    public class CoinExMarketDepth
    {
        public decimal Last { get; set; }
        public CoinExDepthEntry[] Asks { get; set; }
        public CoinExDepthEntry[] Bids { get; set; }
    }

    [JsonConverter(typeof(ArrayConverter))]
    public class CoinExDepthEntry
    {
        [ArrayProperty(0)]
        public decimal Price { get; set; }
        [ArrayProperty(1)]
        public decimal Amount { get; set; }
    }
}
