using CoinEx.Net.Objects;
using CryptoExchange.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System.Threading;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.Clients.SpotApi;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc cref="ICoinExSocketClient" />
    public class CoinExSocketClient : BaseSocketClient, ICoinExSocketClient
    {
        #region fields
        private const string ServerSubject = "server";
        private const string PingAction = "ping";
        private const string AuthenticateAction = "sign";

        private const string SuccessString = "success";
        #endregion

        #region Api clients

        /// <inheritdoc />
        public ICoinExSocketClientSpotStreams SpotStreams { get; }

        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExSocketClient with default options
        /// </summary>
        public CoinExSocketClient() : this(CoinExSocketClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of CoinExSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public CoinExSocketClient(CoinExSocketClientOptions options) : base("CoinEx", options)
        {
            SpotStreams = AddApiClient(new CoinExSocketClientSpotStreams(log, this, options));

            AddGenericHandler("Pong", (messageEvent) => { });
            SendPeriodic("Ping", TimeSpan.FromMinutes(1), con => new CoinExSocketRequest(NextId(), ServerSubject, PingAction));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(CoinExSocketClientOptions options)
        {
            CoinExSocketClientOptions.Default = options;
        }

        #region methods

        private object[] GetAuthParameters(BaseApiClient apiClient)
        {
            if (apiClient.AuthenticationProvider!.Credentials.Key == null || apiClient.AuthenticationProvider.Credentials.Secret == null)
                throw new ArgumentException("ApiKey/Secret not provided");

            var tonce = ((CoinExAuthenticationProvider)apiClient.AuthenticationProvider).GetNonce();
            var parameterString = $"access_id={apiClient.AuthenticationProvider.Credentials.Key.GetString()}&tonce={tonce}&secret_key={apiClient.AuthenticationProvider.Credentials.Secret.GetString()}";
            var auth = apiClient.AuthenticationProvider.Sign(parameterString);
            return new object[] { apiClient.AuthenticationProvider.Credentials.Key.GetString(), auth, tonce };
        }

        internal int NextIdInternal()
        {
            return NextId();
        }

        internal virtual Task<CallResult<T>> QueryInternalAsync<T>(SocketApiClient apiClient, object request, bool authenticated)
        {
            return QueryAsync<T>(apiClient, request, authenticated);
        }

        internal virtual Task<CallResult<UpdateSubscription>> SubscribeInternalAsync<T>(SocketApiClient apiClient, object? request, string? identifier, bool authenticated, Action<DataEvent<T>> dataHandler, CancellationToken ct)
        {
            return SubscribeAsync(apiClient, request, identifier, authenticated, dataHandler, ct);
        }

        internal CallResult<T> DeserializeInternal<T>(JToken obj, JsonSerializer? serializer = null, int? requestId = null)
            => Deserialize<T>(obj, serializer, requestId);


        #endregion

        /// <inheritdoc />
        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T> callResult)
        {
            callResult = null!;
            var cRequest = (CoinExSocketRequest)request;
            var idField = data["id"];
            if (idField == null)
                return false;

            if ((int)idField != cRequest.Id)
                return false;

            var error = data["error"];
            if (error != null && error.Type != JTokenType.Null)
            {
                callResult = new CallResult<T>(new ServerError(error["code"]?.Value<int>() ?? 0, error["message"]?.ToString() ?? "Unknown error"));
                return true;
            }
            else
            {
                var result = data["result"];
                if (result == null)
                {
                    callResult = new CallResult<T>(new UnknownError("No data"));
                    return true;
                }

                var desResult = Deserialize<T>(result);
                if (!desResult)
                {
                    callResult = new CallResult<T>(desResult.Error!);
                    return true;
                }

                callResult = new CallResult<T>(desResult.Data);
                return true;
            }
        }

        /// <inheritdoc />
        protected override bool HandleSubscriptionResponse(SocketConnection s, SocketSubscription subscription, object request, JToken message, out CallResult<object>? callResult)
        {
            callResult = null;
            if (message.Type != JTokenType.Object)
                return false;

            var idField = message["id"];
            if (idField == null || idField.Type == JTokenType.Null)
                return false;

            var cRequest = (CoinExSocketRequest)request;
            if ((int)idField != cRequest.Id)
                return false;

            var subResponse = Deserialize<CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>>(message);
            if (!subResponse)
            {
                log.Write(LogLevel.Warning, "Subscription failed: " + subResponse.Error);
                callResult = new CallResult<object>(subResponse.Error!);
                return true;
            }

            if (subResponse.Data.Error != null)
            {
                log.Write(LogLevel.Debug, $"Failed to subscribe: {subResponse.Data.Error.Code} {subResponse.Data.Error.Message}");
                callResult = new CallResult<object>(new ServerError(subResponse.Data.Error.Code, subResponse.Data.Error.Message));
                return true;
            }

            log.Write(LogLevel.Debug, "Subscription completed");
            callResult = new CallResult<object>(subResponse);
            return true;
        }

        /// <inheritdoc />
        protected override JToken ProcessTokenData(JToken data)
        {
            return data["params"]!;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
        {
            var cRequest = (CoinExSocketRequest)request;
            var method = message["method"]?.ToString();
            if (method == null)
                return false;

            var subject = method.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            return cRequest.Subject == subject;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
        {
            if (message.Type != JTokenType.Object)
                return false;
            return identifier == "Pong" && message["result"]?.ToString() == "pong";
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection s)
        {
            if (s.ApiClient.AuthenticationProvider == null)
                return new CallResult<bool>(new NoApiCredentialsError());

            var request = new CoinExSocketRequest(NextId(), ServerSubject, AuthenticateAction, GetAuthParameters(s.ApiClient));
            var result = new CallResult<bool>(new ServerError("No response from server"));
            await s.SendAndWaitAsync(request, ClientOptions.SocketResponseTimeout, data =>
            {
                var idField = data["id"];
                if (idField == null)
                    return false;

                if ((int)idField != request.Id)
                    return false; // Not for this request

                var authResponse = Deserialize<CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>>(data);
                if (!authResponse)
                {
                    log.Write(LogLevel.Warning, "Authorization failed: " + authResponse.Error);
                    result = new CallResult<bool>(authResponse.Error!);
                    return true;
                }

                if (authResponse.Data.Error != null)
                {
                    var error = new ServerError(authResponse.Data.Error.Code, authResponse.Data.Error.Message);
                    log.Write(LogLevel.Debug, "Failed to authenticate: " + error);
                    result = new CallResult<bool>(error);
                    return true;
                }

                if (authResponse.Data.Result.Status != SuccessString)
                {
                    log.Write(LogLevel.Debug, "Failed to authenticate: " + authResponse.Data.Result.Status);
                    result = new CallResult<bool>(new ServerError(authResponse.Data.Result.Status));
                    return true;
                }

                log.Write(LogLevel.Debug, "Authorization completed");
                result = new CallResult<bool>(true);
                return true;
            }).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        protected override Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription s)
        {
            return Task.FromResult(true);
        }
    }
}
