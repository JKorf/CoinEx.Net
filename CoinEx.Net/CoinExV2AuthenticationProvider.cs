using CoinEx.Net.Objects.Sockets.V2.Queries;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CoinEx.Net
{
    internal class CoinExV2AuthenticationProvider : AuthenticationProvider
    {
        private static IStringMessageSerializer _serializer = new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(CoinExExchange._serializerContext));

        public override ApiCredentialsType[] SupportedCredentialTypes => [ApiCredentialsType.Hmac];
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
                parameterString = GetSerializedBody(_serializer, request.BodyParameters ?? new Dictionary<string, object>());
                request.SetBodyContent(parameterString);
            }

            var timestamp = GetMillisecondTimestamp(apiClient);
            var signData = request.Method.ToString().ToUpperInvariant() + request.Path + parameterString + timestamp;
            var sign = SignHMACSHA256(signData, SignOutputType.Hex);

            request.Headers ??= new Dictionary<string, string>();
            request.Headers.Add("X-COINEX-KEY", _credentials.Key);
            request.Headers.Add("X-COINEX-SIGN", sign);
            request.Headers.Add("X-COINEX-TIMESTAMP", timestamp);
        }

        public override Query? GetAuthenticationQuery(SocketApiClient apiClient, SocketConnection connection, Dictionary<string, object?>? context = null)
        {
            var timestamp = GetMillisecondTimestampLong(apiClient);
            var signData = timestamp.ToString();
            var sign = SignHMACSHA256(signData!, SignOutputType.Hex);
            var parameters = new Dictionary<string, object>
            {
                { "access_id", _credentials.Key },
                { "signed_str", sign },
                { "timestamp", timestamp }
            };

            return new CoinExQuery(apiClient, "server.sign", parameters, false, 0);
        }
    }
}
