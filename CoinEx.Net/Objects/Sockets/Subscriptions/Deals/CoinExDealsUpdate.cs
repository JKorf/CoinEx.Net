using CoinEx.Net.Objects.Models.Socket;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.Balance
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class CoinExDealsUpdate
    {
        [ArrayProperty(1)]
        [JsonConversion]
        public IEnumerable<CoinExSocketSymbolTrade> Trades { get; set; } = Array.Empty<CoinExSocketSymbolTrade>();
    }
}
