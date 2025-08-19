using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CoinEx.Net
{
    internal class CoinExV2AuthenticationProvider : AuthenticationProvider
    {
        private static IStringMessageSerializer _serializer = new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(CoinExExchange._serializerContext));

        public CoinExV2AuthenticationProvider(ApiCredentials credentials): base(credentials)
        {
            if (credentials.CredentialType != ApiCredentialsType.Hmac)
                throw new Exception("Only Hmac authentication is supported");
        }

        public override void ProcessRequest(RestApiClient apiClient, RestRequestConfiguration request)
        {
            if (!request.Authenticated)
                return;

            string parameterString;
            if (request.ParameterPosition == HttpMethodParameterPosition.InUri)
            {
                parameterString = request.GetQueryString(false);
                if (!string.IsNullOrEmpty(parameterString))
                    parameterString= $"?{parameterString}";

                request.SetQueryString(parameterString);
            }
            else
            {
                parameterString = GetSerializedBody(_serializer, request.BodyParameters);
                request.SetBodyContent(parameterString);
            }

            var timestamp = GetMillisecondTimestamp(apiClient);
            var signData = request.Method.ToString().ToUpperInvariant() + request.Path + parameterString + timestamp;
            var sign = SignHMACSHA256(signData, SignOutputType.Hex);

            request.Headers.Add("X-COINEX-KEY", _credentials.Key);
            request.Headers.Add("X-COINEX-SIGN", sign);
            request.Headers.Add("X-COINEX-TIMESTAMP", timestamp);
        }

        public Dictionary<string, object> GetSocketAuthParameters()
        {
            var timestamp = DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow);
            var signData = timestamp.ToString();
            var sign = SignHMACSHA256(signData!, SignOutputType.Hex);
            return new Dictionary<string, object>
            {
                { "access_id", _credentials.Key },
                { "signed_str", sign },
                { "timestamp", timestamp }
            };
        }
    }
}
