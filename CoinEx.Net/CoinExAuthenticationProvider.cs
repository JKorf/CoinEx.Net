using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CoinEx.Net.Objects.Internal;
using System.Linq;

namespace CoinEx.Net
{
    internal class CoinExAuthenticationProvider : AuthenticationProvider
    {
        private readonly INonceProvider _nonceProvider;

        public string GetApiKey() => _credentials.Key!.GetString();
        public long GetNonce() => _nonceProvider.GetNonce();

        public CoinExAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider): base(credentials)
        {
            if (credentials.CredentialType != ApiCredentialsType.Hmac)
                throw new Exception("Only Hmac authentication is supported");

            _nonceProvider = nonceProvider ?? new CoinExNonceProvider();
        }

        public override void AuthenticateRequest(RestApiClient apiClient, Uri uri, HttpMethod method, Dictionary<string, object> providedParameters, bool auth, ArrayParametersSerialization arraySerialization, HttpMethodParameterPosition parameterPosition, out SortedDictionary<string, object> uriParameters, out SortedDictionary<string, object> bodyParameters, out Dictionary<string, string> headers)
        {
            uriParameters = parameterPosition == HttpMethodParameterPosition.InUri ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
            bodyParameters = parameterPosition == HttpMethodParameterPosition.InBody ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
            headers = new Dictionary<string, string>();

            if (!auth)
                return;

            var parameters = parameterPosition == HttpMethodParameterPosition.InUri ? uriParameters: bodyParameters;
            parameters.Add("access_id", _credentials.Key!.GetString());
            parameters.Add("tonce", _nonceProvider.GetNonce());
            var parameterString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            headers.Add("Authorization", SignMD5(parameterString + "&secret_key=" + _credentials.Secret!.GetString()));
        }

        public string Sign(string toSign) => SignMD5(toSign).ToUpper();

        public object[] GetSocketAuthParameters()
        {
            var tonce = GetNonce();
            var parameterString = $"access_id={_credentials.Key!.GetString()}&tonce={tonce}&secret_key={_credentials.Secret!.GetString()}";
            var auth = SignMD5(parameterString).ToUpper();
            return new object[] { _credentials.Key!.GetString(), auth, tonce };
        }
    }
}
