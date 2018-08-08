using CryptoExchange.Net;
using System;

namespace CoinEx.Net.Objects
{
    public class CoinExClientOptions: ExchangeOptions
    {
        public string BaseAddress { get; set; } = "https://api.coinex.com/v1";
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36";
    }

    public class CoinExSocketClientOptions : ExchangeOptions
    {
        public string BaseAddress { get; set; } = "wss://socket.coinex.com/";
        public TimeSpan SubscriptionResponseTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan ReconnectionInterval { get; set; } = TimeSpan.FromSeconds(2);
    }
}
