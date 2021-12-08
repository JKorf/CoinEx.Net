using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CoinEx.Net.Objects.Internal;

namespace CoinEx.Net
{
    internal class CoinExAuthenticationProvider : AuthenticationProvider
    {
        private readonly MD5 encryptor;
        private readonly INonceProvider _nonceProvider;

        public long GetNonce() => _nonceProvider.GetNonce();

        public CoinExAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider): base(credentials)
        {
            encryptor = MD5.Create();
            _nonceProvider = nonceProvider ?? new CoinExNonceProvider();
        }

        public override void AuthenticateUriRequest(RestApiClient apiClient, Uri uri, HttpMethod method, SortedDictionary<string, object> parameters, Dictionary<string, string> headers, bool auth, ArrayParametersSerialization arraySerialization)
        {
            parameters.Add("access_id", Credentials.Key!.GetString());
            parameters.Add("tonce", _nonceProvider.GetNonce());

            uri = uri.SetParameters(parameters);
            headers.Add("Authorization", SignMD5(uri.Query.Replace("?", "") + "&secret_key=" + Credentials.Secret!.GetString()));
        }

        public override void AuthenticateBodyRequest(RestApiClient apiClient, Uri uri, HttpMethod method, SortedDictionary<string, object> parameters, Dictionary<string, string> headers, bool auth, ArrayParametersSerialization arraySerialization)
        {
            parameters.Add("access_id", Credentials.Key!.GetString());
            parameters.Add("tonce", _nonceProvider.GetNonce());

            uri = uri.SetParameters(parameters);
            headers.Add("Authorization", SignMD5(uri.Query.Replace("?", "") + "&secret_key=" + Credentials.Secret!.GetString()));
        }

        public override string Sign(string toSign)
        {
            return SignMD5(toSign).ToUpper();
        }
    }
}
