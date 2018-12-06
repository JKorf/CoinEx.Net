using System;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net.Objects
{
    public class CoinExClientOptions: ClientOptions
    {
        /// <summary>
        /// The user agent send in all requests
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36";

        public CoinExClientOptions()
        {
            BaseAddress = "https://api.coinex.com/v1";
        }

        public CoinExClientOptions Copy()
        {
            var copy = Copy<CoinExClientOptions>();
            copy.UserAgent = UserAgent;
            return copy;
        }
    }

    public class CoinExSocketClientOptions : SocketClientOptions
    {
        /// <summary>
        /// The time to wait for the websocket to respond before assuming it failed
        /// </summary>
        public TimeSpan SubscriptionResponseTimeout { get; set; } = TimeSpan.FromSeconds(5);

        public CoinExSocketClientOptions()
        {
            BaseAddress = "wss://socket.coinex.com/";
        }

        public CoinExSocketClientOptions Copy()
        {
            var copy = Copy<CoinExSocketClientOptions>();
            copy.SubscriptionResponseTimeout = SubscriptionResponseTimeout;
            return copy;
        }
    }
}
