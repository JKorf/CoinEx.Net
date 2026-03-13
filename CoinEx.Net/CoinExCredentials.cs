using CryptoExchange.Net.Authentication;

namespace CoinEx.Net
{
    /// <summary>
    /// CoinEx credentials
    /// </summary>
    public class CoinExCredentials : ApiCredentials
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="apiKey">The API key</param>
        /// <param name="secret">The API secret</param>
        public CoinExCredentials(string apiKey, string secret) : this(new HMACCredential(apiKey, secret)) { }
       
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="credential">The HMAC credentials</param>
        public CoinExCredentials(HMACCredential credential) : base(credential) { }

        /// <inheritdoc />
        public override ApiCredentials Copy() => new CoinExCredentials(Hmac!);
    }
}
