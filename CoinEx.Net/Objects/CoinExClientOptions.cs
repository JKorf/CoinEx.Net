using System;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net.Objects
{
    public class CoinExClientOptions: RestClientOptions
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
        /// The amount of subscriptions that should be made on a single socket connection. Not all exchanges support multiple subscriptions on a single socket.
        /// Setting this to a higher number increases subscription speed, but having more subscriptions on a single connection will also increase the amount of traffic on that single connection.
        /// Not supported on CoinEx
        /// </summary>
        public new int? SocketSubscriptionsCombineTarget
        {
            get => 1;
            set
            {
                if (value != 1)
                    throw new ArgumentException("Can't change SocketSubscriptionsCombineTarget; server implementation does not allow multiple subscription on a socket");
            }
        }

        public CoinExSocketClientOptions()
        {
            BaseAddress = "wss://socket.coinex.com/";
        }
    }

    public class CoinExOrderBookOptions : OrderBookOptions
    {
        public CoinExOrderBookOptions() : base("CoinEx", false)
        {
        }
    }
}
