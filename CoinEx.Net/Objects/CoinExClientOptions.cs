using CryptoExchange.Net;
using System;

namespace CoinEx.Net.Objects
{
    public class CoinExClientOptions: ExchangeOptions
    {
        public CoinExClientOptions()
        {
            BaseAddress = "https://api.coinex.com/v1";
        }

        /// <summary>
        /// The user agent send in all requests
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36";
    }

    public class CoinExSocketClientOptions : ExchangeOptions
    {
        public CoinExSocketClientOptions()
        {
            BaseAddress = "wss://socket.coinex.com/";
        }

        /// <summary>
        /// The time to wait for the websocket to respond before assuming it failed
        /// </summary>
        public TimeSpan SubscriptionResponseTimeout { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// The time to wait after disconnecting before trying to reconnect again
        /// </summary>
        public TimeSpan ReconnectionInterval { get; set; } = TimeSpan.FromSeconds(2);
    }
}
