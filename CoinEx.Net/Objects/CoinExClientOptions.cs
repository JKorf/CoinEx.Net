using System;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Client options
    /// </summary>
    public class CoinExClientOptions: RestClientOptions
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CoinExClientOptions(): base("https://api.coinex.com/v1")
        {
        }
    }

    /// <summary>
    /// Socket client options
    /// </summary>
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

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExSocketClientOptions(): base("wss://socket.coinex.com/")
        {
        }
    }

    /// <summary>
    /// Order book options
    /// </summary>
    public class CoinExOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CoinExOrderBookOptions() : base("CoinEx", false, false)
        {
        }
    }
}
