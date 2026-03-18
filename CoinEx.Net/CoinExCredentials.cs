using CryptoExchange.Net.Authentication;

namespace CoinEx.Net
{
    /// <summary>
    /// CoinEx API credentials
    /// </summary>
    public class CoinExCredentials : HMACCredential
    {
        /// <summary>
        /// Create new credentials providing only credentials in HMAC format
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="secret">API secret</param>
        public CoinExCredentials(string key, string secret) : base(key, secret)
        {
        }
    }
}
