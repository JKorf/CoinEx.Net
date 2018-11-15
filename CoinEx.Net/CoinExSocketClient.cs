using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using CryptoExchange.Net;
using CryptoExchange.Net.Implementation;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Interfaces;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net
{
    public class CoinExSocketClient: ExchangeClient, ICoinExSocketClient
    {
        #region fields
        private static CoinExSocketClientOptions defaultOptions = new CoinExSocketClientOptions();
        private static CoinExSocketClientOptions DefaultOptions
        {
            get
            {
                var result = new CoinExSocketClientOptions()
                {
                    LogVerbosity = defaultOptions.LogVerbosity,
                    BaseAddress = defaultOptions.BaseAddress,
                    LogWriters = defaultOptions.LogWriters,
                    Proxy = defaultOptions.Proxy,
                    RateLimiters = defaultOptions.RateLimiters,
                    RateLimitingBehaviour = defaultOptions.RateLimitingBehaviour,
                    ReconnectionInterval = defaultOptions.ReconnectionInterval,
                    SubscriptionResponseTimeout = defaultOptions.SubscriptionResponseTimeout
                };

                if (defaultOptions.ApiCredentials != null)
                    result.ApiCredentials = new ApiCredentials(defaultOptions.ApiCredentials.Key.GetString(), defaultOptions.ApiCredentials.Secret.GetString());

                return result;
            }
        }

        private int subResponseTimeout;
        private int reconnectInterval;
        private const SslProtocols protocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

        internal static int lastStreamId;
        internal static int lastRequestId;
        private static readonly object streamIdLock = new object();
        private static readonly object requestIdLock = new object();
        internal readonly List<CoinExStream> sockets = new List<CoinExStream>();

        private readonly JsonSerializer marketDepthSerializer = CreateJsonSerializerWithConverter(new ParamConverter(typeof(bool), typeof(CoinExSocketMarketDepth), typeof(string)));
        private readonly JsonSerializer marketTransactionSerializer = CreateJsonSerializerWithConverter(new ParamConverter( typeof(string), typeof(CoinExSocketMarketTransaction[])));
        private readonly JsonSerializer marketKlineSerializer = CreateJsonSerializerWithConverter(new ParamListConverter(typeof(CoinExKline)));
        private readonly JsonSerializer orderSerializer = CreateJsonSerializerWithConverter(new ParamConverter(typeof(int), typeof(CoinExSocketOrder)));

        private List<CoinExSubscription> subscriptions = new List<CoinExSubscription>();

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

        #region properties
        public IWebsocketFactory SocketFactory { get; set; } = new WebsocketFactory();

        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExSocketClient with default options
        /// </summary>
        public CoinExSocketClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of CoinExSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public CoinExSocketClient(CoinExSocketClientOptions options) : base(options, options.ApiCredentials == null ? null : new CoinExAuthenticationProvider(options.ApiCredentials))
        {
            Configure(options);
        }
        #endregion

        #region methods
        #region public
        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(CoinExSocketClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Synchronized version of the <see cref="PingAsync"/> method
        /// </summary>
        /// <returns></returns>
        public new CallResult<bool> Ping() => PingAsync().Result;
        /// <summary>
        /// Pings the server
        /// </summary>
        /// <returns>True if server responded, false otherwise</returns>
        public new async Task<CallResult<bool>> PingAsync()
        {
            var result = await QueryNewSocket<string>(new CoinExSocketRequest(ServerSubject, PingAction), false).ConfigureAwait(false);
            return new CallResult<bool>(result.Success, result.Error);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetServerTimeAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<DateTime> GetServerTime() => GetServerTimeAsync().Result;
        /// <summary>
        /// Gets the server time
        /// </summary>
        /// <returns>The server time</returns>
        public async Task<CallResult<DateTime>> GetServerTimeAsync()
        {
            var result = await QueryNewSocket<long>(new CoinExSocketRequest(ServerSubject, ServerTimeAction), false).ConfigureAwait(false);
            if(!result.Success)
                return new CallResult<DateTime>(default(DateTime), result.Error);
            return new CallResult<DateTime>(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result.Data), null);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetMarketStateAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExSocketMarketState> GetMarketState(string market, int cyclePeriod) => GetMarketStateAsync(market, cyclePeriod).Result;
        /// <summary>
        /// Get the market state
        /// </summary>
        /// <param name="market">The market to get the state for</param>
        /// <param name="cyclePeriod">The period to get data over, specified in seconds. i.e. one minute = 60, one day = 86400</param>
        /// <returns>Market state</returns>
        public async Task<CallResult<CoinExSocketMarketState>> GetMarketStateAsync(string market, int cyclePeriod)
        {
            return await QueryNewSocket<CoinExSocketMarketState>(new CoinExSocketRequest(StateSubject, QueryAction, market, cyclePeriod), false).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetMarketDepthAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExSocketMarketDepth> GetMarketDepth(string market, int limit, int mergeDepth) => GetMarketDepthAsync(market, limit, mergeDepth).Result;
        /// <summary>
        /// Get a market depth overview
        /// </summary>
        /// <param name="market">The market to get depth for</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <returns>Depth overview for a market</returns>
        public async Task<CallResult<CoinExSocketMarketDepth>> GetMarketDepthAsync(string market, int limit, int mergeDepth)
        {
            if (mergeDepth < 0 || mergeDepth > 8)
                return new CallResult<CoinExSocketMarketDepth>(null, new ArgumentError("Merge depth needs to be between 0 - 8"));

            if (limit != 5 && limit != 10 && limit != 20)
                return new CallResult<CoinExSocketMarketDepth>(null, new ArgumentError("Limit should be 5 / 10 / 20"));

            return await QueryNewSocket<CoinExSocketMarketDepth>(new CoinExSocketRequest(DepthSubject, QueryAction, market, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth)), false).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetMarketTransactionsAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExSocketMarketTransaction[]> GetMarketTransactions(string market, int limit, int? lastId = null) => GetMarketTransactionsAsync(market, limit, lastId).Result;
        /// <summary>
        /// Gets the latest transactions on a market
        /// </summary>
        /// <param name="market">The market to get the transactions for</param>
        /// <param name="limit">The limit of transactions</param>
        /// <param name="lastId">Return transactions since this id</param>
        /// <returns>List of transactions</returns>
        public async Task<CallResult<CoinExSocketMarketTransaction[]>> GetMarketTransactionsAsync(string market, int limit, int? lastId = null)
        {
            return await QueryNewSocket<CoinExSocketMarketTransaction[]>(new CoinExSocketRequest(TransactionSubject, QueryAction, market, limit, lastId ?? 0), false).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetMarketKlinesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExKline> GetMarketKlines(string market, KlineInterval interval) => GetMarketKlinesAsync(market, interval).Result;
        /// <summary>
        /// Gets market kline data
        /// </summary>
        /// <param name="market">The market to get the data for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <returns></returns>
        public async Task<CallResult<CoinExKline>> GetMarketKlinesAsync(string market, KlineInterval interval)
        {
            return await QueryNewSocket<CoinExKline>(new CoinExSocketRequest(KlineSubject, QueryAction, market, interval.ToSeconds()), false).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetBalancesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<Dictionary<string, CoinExBalance>> GetBalances(params string[] coins) => GetBalancesAsync(coins).Result;
        /// <summary>
        /// Get balances of coins. Requires API credentials
        /// </summary>
        /// <param name="coins">The coins to get the balances for, empty for all</param>
        /// <returns>Dictionary of coins and their balances</returns>
        public async Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(params string[] coins)
        {
            return await QueryNewSocket<Dictionary<string, CoinExBalance>>(new CoinExSocketRequest(BalanceSubject, QueryAction, coins), true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="GetOpenOrdersAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExSocketPagedResult<CoinExSocketOrder>> GetOpenOrders(string market, TransactionType type, int offset, int limit) => GetOpenOrdersAsync(market, type, offset, limit).Result;
        /// <summary>
        /// Gets a list of open orders for a market
        /// </summary>
        /// <param name="market">Market to get open orders for</param>
        /// <param name="type">The type of orders to get</param>
        /// <param name="offset">The offset in the list</param>
        /// <param name="limit">The limit of results</param>
        /// <returns>List of open orders</returns>
        public async Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOpenOrdersAsync(string market, TransactionType type, int offset, int limit)
        {
            return await QueryNewSocket<CoinExSocketPagedResult<CoinExSocketOrder>> (
                new CoinExSocketRequest(OrderSubject, QueryAction, market, int.Parse(JsonConvert.SerializeObject(type, new TransactionTypeIntConverter(false))), offset, limit), true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToMarketStateUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExStreamSubscription> SubscribeToMarketStateUpdates(string market, Action<string, CoinExSocketMarketState> onMessage) => SubscribeToMarketStateUpdatesAsync(market, onMessage).Result;
        /// <summary>
        /// Subscribe to market state updates for a specific market
        /// </summary>
        /// <param name="market">Market to receive updates for</param>
        /// <param name="onMessage">Datahandler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketState]: the market state update</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="UnsubscribeFromStream(CoinExStreamSubscription)"/> method</returns>
        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketStateUpdatesAsync(string market, Action<string, CoinExSocketMarketState> onMessage)
        {
            var sub = new CoinExStateSubscription() { Market = market, Handler = onMessage };
            return await Subscribe(sub, new CoinExSocketRequest(StateSubject, SubscribeAction, market)).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToMarketStateUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExStreamSubscription> SubscribeToMarketStateUpdates(Action<Dictionary<string, CoinExSocketMarketState>> onMessage) => SubscribeToMarketStateUpdatesAsync(onMessage).Result;
        /// <summary>
        /// Subscribe to market state updates for all markets
        /// </summary>
        /// <param name="onMessage">Datahandler, receives a dictionary of market name -> market state</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="UnsubscribeFromStream(CoinExStreamSubscription)"/> method</returns>
        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketStateUpdatesAsync(Action<Dictionary<string, CoinExSocketMarketState>> onMessage)
        {
            var sub = new CoinExStateMultiSubscription() { Handler = onMessage };
            return await Subscribe(sub, new CoinExSocketRequest(StateSubject, SubscribeAction)).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToMarketDepthUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExStreamSubscription> SubscribeToMarketDepthUpdates(string market, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage) => SubscribeToMarketDepthUpdatesAsync(market, limit, mergeDepth, onMessage).Result;
        /// <summary>
        /// Subscribe to market depth updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="limit">The limit of results to receive in a update</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="onMessage">Datahandler, receives Param 1[string]: the market name, Param 2[bool]: whether this is a full update, or an update based on the last send data, Param 3[CoinExSocketMarketDepth]: the update data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="UnsubscribeFromStream(CoinExStreamSubscription)"/> method</returns>
        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketDepthUpdatesAsync(string market, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage)
        {
            if (mergeDepth < 0 || mergeDepth > 8)
                return new CallResult<CoinExStreamSubscription>(null, new ArgumentError("Merge depth needs to be between 0 - 8"));

            if (limit != 5 && limit != 10 && limit != 20)
                return new CallResult<CoinExStreamSubscription>(null, new ArgumentError("Limit should be 5 / 10 / 20"));

            var sub = new CoinExDepthSubscription() { Handler = onMessage, Market = market };
            return await Subscribe(sub, new CoinExSocketRequest(DepthSubject, SubscribeAction, market, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth))).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToMarketTransactionUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExStreamSubscription> SubscribeToMarketTransactionUpdates(string market, Action<string, CoinExSocketMarketTransaction[]> onMessage) => SubscribeToMarketTransactionUpdatesAsync(market, onMessage).Result;
        /// <summary>
        /// Subscribe to market transaction updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates from</param>
        /// <param name="onMessage">Datahandler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketTransaction[]]: list of transactions</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="UnsubscribeFromStream(CoinExStreamSubscription)"/> method</returns>
        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketTransactionUpdatesAsync(string market, Action<string, CoinExSocketMarketTransaction[]> onMessage)
        {
            var sub = new CoinExTransactionsSubscription() { Handler = onMessage, Market = market };
            return await Subscribe(sub, new CoinExSocketRequest(TransactionSubject, SubscribeAction, market)).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToMarketKlineUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExStreamSubscription> SubscribeToMarketKlineUpdates(string market, KlineInterval interval, Action<string, CoinExKline[]> onMessage) => SubscribeToMarketKlineUpdatesAsync(market, interval, onMessage).Result;
        /// <summary>
        /// Subscribe to kline updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="interval">The interval of the candle to receive updates for</param>
        /// <param name="onMessage">Datahandler, receives Param 1[string]: the market name, Param 2[CoinExKline[]]: list of klines updated klines</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="UnsubscribeFromStream(CoinExStreamSubscription)"/> method</returns>
        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToMarketKlineUpdatesAsync(string market, KlineInterval interval, Action<string, CoinExKline[]> onMessage)
        {
            var sub = new CoinExKlineSubscription() { Handler = onMessage, Market = market };
            return await Subscribe(sub, new CoinExSocketRequest(KlineSubject, SubscribeAction, market, interval.ToSeconds())).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToBalanceUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExStreamSubscription> SubscribeToBalanceUpdates(Action<Dictionary<string, CoinExBalance>> onMessage) => SubscribeToBalanceUpdatesAsync(onMessage).Result;
        /// <summary>
        /// Subscribe to updates of your balances, Receives updates whenever the balance for a coin changes
        /// </summary>
        /// <param name="onMessage">Datahandler, receives a dictionary of ciub name -> balance</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="UnsubscribeFromStream(CoinExStreamSubscription)"/> method</returns>
        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToBalanceUpdatesAsync(Action<Dictionary<string, CoinExBalance>> onMessage)
        {
            var sub = new CoinExBalanceSubscription() { Handler = onMessage };
            return await Subscribe(sub, new CoinExSocketRequest(BalanceSubject, SubscribeAction), true).ConfigureAwait(false);
        }

        /// <summary>
        /// Synchronized version of the <see cref="SubscribeToOrderUpdatesAsync"/> method
        /// </summary>
        /// <returns></returns>
        public CallResult<CoinExStreamSubscription> SubscribeToOrderUpdates(string[] markets, Action<UpdateType, CoinExSocketOrder> onMessage) => SubscribeToOrderUpdatesAsync(markets, onMessage).Result;
        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// </summary>
        /// <param name="markets">The markets to receive order updates from</param>
        /// <param name="onMessage">Datahandler, receives Param 1[UpdateType]: the type of update, Param 2[CoinExSocketOrder]: the order that was updated</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is closed and can close this specific stream 
        /// using the <see cref="UnsubscribeFromStream(CoinExStreamSubscription)"/> method</returns>
        public async Task<CallResult<CoinExStreamSubscription>> SubscribeToOrderUpdatesAsync(string[] markets, Action<UpdateType, CoinExSocketOrder> onMessage)
        {
            var sub = new CoinExOrderSubscription() { Handler = onMessage };
            return await Subscribe(sub, new CoinExSocketRequest(OrderSubject, SubscribeAction, markets), true).ConfigureAwait(false);
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
                IEnumerable<CoinExStream> toClose;
                lock (sockets)
                    toClose = sockets.ToList();

                Task.WaitAll(toClose.Select(socket => socket.Close()).ToArray());                
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
        #endregion

        #region private
        private async Task<CallResult<bool>> Authenticate(CoinExStream stream)
        {
            if (authProvider == null)
                return new CallResult<bool>(false, new NoApiCredentialsError());

            var result = await Query<CoinExSocketRequestResponseMessage>(stream, new CoinExSocketRequest(ServerSubject, AuthenticateAction, GetAuthParameters())).ConfigureAwait(false);
            if (!result.Success)
            {
                log.Write(LogVerbosity.Debug, "Failed to authenticate: " + result.Error);
                return new CallResult<bool>(false, result.Error);
            }

            if (result.Data.Status != SuccessString)
            {
                log.Write(LogVerbosity.Debug, "Failed to authenticate: " + result.Data.Status);
                return new CallResult<bool>(false, new ServerError(result.Data.Status));
            }

            log.Write(LogVerbosity.Debug, "Successfully authenticated");
            return new CallResult<bool>(true, null);
        }

        private object[] GetAuthParameters()
        {
            var tonce = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            var parameterString = $"access_id={authProvider.Credentials.Key.GetString()}&tonce={tonce}&secret_key={authProvider.Credentials.Secret.GetString()}";
            var auth = authProvider.Sign(parameterString);
            return new object[] { authProvider.Credentials.Key.GetString(), auth, tonce };
        }

        private async Task<CallResult<T>> QueryNewSocket<T>(CoinExSocketRequest request, bool authenticated)
        {
            var con = ConnectNewSocket();
            if (!con.Success)
                return new CallResult<T>(default(T), con.Error);

            if (authenticated)
            {
                var auth = await Authenticate(con.Data).ConfigureAwait(false);
                if (!auth.Success)
                    return new CallResult<T>(default(T), auth.Error);
            }

            var result = await Query<T>(con.Data, request).ConfigureAwait(false);
            var closeTask = con.Data.Close().ConfigureAwait(false); // let it close in background
            return result;
        }

        private async Task<CallResult<T>> Query<T>(CoinExStream stream, CoinExSocketRequest request)
        {
            return await Task.Run(() =>
            {
                log.Write(LogVerbosity.Debug, $"Querying socket {stream.Subscription.StreamId} for {request.Method}");
                ManualResetEvent evnt = new ManualResetEvent(false);
                CallResult<CoinExSocketRequestResponse<T>> result = null;
                request.Id = NextRequestId();
                var onMessageAction = new Action<string>((msg) =>
                {
                    log.Write(LogVerbosity.Debug, "Socket received query response: " + msg);

                    var token = JToken.Parse(msg);
                    if ((int?) token["id"] != request.Id)
                        return;

                    if (token["error"].Type == JTokenType.Null)
                    {
                        result = Deserialize<CoinExSocketRequestResponse<T>>(msg);

                        if (request.Method.EndsWith("subscribe"))
                        {
                            lock (subscriptions)
                                if (!subscriptions.Contains(stream.Subscription))
                                    subscriptions.Add(stream.Subscription);
                        }
                    }
                    else
                    {
                        var errorResult = Deserialize<CoinExSocketError>(token["error"].ToString());
                        if (!errorResult.Success)
                            result = new CallResult<CoinExSocketRequestResponse<T>>(null, new ServerError("Unknown error: " + token["error"]));
                        else
                            result = new CallResult<CoinExSocketRequestResponse<T>>(null, new ServerError(errorResult.Data.Code, errorResult.Data.Message));
                    }

                    evnt?.Set();
                });

                stream.Socket.OnMessage += onMessageAction;
                var data = JsonConvert.SerializeObject(request);
                log.Write(LogVerbosity.Debug, "Sending data: " + data);
                stream.Socket.Send(data);
                evnt.WaitOne(subResponseTimeout);
                stream.Socket.OnMessage -= onMessageAction;
                evnt.Dispose();
                evnt = null;
                if (result == null)
                    return new CallResult<T>(default(T), new ServerError("No response from server"));
                if (!result.Success)
                    return new CallResult<T>(default(T), result.Error);
                if (result.Data.Error != null)
                    return new CallResult<T>(default(T), new ServerError(result.Data.Error.Code, result.Data.Error.Message));

                return new CallResult<T>(result.Data.Result, null);
            }).ConfigureAwait(false);
        }
        
        private async Task<CallResult<CoinExStreamSubscription>> Subscribe(CoinExSubscription subscription, CoinExSocketRequest request, bool authenticated = false)
        {
            log.Write(LogVerbosity.Debug, $"Starting new subscription for {request.Method}");
            var con = ConnectNewSocket();
            if (!con.Success)
                return new CallResult<CoinExStreamSubscription>(null, con.Error);

            con.Data.Authenticated = authenticated;
            con.Data.Subscription = subscription;
            return await Subscribe(con.Data, request, false).ConfigureAwait(false);
        }

        private async Task<CallResult<CoinExStreamSubscription>> Subscribe(CoinExStream stream, CoinExSocketRequest request, bool resubscribing)
        {
            if (stream.Authenticated)
            {
                var auth = await Authenticate(stream).ConfigureAwait(false);
                if (!auth.Success)
                    return new CallResult<CoinExStreamSubscription>(null, auth.Error);
            }

            if(!resubscribing) // Only add the message handler once, is already done if resubscribing
                stream.Socket.OnMessage += (msg) => OnMessage(stream.StreamResult.StreamId, msg);

            var subConfirm = await Query<CoinExSocketRequestResponseMessage>(stream, request).ConfigureAwait(false);
            if (subConfirm.Success)
            {
                stream.Request = request;
                stream.Subscription.StreamId = stream.StreamResult.StreamId;
                stream.TryReconnect = true;
                log.Write(LogVerbosity.Info, $"Subscription {stream.Subscription.StreamId} successful");
            }
            else
            {
                log.Write(LogVerbosity.Info, $"Failed to subscribe {stream.Subscription.StreamId}: {subConfirm.Error}");
                if(!resubscribing) // If we're just trying to initialy subscribe we dont need to reconnect if we failed subbing, so close it here
                    await stream.Close().ConfigureAwait(false);
            }

            return new CallResult<CoinExStreamSubscription>(stream.StreamResult, subConfirm.Error);
        }

        private void OnMessage(int streamId, string data)
        {
            var token = JToken.Parse(data);
            var id = (int?)token["id"];
            if (id != null)
                // If id != null it's a response to a query, dont handle here
                return;

            log.Write(LogVerbosity.Debug, $"Socket {streamId} received data: " + data);
            bool hasSubscription = false;
            lock (subscriptions)
                hasSubscription = subscriptions.Any(s => s.StreamId == streamId);

            if (!hasSubscription)
            {
                // Might be that we received the data before being able to process the subscription response.
                // Wait here for a little bit to let it process, if it then still isn't found we give an error message
                Thread.Sleep(10);
                lock (subscriptions)
                {
                    if (!subscriptions.Any(s => s.StreamId == streamId))
                    {
                        log.Write(LogVerbosity.Warning, $"Socket {streamId} received data for unknown subscription: " + data);
                        return;
                    }
                }
            }
            
    
            if (token["method"] == null)
                return;

            var subjectAction = ((string)token["method"]).Split('.');
            switch (subjectAction[0])
            {
                case TransactionSubject:
                    HandleTransactionUpdate(streamId, token);
                    break;
                case StateSubject:
                    HandleStateUpdate(streamId, token);
                    break;
                case DepthSubject:
                    HandleDepthUpdate(streamId, token);
                    break;
                case KlineSubject:
                    HandleKlineUpdate(streamId, token);
                    break;
                case BalanceSubject:
                    HandleBalanceUpdate(streamId, token);
                    break;
                case OrderSubject:
                    HandleOrderUpdate(streamId, token);
                    break;
                default:
                    log.Write(LogVerbosity.Warning, $"Socket {streamId} received unknown data: " + data);
                    break;
            }
        }

        private void HandleStateUpdate(int streamId, JToken token)
        {
            var paramData = Deserialize<Dictionary<string, CoinExSocketMarketState>>(token["params"].First.ToString(), false);
            if (!paramData.Success)
            {
                log.Write(LogVerbosity.Warning, "Failed to deserialize state data: " + paramData.Error);
                return;
            }

            List<CoinExSubscription> subs;
            lock (subscriptions)
                subs = subscriptions.ToList();

            foreach (var handler in subs.Where(s => s.StreamId == streamId).OfType<CoinExStateMultiSubscription>())
                handler.Handler(paramData.Data);

            if (paramData.Data.Count == 1) {
                var first = paramData.Data.First();
                foreach (var handler in subs.Where(s => s.StreamId == streamId).OfType<CoinExStateSubscription>().Where(s => s.Market == first.Key))
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

            if (paramData.Data.Length < 2)
            {
                log.Write(LogVerbosity.Warning, "Received unexpected data format for transaction update. Expected [string, transaction[]], received " + token["params"]);
                return;
            }

            List<CoinExSubscription> subs;
            lock (subscriptions)
                subs = subscriptions.ToList();

            string market = (string)paramData.Data[0];
            foreach (var handler in subs.Where(s => s.StreamId == streamId).OfType<CoinExTransactionsSubscription>().Where(s => s.Market == market))
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

            if (paramData.Data.Length < 3)
            {
                log.Write(LogVerbosity.Warning, "Received unexpected data format for depth update. Expected [bool, marketdepth, string], received " + token["params"]);
                return;
            }

            List<CoinExSubscription> subs;
            lock (subscriptions)
                subs = subscriptions.ToList();

            foreach (var handler in subs.Where(s => s.StreamId == streamId).OfType<CoinExDepthSubscription>())
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

            List<CoinExSubscription> subs;
            lock (subscriptions)
                subs = subscriptions.ToList();

            foreach (var handler in subs.Where(s => s.StreamId == streamId).OfType<CoinExKlineSubscription>())
                handler.Handler(handler.Market, paramData.Data.Cast<CoinExKline>().ToArray());
        }

        private void HandleBalanceUpdate(int streamId, JToken token)
        {
            var paramData = Deserialize<Dictionary<string, CoinExBalance>>(token["params"].First.ToString(), false);
            if (!paramData.Success)
            {
                log.Write(LogVerbosity.Warning, "Failed to deserialize balance data: " + paramData.Error);
                return;
            }

            List<CoinExSubscription> subs;
            lock (subscriptions)
                subs = subscriptions.ToList();

            foreach (var handler in subs.Where(s => s.StreamId == streamId).OfType<CoinExBalanceSubscription>())
                handler.Handler(paramData.Data);
        }

        private void HandleOrderUpdate(int streamId, JToken token)
        {
            var paramData = Deserialize<object[]>(token["params"].ToString(), false, orderSerializer);
            if (!paramData.Success)
            {
                log.Write(LogVerbosity.Warning, "Failed to deserialize order data: " + paramData.Error);
                return;
            }

            if (paramData.Data.Length < 2)
            {
                log.Write(LogVerbosity.Warning, "Received unexpected data format for order update. Expected [int, order], received " + token["params"]);
                return;
            }

            int updateTypeInt = (int)paramData.Data[0];
            UpdateType updateType = JsonConvert.DeserializeObject<UpdateType>(updateTypeInt.ToString(), new UpdateTypeConverter(false));

            List<CoinExSubscription> subs;
            lock (subscriptions)
                subs = subscriptions.ToList();

            foreach (var handler in subs.Where(s => s.StreamId == streamId).OfType<CoinExOrderSubscription>())
                handler.Handler(updateType, (CoinExSocketOrder)paramData.Data[1]);
        }

        private CallResult<CoinExStream> ConnectNewSocket()
        {
            var socket = SocketFactory.CreateWebsocket(log, baseAddress);
            log.Write(LogVerbosity.Debug, "Created new socket");
            var id = NextStreamId();
            var stream = new CoinExStream() { Socket = socket, StreamResult = new CoinExStreamSubscription() { StreamId = id } };

            if (apiProxy != null)
                socket.SetProxy(apiProxy.Host, apiProxy.Port);

            socket.SetEnabledSslProtocols(protocols);

            socket.OnClose += () => SocketOnClose(stream);
            socket.OnError += SocketOnError;
            socket.OnError += stream.StreamResult.InvokeError;
            socket.OnOpen += SocketOnOpen;

            if (socket.Connect().Result)
            {
                lock (sockets)
                    sockets.Add(stream);
                return new CallResult<CoinExStream>(stream, null);
            }

            socket.Dispose();
            return new CallResult<CoinExStream>(null, new CantConnectError());
        }

        private void SocketOnOpen()
        {
            log.Write(LogVerbosity.Debug, "Socket opened");
        }

        private void SocketOnError(Exception e)
        {
            log.Write(LogVerbosity.Error, $"Socket error {e?.Message}");
        }

        private void SocketOnClose(CoinExStream stream)
        {
            log.Write(LogVerbosity.Debug, $"Socket {stream.StreamResult.StreamId} closed event");
            if (stream.TryReconnect)
            {
                log.Write(LogVerbosity.Info, $"Socket {stream.StreamResult.StreamId} Connection lost, going to try to reconnect");
                Task.Run(() =>
                {
                    Thread.Sleep(reconnectInterval);
                    if (!stream.Socket.Connect().Result)
                    {
                        log.Write(LogVerbosity.Debug, $"Socket {stream.StreamResult.StreamId} failed to reconnect");
                        return; // Connect() should result in a SocketClosed event so we end up here again
                    }

                    log.Write(LogVerbosity.Info, $"Socket {stream.StreamResult.StreamId} Reconnected");
                    if (stream.Request != null)
                    {
                        var resubResult = Subscribe(stream, stream.Request, true).Result;
                        if (!resubResult.Success)
                        {
                            log.Write(LogVerbosity.Info, $"Socket {stream.StreamResult.StreamId} failed to resubscribe, closing socket and trying again");
                            stream.Close(true).Wait();
                        }
                        else
                            log.Write(LogVerbosity.Info, $"Socket {stream.StreamResult.StreamId} resubscribed reconnected socket");
                    }
                });
            }
            else
            {
                log.Write(LogVerbosity.Info, $"Socket {stream.StreamResult.StreamId} closed");
                lock(subscriptions)
                    if(subscriptions.Contains(stream.Subscription))
                        subscriptions.Remove(stream.Subscription);
                stream.StreamResult.InvokeClosed();
                stream.Socket.Dispose();
                lock (sockets)
                    sockets.Remove(stream);
            }
        }

        private static JsonSerializer CreateJsonSerializerWithConverter(JsonConverter converter)
        {
            return JsonSerializer.Create(new JsonSerializerSettings() { Converters = new List<JsonConverter>() { converter } });
        }

        private int NextStreamId()
        {
            lock (streamIdLock)
            {
                lastStreamId++;
                return lastStreamId;
            }
        }

        private int NextRequestId()
        {
            lock (requestIdLock)
            {
                lastRequestId++;
                return lastRequestId;
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
        #endregion
    }
}
