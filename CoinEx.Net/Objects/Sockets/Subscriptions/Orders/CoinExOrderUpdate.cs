using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using CoinEx.Net.Objects.Models.Socket;
using CryptoExchange.Net.Attributes;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.Orders
{
    [JsonConverter(typeof(ArrayConverter))]
    internal class CoinExOrderUpdate
    {
        [ArrayProperty(0)]
        [JsonConverter(typeof(UpdateTypeConverter))]
        public UpdateType UpdateType { get; set; }
        [ArrayProperty(1)]
        [JsonConversion]
        public CoinExSocketOrder Order { get; set; } = null!;
    }
}
