using System;
using System.Net.Http;
using CoinEx.Net.Interfaces;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Client options
    /// </summary>
    public class CoinExClientOptions: RestClientOptions
    {
        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        /// <summary>
        /// Create new client options
        /// </summary>
        public CoinExClientOptions() : this(null, "https://api.coinex.com/v1")
        {
        }

        /// <summary>
        /// Create new client options
        /// </summary>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public CoinExClientOptions(HttpClient client) : this(client, "https://api.coinex.com/v1")
        {
        }

        /// <summary>
        /// Create new client options
        /// </summary>
        /// <param name="apiAddress">Custom API address to use</param>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public CoinExClientOptions(HttpClient? client, string apiAddress) : base(apiAddress)
        {
            HttpClient = client;
        }
        
        /// <summary>
        /// Copy the options
        /// </summary>
        /// <returns></returns>
        public CoinExClientOptions Copy()
        {
            var copy = Copy<CoinExClientOptions>();
            copy.NonceProvider = NonceProvider;
            return copy;
        }
    }

    /// <summary>
    /// Socket client options
    /// </summary>
    public class CoinExSocketClientOptions : SocketClientOptions
    {
        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

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

        /// <summary>
        /// Copy the options
        /// </summary>
        /// <returns></returns>
        public CoinExSocketClientOptions Copy()
        {
            var copy = Copy<CoinExSocketClientOptions>();
            copy.NonceProvider = NonceProvider;
            return copy;
        }
    }

    /// <summary>
    /// Order book options
    /// </summary>
    public class CoinExOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public ICoinExSocketClient? SocketClient { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="client">The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.</param>
        public CoinExOrderBookOptions(ICoinExSocketClient? client = null) : base("CoinEx", false, false)
        {
            SocketClient = client;
        }
    }
}
