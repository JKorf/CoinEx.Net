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
    public class CoinExClientOptions: ClientOptions
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
    public class CoinExSocketClientOptions : ClientOptions
    {
        /// <summary>
        /// Default options for the spot client
        /// </summary>
        public static CoinExSocketClientOptions Default { get; set; } = new CoinExSocketClientOptions();

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        private SocketApiClientOptions _spotStreamsOptions = new SocketApiClientOptions(CoinExApiAddresses.Default.SocketClientAddress)
        {
            SocketSubscriptionsCombineTarget = 1
        };

        /// <summary>
        /// Spot stream options
        /// </summary>
        public SocketApiClientOptions SpotStreamsOptions
        {
            get => _spotStreamsOptions;
            set => _spotStreamsOptions = new SocketApiClientOptions(_spotStreamsOptions, value);
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
            _spotStreamsOptions = new SocketApiClientOptions(baseOn.SpotStreamsOptions, null);
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
