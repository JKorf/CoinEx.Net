using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using CryptoExchange.Net.Objects;
using System.Linq;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;

namespace CoinEx.Net
{
    internal class CoinExV2AuthenticationProvider : AuthenticationProvider
    {
        public CoinExV2AuthenticationProvider(ApiCredentials credentials): base(credentials)
        {
            if (credentials.CredentialType != ApiCredentialsType.Hmac)
                throw new Exception("Only Hmac authentication is supported");
        }

        public override void AuthenticateRequest(
            RestApiClient apiClient,
            Uri uri,
            HttpMethod method,
            ref IDictionary<string, object>? uriParameters,
            ref IDictionary<string, object>? bodyParameters,
            ref Dictionary<string, string>? headers,
            bool auth,
            ArrayParametersSerialization arraySerialization,
            HttpMethodParameterPosition parameterPosition,
            RequestBodyFormat requestBodyFormat)
        {
            if (!auth)
                return;

            IDictionary<string, object> parameters;
            if (parameterPosition == HttpMethodParameterPosition.InUri)
            {
                uriParameters ??= new Dictionary<string, object>();
                parameters = uriParameters;
            }
            else
            {
                bodyParameters ??= new Dictionary<string, object>();
                parameters = bodyParameters;
            }

            var parameterString = parameterPosition == HttpMethodParameterPosition.InUri ? (parameters.Any() ? "?" + parameters.CreateParamString(false, arraySerialization) : "") : new SystemTextJsonMessageSerializer().Serialize(parameters);
            var timestamp = GetMillisecondTimestamp(apiClient);
            var signData = method.ToString().ToUpperInvariant() + uri.AbsolutePath + parameterString + timestamp;
            var sign = SignHMACSHA256(signData, SignOutputType.Hex);
            headers ??= new Dictionary<string, string>();
            headers.Add("X-COINEX-KEY", _credentials.Key);
            headers.Add("X-COINEX-SIGN", sign);
            headers.Add("X-COINEX-TIMESTAMP", timestamp);
        }

        public Dictionary<string, object> GetSocketAuthParameters()
        {
            var timestamp = CryptoExchange.Net.Converters.SystemTextJson.DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow);
            var signData = timestamp.ToString();
            var sign = SignHMACSHA256(signData, SignOutputType.Hex);
            return new Dictionary<string, object>
            {
                { "access_id", _credentials.Key },
                { "signed_str", sign },
                { "timestamp", timestamp }
            };
        }
    }
}
