using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects.Options;

namespace CoinEx.Net.Objects.Options
{
    /// <summary>
    /// Options for the CoinExRestClient
    /// </summary>
    public class CoinExRestOptions : RestExchangeOptions<CoinExEnvironment>
    {
        /// <summary>
        /// Default options for the CoinExRestClient
        /// </summary>
        public static CoinExRestOptions Default { get; set; } = new CoinExRestOptions
        {
            Environment = CoinExEnvironment.Live
        };

        /// <summary>
        /// Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that
        /// </summary>
        public INonceProvider? NonceProvider { get; set; }

        /// <summary>
        /// Options for the Spot API
        /// </summary>
        public RestApiOptions SpotOptions { get; private set; } = new RestApiOptions();

        /// <summary>
        /// Options for the Futures API
        /// </summary>
        public RestApiOptions FuturesOptions { get; private set; } = new RestApiOptions();

        /// <summary>
        /// The broker reference id to use
        /// </summary>
        public string? BrokerId { get; set; }

        internal CoinExRestOptions Copy()
        {
            var options = Copy<CoinExRestOptions>();
            options.BrokerId = BrokerId;
            options.NonceProvider = NonceProvider;
            options.SpotOptions = SpotOptions.Copy<RestApiOptions>();
            options.FuturesOptions = SpotOptions.Copy<RestApiOptions>();
            return options;
        }
    }
}
