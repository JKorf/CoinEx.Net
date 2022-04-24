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

        private RestApiClientOptions _spotApiOptions = new RestApiClientOptions(CoinExApiAddresses.Default.RestClientAddress)
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
            set => _spotApiOptions = new RestApiClientOptions(_spotApiOptions, value);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExClientOptions() : this(Default)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn">Base the new options on other options</param>
        internal CoinExClientOptions(CoinExClientOptions baseOn) : base(baseOn)
        {
            if (baseOn == null)
                return;

            NonceProvider = baseOn.NonceProvider;
            _spotApiOptions = new RestApiClientOptions(baseOn.SpotApiOptions, null);
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

        private ApiClientOptions _spotStreamsOptions = new ApiClientOptions(CoinExApiAddresses.Default.SocketClientAddress);
        /// <summary>
        /// Spot stream options
        /// </summary>
        public ApiClientOptions SpotStreamsOptions
        {
            get => _spotStreamsOptions;
            set => _spotStreamsOptions = new ApiClientOptions(_spotStreamsOptions, value);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExSocketClientOptions() : this(Default)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseOn">Base the new options on other options</param>
        internal CoinExSocketClientOptions(CoinExSocketClientOptions baseOn) : base(baseOn)
        {
            if (baseOn == null)
                return;

            NonceProvider = baseOn.NonceProvider;
            _spotStreamsOptions = new ApiClientOptions(baseOn.SpotStreamsOptions, null);
        }
    }

    /// <summary>
    /// Order book options
    /// </summary>
    public class CoinExOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// After how much time we should consider the connection dropped if no data is received for this time after the initial subscriptions
        /// </summary>
        public TimeSpan? InitialDataTimeout { get; set; }

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
