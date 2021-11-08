using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Enums;
using CoinEx.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net.Interfaces.Clients.Rest.Spot
{
    /// <summary>
    /// Interface for the CoinEx client
    /// </summary>
    public interface ICoinExClientSpot: IRestClient
    {
        ICoinExClientSpotAccount Account { get; }
        ICoinExClientSpotExchangeData ExchangeData { get; }
        ICoinExClientSpotTrading Trading { get; }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        /// <param name="nonceProvider">Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that</param>
        void SetApiCredentials(string apiKey, string apiSecret, INonceProvider? nonceProvider = null);
    }
}