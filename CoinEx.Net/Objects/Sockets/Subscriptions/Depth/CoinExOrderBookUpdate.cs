using CoinEx.Net.Objects.Models.Socket;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.Depth
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class CoinExOrderBookUpdate
    {
        [ArrayProperty(0)]
        public bool Snapshot { get; set; }
        [ArrayProperty(1)]
        [JsonConversion]
        public CoinExSocketOrderBook Book { get; set; } = null!;
        [ArrayProperty(2)]
        public string Symbol { get; set; } = string.Empty;
    }
}
