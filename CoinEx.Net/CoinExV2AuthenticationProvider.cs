using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CoinEx.Net.Objects.Internal;
using System.Linq;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;

namespace CoinEx.Net
{
    internal class CoinExV2AuthenticationProvider : AuthenticationProvider
    {
        public string GetApiKey() => _credentials.Key!.GetString();

        public CoinExV2AuthenticationProvider(ApiCredentials credentials): base(credentials)
        {
            if (credentials.CredentialType != ApiCredentialsType.Hmac)
                throw new Exception("Only Hmac authentication is supported");
        }

        public override void AuthenticateRequest(RestApiClient apiClient, Uri uri, HttpMethod method, Dictionary<string, object> providedParameters, bool auth, ArrayParametersSerialization arraySerialization, HttpMethodParameterPosition parameterPosition, RequestBodyFormat bodyFormat, out SortedDictionary<string, object> uriParameters, out SortedDictionary<string, object> bodyParameters, out Dictionary<string, string> headers)
        {
            uriParameters = parameterPosition == HttpMethodParameterPosition.InUri ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
            bodyParameters = parameterPosition == HttpMethodParameterPosition.InBody ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
            headers = new Dictionary<string, string>();

            if (!auth)
                return;

            var parameters = parameterPosition == HttpMethodParameterPosition.InUri ? uriParameters: bodyParameters;
            var parameterString = parameterPosition == HttpMethodParameterPosition.InUri ? (parameters.Any() ? "?" + parameters.CreateParamString(false, arraySerialization) : "") : new SystemTextJsonMessageSerializer().Serialize(parameters);
            var timestamp = GetMillisecondTimestamp(apiClient);
            var signData = method.ToString().ToUpperInvariant() + uri.AbsolutePath + parameterString + timestamp + _credentials.Secret!.GetString();
            var sign = SignSHA256(signData, SignOutputType.Hex);
            headers.Add("X-COINEX-KEY", _credentials.Key!.GetString());
            headers.Add("X-COINEX-SIGN", sign);
            headers.Add("X-COINEX-TIMESTAMP", timestamp);
        }

        public Dictionary<string, object> GetSocketAuthParameters()
        {
            var timestamp = CryptoExchange.Net.Converters.SystemTextJson.DateTimeConverter.ConvertToMilliseconds(DateTime.UtcNow);
            var signData = timestamp + _credentials.Secret!.GetString();
            var sign = SignSHA256(signData, SignOutputType.Hex);
            return new Dictionary<string, object>
            {
                { "access_id", _credentials.Key!.GetString() },
                { "signed_str", sign },
                { "timestamp", timestamp }
            };
        }
    }
}
