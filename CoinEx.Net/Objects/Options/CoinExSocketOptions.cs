using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;

namespace CoinEx.Net.Objects.Options
{
    /// <summary>
    /// Options for the CoinExSocketClient
    /// </summary>
    public class CoinExSocketOptions : SocketExchangeOptions<CoinExEnvironment, CoinExCredentials>
    {
        /// <summary>
        /// Default options for the CoinExRestClient
        /// </summary>
        internal static CoinExSocketOptions Default { get; set; } = new CoinExSocketOptions
        {
            Environment = CoinExEnvironment.Live,
            SocketSubscriptionsCombineTarget = 10
        };

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExSocketOptions()
        {
            Default?.Set(this);
        }

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        /// <summary>
        /// Options for the Spot API
        /// </summary>
        public SocketApiOptions<CoinExCredentials> SpotOptions { get; private set; } = new SocketApiOptions<CoinExCredentials>();

        /// <summary>
        /// Options for the Futures API
        /// </summary>
        public SocketApiOptions<CoinExCredentials> FuturesOptions { get; private set; } = new SocketApiOptions<CoinExCredentials>();

        internal CoinExSocketOptions Set(CoinExSocketOptions targetOptions)
        {
            targetOptions = base.Set<CoinExSocketOptions>(targetOptions);
            targetOptions.NonceProvider = NonceProvider;
            targetOptions.SpotOptions = SpotOptions.Set(targetOptions.SpotOptions);
            targetOptions.FuturesOptions = FuturesOptions.Set(targetOptions.FuturesOptions);
            return targetOptions;
        }
    }
}
