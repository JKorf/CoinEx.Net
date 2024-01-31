using CoinEx.Net.Objects.Models;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.Balance
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class CoinExBalanceUpdate
    {
        [ArrayProperty(0)]
        [JsonConversion]
        public Dictionary<string, CoinExBalance> Balances { get; set; } = new Dictionary<string, CoinExBalance>();
    }
}
