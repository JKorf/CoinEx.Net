using System;
using System.Collections.Generic;
using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Client options
    /// </summary>
    public class CoinExClientOptions: BaseRestClientOptions
    {
        /// <summary>
        /// Default options for the spot client
        /// </summary>
        public static CoinExClientOptions Default { get; set; } = new CoinExClientOptions();

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        private readonly RestApiClientOptions _spotApiOptions = new RestApiClientOptions(CoinExApiAddresses.Default.RestClientAddress)
        {
            RateLimiters = new List<IRateLimiter>
                {
                    new RateLimiter()
                        .AddPartialEndpointLimit("/v1/order/", 100, TimeSpan.FromSeconds(10), countPerEndpoint: true)
                }
        };
        /// <summary>
        /// Spot API options
        /// </summary>
        public RestApiClientOptions SpotApiOptions
        {
            get => _spotApiOptions;
            set => _spotApiOptions.Copy(_spotApiOptions, value);
        }

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
            input.SpotApiOptions = new RestApiClientOptions(def.SpotApiOptions);
        }
    }

    /// <summary>
    /// Socket client options
    /// </summary>
    public class CoinExSocketClientOptions : BaseSocketClientOptions
    {
        /// <summary>
        /// Default options for the spot client
        /// </summary>
        public static CoinExSocketClientOptions Default { get; set; } = new CoinExSocketClientOptions()
        {
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

        private readonly ApiClientOptions _spotStreamsOptions = new ApiClientOptions(CoinExApiAddresses.Default.SocketClientAddress);
        /// <summary>
        /// Spot stream options
        /// </summary>
        public ApiClientOptions SpotStreamsOptions
        {
            get => _spotStreamsOptions;
            set => _spotStreamsOptions.Copy(_spotStreamsOptions, value);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public CoinExSocketClientOptions()
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
        public new void Copy<T>(T input, T def) where T : CoinExSocketClientOptions
        {
            base.Copy(input, def);

            input.NonceProvider = def.NonceProvider;
            input.SpotStreamsOptions = new ApiClientOptions(def.SpotStreamsOptions);
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
        public ICoinExSocketClient? SocketClient { get; set; }

        /// <summary>
        /// The amount of rows. Should be one of: 5/10/20/50
        /// </summary>
        public int? Limit { get; set; }
    }
}
