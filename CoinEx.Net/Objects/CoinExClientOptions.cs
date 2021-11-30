using System;
using System.Collections.Generic;
using CoinEx.Net.Interfaces.Clients.Socket;
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
        /// Default options for the spot client
        /// </summary>
        public static CoinExClientOptions Default { get; set; } = new CoinExClientOptions()
        {
            OptionsSpot = new RestSubClientOptions
            {
                BaseAddress = "https://api.coinex.com/v1",
                RateLimiters = new List<IRateLimiter>
                {
                    new RateLimiter()
                        .AddPartialEndpointLimit("/v1/order/", 100, TimeSpan.FromSeconds(10), countPerEndpoint: true)
                }
            }
        };

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        public RestSubClientOptions OptionsSpot { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public CoinExClientOptions()
        {
            if (Default == null)
                return;

            Copy(this, Default);
        }

        /// <summary>
        /// Copy the values of the def to the input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="def"></param>
        public new void Copy<T>(T input, T def) where T : CoinExClientOptions
        {
            base.Copy(input, def);

            input.NonceProvider = def.NonceProvider;

            input.OptionsSpot = new RestSubClientOptions();
            def.OptionsSpot.Copy(input.OptionsSpot, def.OptionsSpot);
        }
    }

    /// <summary>
    /// Socket client options
    /// </summary>
    public class CoinExSocketClientSpotOptions : SocketClientOptions
    {
        /// <summary>
        /// Default options for the spot client
        /// </summary>
        public static CoinExSocketClientSpotOptions Default { get; set; } = new CoinExSocketClientSpotOptions()
        {
            OptionsSpot = new SubClientOptions
            {
                BaseAddress = "wss://socket.coinex.com/"
            },
            SocketSubscriptionsCombineTarget = 1
        };

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

        public SubClientOptions OptionsSpot { get; set; }


        /// <summary>
        /// Ctor
        /// </summary>
        public CoinExSocketClientSpotOptions()
        {
            if (Default == null)
                return;

            Copy(this, Default);
        }

        /// <summary>
        /// Copy the values of the def to the input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="def"></param>
        public new void Copy<T>(T input, T def) where T : CoinExSocketClientSpotOptions
        {
            base.Copy(input, def);

            input.NonceProvider = def.NonceProvider;

            input.OptionsSpot = new SubClientOptions();
            def.OptionsSpot.Copy(input.OptionsSpot, def.OptionsSpot);
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
        public CoinExOrderBookOptions(ICoinExSocketClient? client = null)
        {
            SocketClient = client;
        }
    }
}
