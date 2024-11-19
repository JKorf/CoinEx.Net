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
        internal static CoinExRestOptions Default { get; set; } = new CoinExRestOptions
        {
            Environment = CoinExEnvironment.Live
        };

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExRestOptions()
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
        public RestApiOptions SpotOptions { get; private set; } = new RestApiOptions();

        /// <summary>
        /// Options for the Futures API
        /// </summary>
        public RestApiOptions FuturesOptions { get; private set; } = new RestApiOptions();

        /// <summary>
        /// The broker reference id to use
        /// </summary>
        public string? BrokerId { get; set; }

        internal CoinExRestOptions Set(CoinExRestOptions targetOptions)
        {
            targetOptions = base.Set<CoinExRestOptions>(targetOptions);
            targetOptions.BrokerId = BrokerId;
            targetOptions.NonceProvider = NonceProvider;
            targetOptions.SpotOptions = SpotOptions.Set(targetOptions.SpotOptions);
            targetOptions.FuturesOptions = FuturesOptions.Set(targetOptions.FuturesOptions);
            return targetOptions;
        }
    }
}
