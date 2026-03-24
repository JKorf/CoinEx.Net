using CryptoExchange.Net.Authentication;
using System;

namespace CoinEx.Net
{
    /// <summary>
    /// CoinEx API credentials
    /// </summary>
    public class CoinExCredentials : HMACCredential
    {
        /// <summary>
        /// Create new credentials
        /// </summary>
        public CoinExCredentials() { }

        /// <summary>
        /// Create new credentials providing HMAC credentials
        /// </summary>
        /// <param name="credential">HMAC credentials</param>
        public CoinExCredentials(HMACCredential credential) : base(credential.Key, credential.Secret)
        {
        }

        /// <summary>
        /// Create new credentials providing only credentials in HMAC format
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="secret">API secret</param>
        public CoinExCredentials(string key, string secret) : base(key, secret)
        {
        }

        /// <summary>
        /// Specify the HMAC credentials
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="secret">API secret</param>
        public CoinExCredentials WithHMAC(string key, string secret)
        {
            if (!string.IsNullOrEmpty(Key)) throw new InvalidOperationException("Credentials already set");

            Key = key;
            Secret = secret;
            return this;
        }

        /// <inheritdoc />
        public override ApiCredentials Copy() => new CoinExCredentials(this);
    }
}
