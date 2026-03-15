using CryptoExchange.Net.Authentication;
using System;

namespace CoinEx.Net
{
    /// <summary>
    /// CoinEx credentials
    /// </summary>
    public class CoinExCredentials : ApiCredentials
    {
        /// <summary>
        /// </summary>
        [Obsolete("Parameterless constructor is only for deserialization purposes and should not be used directly. Use parameterized constructor instead.")]
        public CoinExCredentials() { }

        /// <summary>
        /// Create credentials using an HMAC key and secret.
        /// </summary>
        /// <param name="apiKey">The API key</param>
        /// <param name="secret">The API secret</param>
        public CoinExCredentials(string apiKey, string secret) : this(new HMACCredential(apiKey, secret)) { }

        /// <summary>
        /// Create CoinEx credentials using HMAC credentials
        /// </summary>
        /// <param name="credential">The HMAC credentials</param>
        public CoinExCredentials(HMACCredential credential) : base(credential) { }

        /// <inheritdoc />
#pragma warning disable CS0618 // Type or member is obsolete
        public override ApiCredentials Copy() => new CoinExCredentials { CredentialPairs = CredentialPairs };
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
