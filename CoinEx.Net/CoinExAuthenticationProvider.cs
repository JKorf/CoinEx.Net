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
    internal class CoinExAuthenticationProvider: AuthenticationProvider
    {
        private readonly MD5 encryptor;
        private readonly INonceProvider _nonceProvider;

        public long GetNonce() => _nonceProvider.GetNonce();

        public CoinExAuthenticationProvider(ApiCredentials credentials, INonceProvider? nonceProvider): base(credentials)
        {
            encryptor = MD5.Create();
            _nonceProvider = nonceProvider ?? new CoinExNonceProvider();
        }

        public override Dictionary<string, string> AddAuthenticationToHeaders(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization)
        {
            var result = new Dictionary<string, string>();
            if (!signed)
                return result;

            if(Credentials.Secret == null)
                throw new ArgumentException("ApiKey/secret not provided");

            var paramString = parameters.CreateParamString(true, ArrayParametersSerialization.MultipleValues);
            result.Add("Authorization", Sign(paramString + "&secret_key=" + Credentials.Secret.GetString()));
            return result;
        }

        public override Dictionary<string, object> AddAuthenticationToParameters(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization)
        {
            if (!signed)
                return parameters;

            if (Credentials.Key == null)
                throw new ArgumentException("ApiKey/secret not provided");

            parameters.Add("access_id", Credentials.Key.GetString());
            parameters.Add("tonce", _nonceProvider.GetNonce());
            parameters = parameters.OrderBy(p => p.Key).ToDictionary(k => k.Key, v => v.Value);
            return parameters;
        }

        public override string Sign(string toSign)
        {
            return BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(toSign))).Replace("-", string.Empty).ToUpper();
        }
    }
}
