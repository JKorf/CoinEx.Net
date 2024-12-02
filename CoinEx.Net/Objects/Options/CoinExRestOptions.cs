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
        /// Whether to allow the client to adjust the clientOrderId parameter send by the user when placing orders to include a client reference. This reference is used by the exchange to allocate a small percentage of the paid trading fees to developer of this library. Defaults to false.<br />
        /// Note that:<br />
        /// * It does not impact the amount of fees a user pays in any way<br />
        /// * It does not impact functionality. The reference is added just before sending the request and removed again during data deserialization<br />
        /// * It does respect client order id field limitations. For example if the user provided client order id parameter is too long to fit the reference it will not be added<br />
        /// * Toggling this option might fail operations using a clientOrderId parameter for pre-existing orders which were placed before the toggle. Operations on orders placed after the toggle will work as expected. It's adviced to toggle when there are no open orders
        /// </summary>
        public bool AllowAppendingClientOrderId { get; set; } = false;

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

        internal CoinExRestOptions Set(CoinExRestOptions targetOptions)
        {
            targetOptions = base.Set<CoinExRestOptions>(targetOptions);
            targetOptions.AllowAppendingClientOrderId = AllowAppendingClientOrderId;
            targetOptions.NonceProvider = NonceProvider;
            targetOptions.SpotOptions = SpotOptions.Set(targetOptions.SpotOptions);
            targetOptions.FuturesOptions = FuturesOptions.Set(targetOptions.FuturesOptions);
            return targetOptions;
        }
    }
}
