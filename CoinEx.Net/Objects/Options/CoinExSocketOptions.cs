using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;

namespace CoinEx.Net.Objects.Options
{
    /// <summary>
    /// Options for the CoinExSocketClient
    /// </summary>
    public class CoinExSocketOptions : SocketExchangeOptions<CoinExEnvironment>
    {
        /// <summary>
        /// Default options for the CoinExRestClient
        /// </summary>
        public static CoinExSocketOptions Default { get; set; } = new CoinExSocketOptions
        {
            Environment = CoinExEnvironment.Live,
            SocketSubscriptionsCombineTarget = 10
        };

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        /// <summary>
        /// Options for the Spot API
        /// </summary>
        public SocketApiOptions SpotOptions { get; private set; } = new SocketApiOptions();

        /// <summary>
        /// Options for the Futures API
        /// </summary>
        public SocketApiOptions FuturesOptions { get; private set; } = new SocketApiOptions();

        internal CoinExSocketOptions Copy()
        {
            var options = Copy<CoinExSocketOptions>();
            options.NonceProvider = NonceProvider;
            options.SpotOptions = SpotOptions.Copy<SocketApiOptions>();
            options.FuturesOptions = SpotOptions.Copy<SocketApiOptions>();
            return options;
        }
    }
}
