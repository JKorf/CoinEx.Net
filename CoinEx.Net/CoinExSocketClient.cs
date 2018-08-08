using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Implementation;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net
{
    public class CoinExSocketClient: ExchangeClient
    {
        #region fields
        private static CoinExSocketClientOptions defaultOptions = new CoinExSocketClientOptions();

        private string baseAddress;
        private int subResponseTimeout;
        private int reconnectInterval;
        private SslProtocols protocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

        private int lastStreamId;
        private readonly object streamIdLock = new object();
        private readonly object connectionLock = new object();
        private List<CoinExStream> sockets = new List<CoinExStream>();

        private JsonSerializer marketDepthSerializer;
        private JsonSerializer marketTransactionSerializer;
        private JsonSerializer marketKlineSerializer;

        private List<CoinExSubscription> subscriptions = new List<CoinExSubscription>();
        #endregion

        #region properties
        public IWebsocketFactory SocketFactory { get; set; } = new WebsocketFactory();

        #endregion

        #region ctor
        public CoinExSocketClient() : this(defaultOptions)
        {
        }

        public CoinExSocketClient(CoinExSocketClientOptions options) : base(options, options.ApiCredentials == null ? null : new CoinExAuthenticationProvider(options.ApiCredentials))
        {
            Configure(options);

            marketDepthSerializer = new JsonSerializer();
            marketDepthSerializer.Converters.Add(new ParamConverter(new[] { typeof(bool), typeof(CoinExSocketMarketDepth), typeof(string) }));
            marketTransactionSerializer = new JsonSerializer();
            marketTransactionSerializer.Converters.Add(new ParamConverter(new[] { typeof(string), typeof(CoinExSocketMarketTransaction[]) }));
            marketKlineSerializer = new JsonSerializer();
            marketKlineSerializer.Converters.Add(new ParamListConverter(typeof(CoinExKline)));
        }
        #endregion

        #region methods
        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options"></param>
        public static void SetDefaultOptions(CoinExSocketClientOptions options)
        {
            defaultOptions = options;
        }

        public async Task<CallResult<bool>> PingAsync()
        {
            var result = await QueryNewSocket<string>(new CoinExSocketRequest("server", "ping"), false).ConfigureAwait(false);
            return new CallResult<bool>(result.Success, result.Error);
        }

        public async Task<CallResult<DateTime>> GetServerTimeAsync()
        {
            var result = await QueryNewSocket<long>(new CoinExSocketRequest("server", "time"), false).ConfigureAwait(false);
            if(!result.Success)
                return new CallResult<DateTime>(default(DateTime), result.Error);
            return new CallResult<DateTime>(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result.Data), null);
        }

        public async Task<CallResult<CoinExMarketStatus>> GetMarketStatusAsync(string market, int cyclePeriod)
        {
            return await QueryNewSocket<CoinExMarketStatus>(new CoinExSocketRequest("state", "query", market, cyclePeriod), false).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExSocketMarketDepth>> GetMarketDepthAsync(string market, int limit, int mergeDepth)
        {
            return await QueryNewSocket<CoinExSocketMarketDepth>(new CoinExSocketRequest("depth", "query", market, limit, "0.01"), false).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExSocketMarketTransaction[]>> GetMarketTransactionsAsync(string market, int limit, int? lastId = null)
        {
            return await QueryNewSocket<CoinExSocketMarketTransaction[]>(new CoinExSocketRequest("deals", "query", market, limit, lastId ?? 0), false).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExKline>> GetMarketKlinesAsync(string market, KlineInterval interval)
        {
            return await QueryNewSocket<CoinExKline>(new CoinExSocketRequest("kline", "query", market, 300), false).ConfigureAwait(false);
        }

        public async Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalances(params string[] markets)
        {
            return await QueryNewSocket<Dictionary<string, CoinExBalance>>(new CoinExSocketRequest("asset", "query", markets), true).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOrders(string market, TransactionType type, int offset, int limit)
        {
            return await QueryNewSocket<CoinExSocketPagedResult<CoinExSocketOrder>> (new CoinExSocketRequest("order", "query", market, 0, offset, limit), true).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketStateAsync(string market, Action<string, CoinExMarketStatus> onMessage)
        {
            var sub = new CoinExStateSubscription() { Market = market, Handler = onMessage };
            return await Subscribe(sub, new CoinExSocketRequest("state", "subscribe", market), false).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketStateAsync(Action<Dictionary<string, CoinExMarketStatus>> onMessage)
        {
            var sub = new CoinExStateMultiSubscription() { Handler = onMessage };
            return await Subscribe(sub, new CoinExSocketRequest("state", "subscribe"), false).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketDepthAsync(string market, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage)
        {
            var sub = new CoinExDepthSubscription() { Handler = onMessage, Market = market };
            return await Subscribe(sub, new CoinExSocketRequest("depth", "subscribe", market, limit, "0"), false).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketTransactionsAsync(string market, Action<string, CoinExSocketMarketTransaction[]> onMessage)
        {
            var sub = new CoinExTransactionsSubscription() { Handler = onMessage, Market = market };
            return await Subscribe(sub, new CoinExSocketRequest("deals", "subscribe", market), false).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketKlinesAsync(string market, KlineInterval interval, Action<string, CoinExKline[]> onMessage)
        {
            var sub = new CoinExKlineSubscription() { Handler = onMessage, Market = market };
            return await Subscribe(sub, new CoinExSocketRequest("kline", "subscribe", market, 60), false).ConfigureAwait(false);
        }

        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToBalancesAsync(Action<Dictionary<string, CoinExBalance>> onMessage)
        {
            var sub = new CoinExBalanceSubscription() { Handler = onMessage };
            return await Subscribe(sub, new CoinExSocketRequest("asset", "subscribe"), true).ConfigureAwait(false);
        }

        /// <summary>
        /// Unsubscribes from a stream
        /// </summary>
        /// <param name="streamSubscription">The stream subscription received by subscribing</param>
        public async Task UnsubscribeFromStream(CoinExStreamSubscription streamSubscription)
        {
            CoinExStream stream;
            lock (sockets) 
                stream = sockets.SingleOrDefault(s => s.StreamResult.StreamId == streamSubscription.StreamId);

            if (stream == null)
                return;            

            await stream.Close().ConfigureAwait(false);

        }

        /// <summary>
        /// Unsubscribes from all streams
        /// </summary>
        public async Task UnsubscribeAllStreams()
        {
            await Task.Run(() =>
            {
                lock (sockets)
                {
                    List<Task> closeTasks = new List<Task>();
                    foreach (var socket in sockets)
                        closeTasks.Add(socket.Close());
                    Task.WaitAll(closeTasks.ToArray());
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose this instance
        /// </summary>
        public override void Dispose()
        {
            log.Write(LogVerbosity.Info, "Disposing socket client, closing sockets");
            UnsubscribeAllStreams().Wait();
            base.Dispose();
        }

        private async Task<CallResult<bool>> Authenticate(CoinExStream stream)
        {
            var tonce = GetTimestamp();
            var parameterString = $"access_id={authProvider.Credentials.Key.GetString()}&tonce={tonce}&secret_key={authProvider.Credentials.Secret.GetString()}";
            var auth = authProvider.Sign(parameterString);
            object[] parameters = new object[] { authProvider.Credentials.Key.GetString(), auth, tonce };
            var result = await Query<CoinExSocketRequestResponseMessage>(stream.Socket, new CoinExSocketRequest("server", "sign", parameters));
            if (!result.Success)
                return new CallResult<bool>(false, result.Error);
            if(result.Data.Status != "success")
                return new CallResult<bool>(false, new ServerError(result.Data.Status));
            return new CallResult<bool>(true, null);
        }

        private long GetTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private async Task<CallResult<T>> QueryNewSocket<T>(CoinExSocketRequest request, bool authenticated)
        {
            var con = ConnectNewSocket();
            if (!con.Success)
                return new CallResult<T>(default(T), con.Error);

            var auth = await Authenticate(con.Data);
            if (!auth.Success)
                return new CallResult<T>(default(T), auth.Error);

            var result = await Query<T>(con.Data.Socket, request).ConfigureAwait(false);
            var closeTask = con.Data.Close().ConfigureAwait(false); // let it close in background
            return result;
        }

        private async Task<CallResult<T>> Query<T>(IWebsocket socket, CoinExSocketRequest request)
        {
            return await Task.Run(() =>
            {
                ManualResetEvent evnt = new ManualResetEvent(false);
                CallResult<CoinExSocketRequestResponse<T>> result = null;
                request.Id = NextStreamId();
                var onMessageAction = new Action<string>((msg) =>
                {
                    log.Write(LogVerbosity.Debug, $"Socket received query response: " + msg);

                    var token = JToken.Parse(msg);
                    if ((int?)token["id"] == request.Id)
                    {
                        if (token["error"].Type == JTokenType.Null)
                            result = Deserialize<CoinExSocketRequestResponse<T>>(msg, true);
                        else
                        {
                            var errorResult = Deserialize<CoinExSocketError>(token["error"].ToString());
                            if (!errorResult.Success)
                                result = new CallResult<CoinExSocketRequestResponse<T>>(null, new ServerError("Unknown error: " + token["error"]));
                            else
                                result = new CallResult<CoinExSocketRequestResponse<T>>(null, new ServerError(errorResult.Data.Code, errorResult.Data.Message));
                        }

                        evnt.Set();
                    }
                });

                socket.OnMessage += onMessageAction;
                var data = JsonConvert.SerializeObject(request);
                log.Write(LogVerbosity.Debug, "Sending data: " + data);
                socket.Send(data);
                evnt.WaitOne(subResponseTimeout);
                socket.OnMessage -= onMessageAction;
                evnt.Dispose();
                if (result == null)
                    return new CallResult<T>(default(T), new ServerError("No response from server"));
                if (!result.Success)
                    return new CallResult<T>(default(T), result.Error);
                if (result.Data.Error != null)
                    return new CallResult<T>(default(T), new ServerError(result.Data.Error.Code, result.Data.Error.Message));

                return new CallResult<T>(result.Data.Result, null);
            }).ConfigureAwait(false);
        }

        private async Task<CallResult<CoinExStreamSubscription>> Subscribe(CoinExSubscription subscription, CoinExSocketRequest request, bool authenticated)
        {
            var con = ConnectNewSocket();
            if (!con.Success)
                return new CallResult<CoinExStreamSubscription>(null, con.Error);

            var auth = await Authenticate(con.Data);
            if (!auth.Success)
                return new CallResult<CoinExStreamSubscription>(null, auth.Error);

            var subConfirm = await Query<CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>>(con.Data.Socket, request).ConfigureAwait(false);
            con.Data.Socket.OnMessage += (msg) => OnMessage(con.Data.StreamResult.StreamId, msg);
            if (subConfirm.Success)
            {
                subscription.StreamId = con.Data.StreamResult.StreamId;
                subscriptions.Add(subscription);
                log.Write(LogVerbosity.Info, $"Subscription {subscription.StreamId} successful");
            }
            else
            {
                log.Write(LogVerbosity.Info, $"Failed to subscribe {subscription.StreamId}: {subConfirm.Error}");
                await con.Data.Close().ConfigureAwait(false);
            }

            return new CallResult<CoinExStreamSubscription>(con.Data.StreamResult, subConfirm.Error);
        }

        private void OnMessage(int streamId, string data)
        {
            log.Write(LogVerbosity.Debug, $"Socket {streamId} received data: " + data);           

            var token = JToken.Parse(data);
            var id = (int?)token["id"];
            if (id != null)
                // If id != null it's a response to a query, dont handle here
                return;

            if (!subscriptions.Any(s => s.StreamId == streamId))
            {
                log.Write(LogVerbosity.Warning, $"Socket {streamId} received data for unknown subscription: " + data);
                return;
            }

            if (token["method"] == null)
                return;

            var subjectAction = ((string)token["method"]).Split('.');
            if (subjectAction[0] == "deals")
                HandleTransactionUpdate(streamId, token);
            else if (subjectAction[0] == "state")
                HandleStateUpdate(streamId, token);
            else if (subjectAction[0] == "depth")
                HandleDepthUpdate(streamId, token);
            else if (subjectAction[0] == "kline")
                HandleKlineUpdate(streamId, token);
            else if (subjectAction[0] == "asset")
                HandleBalanceUpdate(streamId, token);
            else
                log.Write(LogVerbosity.Warning, $"Socket {streamId} received unknown data: " + data);
        }

        private void HandleStateUpdate(int streamId, JToken token)
        {
            var paramData = Deserialize<Dictionary<string, CoinExMarketStatus>>(token["params"].First.ToString(), false);
            if (!paramData.Success)
            {
                log.Write(LogVerbosity.Warning, "Failed to deserialize state data: " + paramData.Error);
                return;
            }
            foreach (var handler in subscriptions.Where(s => s.StreamId == streamId).OfType<CoinExStateMultiSubscription>())
                handler.Handler(paramData.Data);

            if (paramData.Data.Count == 1) {
                var first = paramData.Data.First();
                foreach (var handler in subscriptions.Where(s => s.StreamId == streamId).OfType<CoinExStateSubscription>().Where(s => s.Market == first.Key))
                    handler.Handler(first.Key, first.Value);
            }
        }

        private void HandleTransactionUpdate(int streamId, JToken token)
        {
            var paramData = Deserialize<object[]>(token["params"].ToString(), false, marketTransactionSerializer);
            if (!paramData.Success)
            {
                log.Write(LogVerbosity.Warning, "Failed to deserialize transaction data: " + paramData.Error);
                return;
            }

            string market = (string)paramData.Data[0];
            foreach (var handler in subscriptions.Where(s => s.StreamId == streamId).OfType<CoinExTransactionsSubscription>().Where(s => s.Market == market))
                handler.Handler(market, (CoinExSocketMarketTransaction[])paramData.Data[1]);
        }

        private void HandleDepthUpdate(int streamId, JToken token)
        {
            var paramData = Deserialize<object[]>(token["params"].ToString(), false, marketDepthSerializer);
            if (!paramData.Success)
            {
                log.Write(LogVerbosity.Warning, "Failed to deserialize depth data: " + paramData.Error);
                return;
            }

            foreach (var handler in subscriptions.Where(s => s.StreamId == streamId).OfType<CoinExDepthSubscription>())
                handler.Handler((string)paramData.Data[2], (bool)paramData.Data[0], (CoinExSocketMarketDepth)paramData.Data[1]);
        }

        private void HandleKlineUpdate(int streamId, JToken token)
        {
            var paramData = Deserialize<object[]>(token["params"].ToString(), false, marketKlineSerializer);
            if (!paramData.Success)
            {
                log.Write(LogVerbosity.Warning, "Failed to deserialize kline data: " + paramData.Error);
                return;
            }

            foreach (var handler in subscriptions.Where(s => s.StreamId == streamId).OfType<CoinExKlineSubscription>())
                handler.Handler(handler.Market, paramData.Data.Cast<CoinExKline>().ToArray());
        }

        private void HandleBalanceUpdate(int streamId, JToken token)
        {
            var paramData = Deserialize<Dictionary<string, CoinExBalance>>(token["params"].First.ToString(), false);
            if (!paramData.Success)
            {
                log.Write(LogVerbosity.Warning, "Failed to deserialize kline data: " + paramData.Error);
                return;
            }

            foreach (var handler in subscriptions.Where(s => s.StreamId == streamId).OfType<CoinExBalanceSubscription>())
                handler.Handler(paramData.Data);
        }

        private CallResult<CoinExStream> ConnectNewSocket()
        {
            var socket = SocketFactory.CreateWebsocket(log, baseAddress);
            var id = NextStreamId();
            var stream = new CoinExStream() { Socket = socket, StreamResult = new CoinExStreamSubscription() { StreamId = id } };

            if (apiProxy != null)
                socket.SetProxy(apiProxy.Host, apiProxy.Port);

            socket.SetEnabledSslProtocols(protocols);

            socket.OnClose += () => SocketOnClose(stream);
            socket.OnError += SocketOnError;
            socket.OnOpen += SocketOnOpen;

            if (socket.Connect().Result)
            {
                lock (sockets)
                    sockets.Add(stream);
                return new CallResult<CoinExStream>(stream, null);
            }
            else
            {
                socket.Dispose();
                return new CallResult<CoinExStream>(null, new CantConnectError());
            }
        }

        private void SocketOnOpen()
        {
            log.Write(LogVerbosity.Debug, "Socket opened");
        }

        private void SocketOnError(Exception e)
        {
            log.Write(LogVerbosity.Error, $"Socket error {e?.Message}");
        }

        private void SocketOnClose(CoinExStream con)
        {
            if (con.TryReconnect)
            {
                log.Write(LogVerbosity.Info, "Connection lost, going to try to reconnect");
                Task.Run(() =>
                {
                    Thread.Sleep(reconnectInterval);
                    if (con.Socket.Connect().Result)
                        log.Write(LogVerbosity.Info, "Reconnected");
                });
            }
            else
            {
                log.Write(LogVerbosity.Info, "Socket closed");
                con.StreamResult.InvokeClosed();
                con.Socket.Dispose();
                lock (sockets)
                    sockets.Remove(con);
            }
        }



        private int NextStreamId()
        {
            lock (streamIdLock)
            {
                lastStreamId++;
                return lastStreamId;
            }
        }

        private void Configure(CoinExSocketClientOptions options)
        {
            base.Configure(options);
            baseAddress = options.BaseAddress;
            subResponseTimeout = (int)Math.Round(options.SubscriptionResponseTimeout.TotalMilliseconds);
            reconnectInterval = (int)Math.Round(options.ReconnectionInterval.TotalMilliseconds);
        }

        #endregion
    }
}
