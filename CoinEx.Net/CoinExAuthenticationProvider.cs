using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net
{
    internal class CoinExAuthenticationProvider: AuthenticationProvider
    {
        private readonly MD5 encryptor;


        public CoinExAuthenticationProvider(ApiCredentials credentials): base(credentials)
        {
            encryptor = MD5.Create();
        }

        public override Dictionary<string, string> AddAuthenticationToHeaders(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed)
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

        public override Dictionary<string, object> AddAuthenticationToParameters(string uri, HttpMethod method, Dictionary<string, object> parameters, bool signed)
        {
            if (!signed)
                return parameters;

            if (Credentials.Key == null)
                throw new ArgumentException("ApiKey/secret not provided");

            parameters.Add("access_id", Credentials.Key.GetString());
            parameters.Add("tonce", (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
            parameters = parameters.OrderBy(p => p.Key).ToDictionary(k => k.Key, v => v.Value);
            return parameters;
        }

        public override string Sign(string toSign)
        {
            return BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(toSign))).Replace("-", string.Empty).ToUpper();
        }
    }
}
