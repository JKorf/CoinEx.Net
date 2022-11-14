using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CryptoExchange.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;
using CoinEx.Net.Enums;
using System.Threading;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Objects.Models.Socket;
using CryptoExchange.Net.Logging;
using CoinEx.Net.Interfaces.Clients.SpotApi;

namespace CoinEx.Net.Clients.SpotApi
{
    /// <inheritdoc cref="ICoinExSocketClientSpotStreams" />
    public class CoinExSocketClientSpotStreams : SocketApiClient, ICoinExSocketClientSpotStreams
    {
        #region fields
        private readonly CoinExSocketClientOptions _options;

        private const string ServerSubject = "server";
        private const string StateSubject = "state";
        private const string DepthSubject = "depth";
        private const string TransactionSubject = "deals";
        private const string KlineSubject = "kline";
        private const string BalanceSubject = "asset";
        private const string OrderSubject = "order";

        private const string SubscribeAction = "subscribe";
        private const string QueryAction = "query";
        private const string ServerTimeAction = "time";
        private const string PingAction = "ping";

        private const string AuthenticateAction = "sign";

        private const string SuccessString = "success";
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExSocketClient with default options
        /// </summary>
        public CoinExSocketClientSpotStreams(Log log, CoinExSocketClientOptions options)
            : base(log, options, options.SpotStreamsOptions)
        {
            _log = log;
            _options = options;

            AddGenericHandler("Pong", (messageEvent) => { });
            SendPeriodic("Ping", TimeSpan.FromMinutes(1), con => new CoinExSocketRequest(NextId(), ServerSubject, PingAction));
        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new CoinExAuthenticationProvider(credentials, _options.NonceProvider ?? new CoinExNonceProvider());

        #region methods
        #region public

        /// <inheritdoc />
        public async Task<CallResult<bool>> PingAsync()
        {
            var result = await QueryAsync<string>(new CoinExSocketRequest(NextId(), ServerSubject, PingAction), false).ConfigureAwait(false);
            return result.As(result.Success);
        }

        /// <inheritdoc />
        public async Task<CallResult<DateTime>> GetServerTimeAsync()
        {
            var result = await QueryAsync<long>(new CoinExSocketRequest(NextId(), ServerSubject, ServerTimeAction), false).ConfigureAwait(false);
            if (!result)
                return new CallResult<DateTime>(result.Error!);
            return new CallResult<DateTime>(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result.Data));
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketSymbolState>> GetTickerAsync(string symbol, int cyclePeriod)
        {
            symbol.ValidateCoinExSymbol();
            return await QueryAsync<CoinExSocketSymbolState>(new CoinExSocketRequest(NextId(), StateSubject, QueryAction, symbol, cyclePeriod), false).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketOrderBook>> GetOrderBookAsync(string symbol, int limit, int mergeDepth)
        {
            symbol.ValidateCoinExSymbol();
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit.ValidateIntValues(nameof(limit), 5, 10, 20);

            return await QueryAsync<CoinExSocketOrderBook>(new CoinExSocketRequest(NextId(), DepthSubject, QueryAction, symbol, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth)), false).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<IEnumerable<CoinExSocketSymbolTrade>>> GetTradeHistoryAsync(string symbol, int limit, int? fromId = null)
        {
            symbol.ValidateCoinExSymbol();

            return await QueryAsync<IEnumerable<CoinExSocketSymbolTrade>>(new CoinExSocketRequest(NextId(), TransactionSubject, QueryAction, symbol, limit, fromId ?? 0), false).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExKline>> GetKlinesAsync(string symbol, KlineInterval interval)
        {
            symbol.ValidateCoinExSymbol();

            return await QueryAsync<CoinExKline>(new CoinExSocketRequest(NextId(), KlineSubject, QueryAction, symbol, interval.ToSeconds()), false).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(IEnumerable<string> assets)
        {
            return await QueryAsync<Dictionary<string, CoinExBalance>>(new CoinExSocketRequest(NextId(), BalanceSubject, QueryAction, assets.ToArray()), true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOpenOrdersAsync(string symbol, OrderSide side, int offset, int limit)
        {
            symbol.ValidateCoinExSymbol();
            return await QueryAsync<CoinExSocketPagedResult<CoinExSocketOrder>>(
                new CoinExSocketRequest(NextId(), OrderSubject, QueryAction, symbol, int.Parse(JsonConvert.SerializeObject(side, new OrderSideIntConverter(false))), offset, limit), true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<CoinExSocketSymbolState>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                var desResult = Deserialize<Dictionary<string, CoinExSocketSymbolState>>(data.Data[0]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid state update: " + desResult.Error);
                    return;
                }
                var result = desResult.Data.First().Value;
                result.Symbol = symbol;

                onMessage(data.As(result, symbol));
            });

            return await SubscribeAsync(new CoinExSocketRequest(NextId(), StateSubject, SubscribeAction, symbol), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToAllTickerUpdatesAsync(Action<DataEvent<IEnumerable<CoinExSocketSymbolState>>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                var desResult = Deserialize<Dictionary<string, CoinExSocketSymbolState>>(data.Data[0]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid state update: " + desResult.Error);
                    return;
                }

                foreach (var item in desResult.Data)
                    item.Value.Symbol = item.Key;

                onMessage(data.As(desResult.Data.Select(d => d.Value)));
            });

            return await SubscribeAsync(new CoinExSocketRequest(NextId(), StateSubject, SubscribeAction), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int limit, int mergeDepth, Action<DataEvent<CoinExSocketOrderBook>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit.ValidateIntValues(nameof(limit), 5, 10, 20);

            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length != 3)
                {
                    _log.Write(LogLevel.Warning, $"Received unexpected data format for depth update. Expected 3 objects, received {data.Data.Length}. Data: " + data);
                    return;
                }

                var fullUpdate = (bool)data.Data[0];
                var desResult = Deserialize<CoinExSocketOrderBook>(data.Data[1]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid depth update: " + desResult.Error);
                    return;
                }

                desResult.Data.FullUpdate = fullUpdate;
                onMessage(data.As(desResult.Data, symbol));
            });

            return await SubscribeAsync(new CoinExSocketRequest(NextId(), DepthSubject, SubscribeAction, symbol, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth)), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<CoinExSocketSymbolTrade>>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length != 2 && data.Data.Length != 3)
                {
                    // Sometimes an extra True is send as 3rd parameter?
                    _log.Write(LogLevel.Warning, $"Received unexpected data format for trade update. Expected 2 objects, received {data.Data.Length}. Data: {data.OriginalData}");
                    return;
                }

                var desResult = Deserialize<IEnumerable<CoinExSocketSymbolTrade>>(data.Data[1]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid trade update: " + desResult.Error);
                    return;
                }

                onMessage(data.As(desResult.Data, symbol));
            });

            return await SubscribeAsync(new CoinExSocketRequest(NextId(), TransactionSubject, SubscribeAction, symbol), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<CoinExKline>>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length > 2)
                {
                    _log.Write(LogLevel.Warning, $"Received unexpected data format for kline update. Expected 1 or 2 objects, received {data.Data.Length}. Data: [{string.Join(",", data.Data.Select(s => s.ToString()))}]");
                    return;
                }

                var desResult = Deserialize<IEnumerable<CoinExKline>>(new JArray(data.Data));
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid kline update: " + desResult.Error);
                    return;
                }

                onMessage(data.As(desResult.Data, symbol));
            });

            return await SubscribeAsync(new CoinExSocketRequest(NextId(), KlineSubject, SubscribeAction, symbol, interval.ToSeconds()), null, false, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<IEnumerable<CoinExBalance>>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length != 1)
                {
                    if (data.Data.Length != 2 || data.Data.Length == 2 && data.Data[1].ToString().Trim() != "0")
                    {
                        _log.Write(LogLevel.Warning, $"Received unexpected data format for balance update. Expected 1 objects, received {data.Data.Length}. Data: [{string.Join(",", data.Data.Select(s => s.ToString()))}]");
                        return;
                    }
                }

                var desResult = Deserialize<Dictionary<string, CoinExBalance>>(data.Data[0]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid balance update: " + desResult.Error);
                    return;
                }

                foreach (var item in desResult.Data)
                    item.Value.Asset = item.Key;

                onMessage(data.As<IEnumerable<CoinExBalance>>(desResult.Data.Values, null));
            });

            return await SubscribeAsync(new CoinExSocketRequest(NextId(), BalanceSubject, SubscribeAction), null, true, internalHandler, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default)
            => SubscribeToOrderUpdatesAsync(Array.Empty<string>(), onMessage, ct);

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default)
        {
            var internalHandler = new Action<DataEvent<JToken[]>>(data =>
            {
                if (data.Data.Length != 2)
                {
                    _log.Write(LogLevel.Warning, $"Received unexpected data format for order update. Expected 2 objects, received {data.Data.Length}. Data: [{string.Join(",", data.Data.Select(s => s.ToString()))}]");
                    return;
                }

                var updateResult = JsonConvert.DeserializeObject<UpdateType>(data.Data[0].ToString(), new UpdateTypeConverter(false));
                var desResult = Deserialize<CoinExSocketOrder>(data.Data[1]);
                if (!desResult)
                {
                    _log.Write(LogLevel.Warning, "Received invalid order update: " + desResult.Error);
                    return;
                }

                var result = new CoinExSocketOrderUpdate()
                {
                    UpdateType = updateResult,
                    Order = desResult.Data
                };
                onMessage(data.As(result, result.Order.Symbol));
            });

            var request = new CoinExSocketRequest(NextId(), OrderSubject, SubscribeAction, symbols.ToArray());
            return await SubscribeAsync(request, null, true, internalHandler, ct).ConfigureAwait(false);
        }


        private object[] GetAuthParameters(BaseApiClient apiClient)
        {
            if (apiClient.AuthenticationProvider!.Credentials.Key == null || apiClient.AuthenticationProvider.Credentials.Secret == null)
                throw new ArgumentException("ApiKey/Secret not provided");

            var tonce = ((CoinExAuthenticationProvider)apiClient.AuthenticationProvider).GetNonce();
            var parameterString = $"access_id={apiClient.AuthenticationProvider.Credentials.Key.GetString()}&tonce={tonce}&secret_key={apiClient.AuthenticationProvider.Credentials.Secret.GetString()}";
            var auth = apiClient.AuthenticationProvider.Sign(parameterString);
            return new object[] { apiClient.AuthenticationProvider.Credentials.Key.GetString(), auth, tonce };
        }

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
                _log.Write(LogLevel.Warning, "Subscription failed: " + subResponse.Error);
                callResult = new CallResult<object>(subResponse.Error!);
                return true;
            }

            if (subResponse.Data.Error != null)
            {
                _log.Write(LogLevel.Debug, $"Failed to subscribe: {subResponse.Data.Error.Code} {subResponse.Data.Error.Message}");
                callResult = new CallResult<object>(new ServerError(subResponse.Data.Error.Code, subResponse.Data.Error.Message));
                return true;
            }

            _log.Write(LogLevel.Debug, "Subscription completed");
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
            await s.SendAndWaitAsync(request, Options.SocketResponseTimeout, data =>
            {
                var idField = data["id"];
                if (idField == null)
                    return false;

                if ((int)idField != request.Id)
                    return false; // Not for this request

                var authResponse = Deserialize<CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>>(data);
                if (!authResponse)
                {
                    _log.Write(LogLevel.Warning, "Authorization failed: " + authResponse.Error);
                    result = new CallResult<bool>(authResponse.Error!);
                    return true;
                }

                if (authResponse.Data.Error != null)
                {
                    var error = new ServerError(authResponse.Data.Error.Code, authResponse.Data.Error.Message);
                    _log.Write(LogLevel.Debug, "Failed to authenticate: " + error);
                    result = new CallResult<bool>(error);
                    return true;
                }

                if (authResponse.Data.Result.Status != SuccessString)
                {
                    _log.Write(LogLevel.Debug, "Failed to authenticate: " + authResponse.Data.Result.Status);
                    result = new CallResult<bool>(new ServerError(authResponse.Data.Result.Status));
                    return true;
                }

                _log.Write(LogLevel.Debug, "Authorization completed");
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
        #endregion

        #endregion
    }
}
