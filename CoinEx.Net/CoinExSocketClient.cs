using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using CryptoExchange.Net;
using CryptoExchange.Net.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinEx.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace CoinEx.Net
{
    public class CoinExSocketClient: SocketClient, ICoinExSocketClient
    {
        #region fields
        private static CoinExSocketClientOptions defaultOptions = new CoinExSocketClientOptions();
        private static CoinExSocketClientOptions DefaultOptions => defaultOptions.Copy();

        private int subResponseTimeout;
        
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
        /// Pings the server
        /// </summary>
        /// <returns>True if server responded, false otherwise</returns>
        public CallResult<bool> Ping() => PingAsync().Result;
        /// <summary>
        /// Pings the server
        /// </summary>
        /// <returns>True if server responded, false otherwise</returns>
        public async Task<CallResult<bool>> PingAsync()
        {
            var result = await Query<string>(new CoinExSocketRequest(ServerSubject, PingAction, false)).ConfigureAwait(false);
            return new CallResult<bool>(result.Success, result.Error);
        }

        /// <summary>
        /// Gets the server time
        /// </summary>
        /// <returns>The server time</returns>
        public CallResult<DateTime> GetServerTime() => GetServerTimeAsync().Result;
        /// <summary>
        /// Gets the server time
        /// </summary>
        /// <returns>The server time</returns>
        public async Task<CallResult<DateTime>> GetServerTimeAsync()
        {
            var result = await Query<long>(new CoinExSocketRequest(ServerSubject, ServerTimeAction, false)).ConfigureAwait(false);
            if (!result.Success)
                return new CallResult<DateTime>(default(DateTime), result.Error);
            return new CallResult<DateTime>(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result.Data), null);
        }

        /// <summary>
        /// Get the market state
        /// </summary>
        /// <param name="market">The market to get the state for</param>
        /// <param name="cyclePeriod">The period to get data over, specified in seconds. i.e. one minute = 60, one day = 86400</param>
        /// <returns>Market state</returns>
        public CallResult<CoinExSocketMarketState> GetMarketState(string market, int cyclePeriod) => GetMarketStateAsync(market, cyclePeriod).Result;
        /// <summary>
        /// Get the market state
        /// </summary>
        /// <param name="market">The market to get the state for</param>
        /// <param name="cyclePeriod">The period to get data over, specified in seconds. i.e. one minute = 60, one day = 86400</param>
        /// <returns>Market state</returns>
        public async Task<CallResult<CoinExSocketMarketState>> GetMarketStateAsync(string market, int cyclePeriod)
        {
            return await Query<CoinExSocketMarketState>(new CoinExSocketRequest(StateSubject, QueryAction, false, market, cyclePeriod)).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a market depth overview
        /// </summary>
        /// <param name="market">The market to get depth for</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <returns>Depth overview for a market</returns>
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

            return await Query<CoinExSocketMarketDepth>(new CoinExSocketRequest(DepthSubject, QueryAction, false, market, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth))).ConfigureAwait(false);            
        }

        /// <summary>
        /// Gets the latest transactions on a market
        /// </summary>
        /// <param name="market">The market to get the transactions for</param>
        /// <param name="limit">The limit of transactions</param>
        /// <param name="fromId">Return transactions since this id</param>
        /// <returns>List of transactions</returns>
        public CallResult<CoinExSocketMarketTransaction[]> GetMarketTransactions(string market, int limit, int? fromId = null) => GetMarketTransactionsAsync(market, limit, fromId).Result;
        /// <summary>
        /// Gets the latest transactions on a market
        /// </summary>
        /// <param name="market">The market to get the transactions for</param>
        /// <param name="limit">The limit of transactions</param>
        /// <param name="fromId">Return transactions since this id</param>
        /// <returns>List of transactions</returns>
        public async Task<CallResult<CoinExSocketMarketTransaction[]>> GetMarketTransactionsAsync(string market, int limit, int? fromId = null)
        {
            return await Query<CoinExSocketMarketTransaction[]>(new CoinExSocketRequest(TransactionSubject, QueryAction, false, market, limit, fromId ?? 0)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets market kline data
        /// </summary>
        /// <param name="market">The market to get the data for</param>
        /// <param name="interval">The interval of the candles</param>
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
            return await Query<CoinExKline>(new CoinExSocketRequest(KlineSubject, QueryAction, false, market, interval.ToSeconds())).ConfigureAwait(false);
        }

        /// <summary>
        /// Get balances of coins. Requires API credentials
        /// </summary>
        /// <param name="coins">The coins to get the balances for, empty for all</param>
        /// <returns>Dictionary of coins and their balances</returns>
        public CallResult<Dictionary<string, CoinExBalance>> GetBalances(params string[] coins) => GetBalancesAsync(coins).Result;
        /// <summary>
        /// Get balances of coins. Requires API credentials
        /// </summary>
        /// <param name="coins">The coins to get the balances for, empty for all</param>
        /// <returns>Dictionary of coins and their balances</returns>
        public async Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(params string[] coins)
        {
            return await Query<Dictionary<string, CoinExBalance>>(new CoinExSocketRequest(BalanceSubject, QueryAction, true, coins)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of open orders for a market
        /// </summary>
        /// <param name="market">Market to get open orders for</param>
        /// <param name="type">The type of orders to get</param>
        /// <param name="offset">The offset in the list</param>
        /// <param name="limit">The limit of results</param>
        /// <returns>List of open orders</returns>
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
            return await Query<CoinExSocketPagedResult<CoinExSocketOrder>> (
                new CoinExSocketRequest(OrderSubject, QueryAction, true, market, int.Parse(JsonConvert.SerializeObject(type, new TransactionTypeIntConverter(false))), offset, limit)).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to market state updates for a specific market
        /// </summary>
        /// <param name="market">Market to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketState]: the market state update</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToMarketStateUpdates(string market, Action<string, CoinExSocketMarketState> onMessage) => SubscribeToMarketStateUpdatesAsync(market, onMessage).Result;
        /// <summary>
        /// Subscribe to market state updates for a specific market
        /// </summary>
        /// <param name="market">Market to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketState]: the market state update</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToMarketStateUpdatesAsync(string market, Action<string, CoinExSocketMarketState> onMessage)
        {
            var internalHandler = new Action<JToken[]>(data =>
            {
                var desResult = Deserialize<Dictionary<string, CoinExSocketMarketState>>(data[0]);
                if (!desResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid state update: " + desResult.Error);
                    return;
                }

                onMessage(market, desResult.Data.First().Value);
            });

            return await Subscribe(new CoinExSocketRequest(StateSubject, SubscribeAction, false, market), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to market state updates for all markets
        /// </summary>
        /// <param name="onMessage">Data handler, receives a dictionary of market name -> market state</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToMarketStateUpdates(Action<Dictionary<string, CoinExSocketMarketState>> onMessage) => SubscribeToMarketStateUpdatesAsync(onMessage).Result;
        /// <summary>
        /// Subscribe to market state updates for all markets
        /// </summary>
        /// <param name="onMessage">Data handler, receives a dictionary of market name -> market state</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToMarketStateUpdatesAsync(Action<Dictionary<string, CoinExSocketMarketState>> onMessage)
        {
            var internalHandler = new Action<JToken[]>(data =>
            {
                var desResult = Deserialize<Dictionary<string, CoinExSocketMarketState>>(data[0]);
                if (!desResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid state update: " + desResult.Error);
                    return;
                }

                onMessage(desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(StateSubject, SubscribeAction, false), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to market depth updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="limit">The limit of results to receive in a update</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[bool]: whether this is a full update, or an update based on the last send data, Param 3[CoinExSocketMarketDepth]: the update data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToMarketDepthUpdates(string market, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage) => SubscribeToMarketDepthUpdatesAsync(market, limit, mergeDepth, onMessage).Result;
        /// <summary>
        /// Subscribe to market depth updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="limit">The limit of results to receive in a update</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[bool]: whether this is a full update, or an update based on the last send data, Param 3[CoinExSocketMarketDepth]: the update data</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToMarketDepthUpdatesAsync(string market, int limit, int mergeDepth, Action<string, bool, CoinExSocketMarketDepth> onMessage)
        {
            if (mergeDepth < 0 || mergeDepth > 8)
                return new CallResult<UpdateSubscription>(null, new ArgumentError("Merge depth needs to be between 0 - 8"));

            if (limit != 5 && limit != 10 && limit != 20)
                return new CallResult<UpdateSubscription>(null, new ArgumentError("Limit should be 5 / 10 / 20"));

            var internalHandler = new Action<JToken[]>(data =>
            {
                if(data.Length != 3)
                {
                    log.Write(LogVerbosity.Warning, $"Received unexpected data format for depth update. Expected 3 objects, received {data.Length}. Data: " + data.ToString());
                    return;
                }

                var fullUpdate = (bool)data[0];
                var desResult = Deserialize<CoinExSocketMarketDepth>(data[1], false);
                if (!desResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid depth update: " + desResult.Error);
                    return;
                }

                onMessage(market, fullUpdate, desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(DepthSubject, SubscribeAction, false, market, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth)), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to market transaction updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketTransaction[]]: list of transactions</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToMarketTransactionUpdates(string market, Action<string, CoinExSocketMarketTransaction[]> onMessage) => SubscribeToMarketTransactionUpdatesAsync(market, onMessage).Result;
        /// <summary>
        /// Subscribe to market transaction updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketTransaction[]]: list of transactions</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToMarketTransactionUpdatesAsync(string market, Action<string, CoinExSocketMarketTransaction[]> onMessage)
        {
            var internalHandler = new Action<JToken[]>(data =>
            {
                if (data.Length != 2)
                {
                    log.Write(LogVerbosity.Warning, $"Received unexpected data format for order update. Expected 2 objects, received {data.Length}. Data: [{string.Join(",", data.Select(s => s.ToString()))}]");
                    return;
                }

                var desResult = Deserialize<CoinExSocketMarketTransaction[]>(data[1], false);
                if (!desResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid order update: " + desResult.Error);
                    return;
                }

                onMessage(market, desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(TransactionSubject, SubscribeAction, false, market), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to kline updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="interval">The interval of the candle to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExKline[]]: list of klines updated klines</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToMarketKlineUpdates(string market, KlineInterval interval, Action<string, CoinExKline[]> onMessage) => SubscribeToMarketKlineUpdatesAsync(market, interval, onMessage).Result;
        /// <summary>
        /// Subscribe to kline updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="interval">The interval of the candle to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExKline[]]: list of klines updated klines</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToMarketKlineUpdatesAsync(string market, KlineInterval interval, Action<string, CoinExKline[]> onMessage)
        {
            var internalHandler = new Action<JToken[]>(data =>
            {
                if (data.Length > 2)
                {
                    log.Write(LogVerbosity.Warning, $"Received unexpected data format for kline update. Expected 1 or 2 objects, received {data.Length}. Data: [{string.Join(",", data.Select(s=> s.ToString()))}]");
                    return;
                }

                var desResult = Deserialize<CoinExKline[]>(new JArray(data), false);
                if (!desResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid market kline update: " + desResult.Error);
                    return;
                }

                onMessage(market, desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(KlineSubject, SubscribeAction, false, market, interval.ToSeconds()), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to updates of your balances, Receives updates whenever the balance for a coin changes
        /// </summary>
        /// <param name="onMessage">Data handler, receives a dictionary of coin name -> balance</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToBalanceUpdates(Action<Dictionary<string, CoinExBalance>> onMessage) => SubscribeToBalanceUpdatesAsync(onMessage).Result;
        /// <summary>
        /// Subscribe to updates of your balances, Receives updates whenever the balance for a coin changes
        /// </summary>
        /// <param name="onMessage">Data handler, receives a dictionary of coin name -> balance</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<Dictionary<string, CoinExBalance>> onMessage)
        {
            var internalHandler = new Action<JToken[]>(data =>
            {
                if (data.Length != 1)
                {
                    log.Write(LogVerbosity.Warning, $"Received unexpected data format for order update. Expected 1 objects, received {data.Length}. Data: [{string.Join(",", data.Select(s => s.ToString()))}]");
                    return;
                }

                var desResult = Deserialize<Dictionary<string, CoinExBalance>>(data[0], false);
                if (!desResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid balance update: " + desResult.Error);
                    return;
                }

                onMessage(desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(BalanceSubject, SubscribeAction, true), internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// </summary>
        /// <param name="markets">The markets to receive order updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[UpdateType]: the type of update, Param 2[CoinExSocketOrder]: the order that was updated</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToOrderUpdates(string[] markets, Action<UpdateType, CoinExSocketOrder> onMessage) => SubscribeToOrderUpdatesAsync(markets, onMessage).Result;
        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// </summary>
        /// <param name="markets">The markets to receive order updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[UpdateType]: the type of update, Param 2[CoinExSocketOrder]: the order that was updated</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(string[] markets, Action<UpdateType, CoinExSocketOrder> onMessage)
        {
            var internalHandler = new Action<JToken[]>(data =>
            {
                if (data.Length != 2)
                {
                    log.Write(LogVerbosity.Warning, $"Received unexpected data format for order update. Expected 2 objects, received {data.Length}. Data: [{string.Join(",", data.Select(s => s.ToString()))}]");
                    return;
                }

                var updateResult = JsonConvert.DeserializeObject<UpdateType>((string)data[0], new UpdateTypeConverter(false));
                var desResult = Deserialize<CoinExSocketOrder>(data[1], false);
                if (!desResult.Success)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid order update: " + desResult.Error);
                    return;
                }

                onMessage(updateResult, desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(OrderSubject, SubscribeAction, true, markets), internalHandler).ConfigureAwait(false);
        }
        #endregion

        #region private
        private object[] GetAuthParameters()
        {
            var tonce = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            var parameterString = $"access_id={authProvider.Credentials.Key.GetString()}&tonce={tonce}&secret_key={authProvider.Credentials.Secret.GetString()}";
            var auth = authProvider.Sign(parameterString);
            return new object[] { authProvider.Credentials.Key.GetString(), auth, tonce };
        }

        private async Task<CallResult<T>> Query<T>(CoinExSocketRequest request)
        {
            CallResult<T> result = null;
            var internalHandler = new Action<JToken[]>(data => result = Deserialize<T>(data[0]));

            var subscription = GetBackgroundSocket(request.Signed);
            if (subscription == null)
            {
                // We don't have a background socket to query, create a new one
                var connectResult = await CreateAndConnectSocket(request.Signed, false, internalHandler).ConfigureAwait(false);
                if (!connectResult.Success)
                    return new CallResult<T>(default(T), connectResult.Error);

                subscription = connectResult.Data;
                subscription.Type = request.Signed ? SocketType.BackgroundAuthenticated : SocketType.Background;
            }
            else
            {
                // Use earlier created background socket to query without having to connect again
                subscription.Events.Single(s => s.Name == DataEvent).Reset();
                subscription.MessageHandlers[DataHandlerName] = (subs, data) => DataHandlerQuery(subs, data, internalHandler);
            }

            request.Id = NextId();

            var waitTask = subscription.WaitForEvent(DataEvent, request.Id.ToString(), subResponseTimeout);
            Send(subscription.Socket, request);
            var dataResult = await waitTask.ConfigureAwait(false);

            if (!dataResult.Success)
                return new CallResult<T>(default(T), dataResult.Error);
			
            return result;
        }
               

        private async Task<CallResult<UpdateSubscription>> Subscribe(CoinExSocketRequest request, Action<JToken[]> onData)
        {
            var connectResult = await CreateAndConnectSocket(request.Signed, true, onData).ConfigureAwait(false);
            if (!connectResult.Success)
                return new CallResult<UpdateSubscription>(null, connectResult.Error);

            return await Subscribe(connectResult.Data, request).ConfigureAwait(false);
        }

        private async Task<CallResult<UpdateSubscription>> Subscribe(SocketSubscription subscription, CoinExSocketRequest request)
        {
            request.Id = NextId();
            var waitTask = subscription.WaitForEvent(SubscriptionEvent, request.Id.ToString(), subResponseTimeout);
            Send(subscription.Socket, request);

            var subResult = await waitTask.ConfigureAwait(false);
            if (!subResult.Success)
            {
                await subscription.Close().ConfigureAwait(false);
                return new CallResult<UpdateSubscription>(null, subResult.Error);
            }

            subscription.Request = request;
            subscription.Socket.ShouldReconnect = true;
            return new CallResult<UpdateSubscription>(new UpdateSubscription(subscription), null);
        }

        private async Task<CallResult<SocketSubscription>> CreateAndConnectSocket(bool authenticate, bool sub, Action<JToken[]> onMessage)
        {
            var socket = CreateSocket(BaseAddress);
            var subscription = new SocketSubscription(socket);
            if (sub)
            {
                subscription.MessageHandlers.Add(DataHandlerName, (subs, data) => DataHandlerSubscription(data, onMessage));
                subscription.AddEvent(SubscriptionEvent);
            }
            else
            {
                subscription.MessageHandlers.Add(DataHandlerName, (subs, data) => DataHandlerQuery(subs, data, onMessage));
                subscription.AddEvent(DataEvent);
            }

            subscription.MessageHandlers.Add(AuthenticationHandlerName, AuthenticationHandler);
            subscription.MessageHandlers.Add(SubscriptionHandlerName, SubscriptionHandler);

            if (authenticate)
                subscription.AddEvent(AuthenticationEvent);            

            var connectResult = await ConnectSocket(subscription).ConfigureAwait(false);
            if (!connectResult.Success)
                return new CallResult<SocketSubscription>(null, connectResult.Error);

            if (authenticate)
            {
                var authResult = await Authenticate(subscription).ConfigureAwait(false);
                if (!authResult.Success)
                    return new CallResult<SocketSubscription>(null, authResult.Error);
            }

            return new CallResult<SocketSubscription>(subscription, null);
        }

        private async Task<CallResult<bool>> Authenticate(SocketSubscription subscription)
        {
            var request = new CoinExSocketRequest(ServerSubject, AuthenticateAction, true, GetAuthParameters()) {Id = NextId()};

            var waitTask = subscription.WaitForEvent(AuthenticationEvent, request.Id.ToString(), subResponseTimeout);
            Send(subscription.Socket, request);
            var authResult = await waitTask.ConfigureAwait(false);

            if (!authResult.Success)
            {
                var closeTask = subscription.Close();
                return new CallResult<bool>(false, authResult.Error);
            }

            return new CallResult<bool>(true, null);
        }

        private bool DataHandlerSubscription(JToken data, Action<JToken[]> handler)
        {
            var notifyData = data["params"] != null && ((string)data["method"]).EndsWith(".update");
            if (!notifyData)
                return false;
            
            var desResult = Deserialize<CoinExSocketResponse>(data, false);
            if (!desResult.Success)
            {
                log.Write(LogVerbosity.Warning, $"Failed to deserialize data: {desResult.Error}. Data: {data}");
                return true;
            }

            handler(data["params"].ToArray());
            return true;
        }

        private bool DataHandlerQuery(SocketSubscription subscription, JToken data, Action<JToken[]> handler)
        {
            var evnt = subscription.GetWaitingEvent(DataEvent);
            if (evnt == null)
                return false;

            if (((int?)data["id"])?.ToString() != evnt.WaitingId)
                return false;
            
            if (data["result"].Type == JTokenType.Null)
            {
                subscription.SetEventById(evnt.WaitingId, false, new ServerError((int)data["error"]["code"], (string)data["error"]["message"]));
            }
            else
            {
                handler(new[] { data["result"] });
                subscription.SetEventById(evnt.WaitingId, true, null);
            }
            return true;
        }

        private bool AuthenticationHandler(SocketSubscription subscription, JToken data)
        {
            var evnt = subscription.GetWaitingEvent(AuthenticationEvent);
            if (evnt == null)
                return false;

            if (((int?)data["id"])?.ToString() != evnt.WaitingId)
                return false;

            var authResponse = Deserialize< CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>>(data, false);
            if (!authResponse.Success)
            {
                log.Write(LogVerbosity.Warning, $"Authorization failed: " + authResponse.Error);
                subscription.SetEventById(evnt.WaitingId, false, authResponse.Error);
                return true;
            }

            if(authResponse.Data.Error != null)
            {
                var error = new ServerError(authResponse.Data.Error.Code, authResponse.Data.Error.Message);
                log.Write(LogVerbosity.Debug, "Failed to authenticate: " + error);
                subscription.SetEventById(evnt.WaitingId, false, error);
                return true;
            }

            if (authResponse.Data.Result.Status != SuccessString)
            {
                log.Write(LogVerbosity.Debug, "Failed to authenticate: " + authResponse.Data.Result.Status);
                subscription.SetEventById(evnt.WaitingId, false, new ServerError(authResponse.Data.Result.Status));
                return true;
            }

            log.Write(LogVerbosity.Debug, $"Authorization completed");
            subscription.SetEventById(evnt.WaitingId, true, null);
            return true;
        }

        private bool SubscriptionHandler(SocketSubscription subscription, JToken data)
        {
            var evnt = subscription.GetWaitingEvent(SubscriptionEvent);
            if (evnt == null)
                return false;

            if (((int?)data["id"])?.ToString() != evnt.WaitingId)
                return false;

            var authResponse = Deserialize<CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>>(data, false);
            if (!authResponse.Success)
            {
                log.Write(LogVerbosity.Warning, $"Subscription failed: " + authResponse.Error);
                subscription.SetEventById(evnt.WaitingId, false, authResponse.Error);
                return true;
            }

            if (authResponse.Data.Error != null)
            {
                log.Write(LogVerbosity.Debug, $"Failed to subscribe: {authResponse.Data.Error.Code} {authResponse.Data.Error.Message}");
                subscription.SetEventById(evnt.WaitingId, false, new ServerError(authResponse.Data.Error.Code, authResponse.Data.Error.Message));
                return true;
            }

            log.Write(LogVerbosity.Debug, $"Subscription completed");
            subscription.SetEventById(evnt.WaitingId, true, null);
            return true;
        }

        protected override bool SocketReconnect(SocketSubscription subscription, TimeSpan disconnectedTime)
        {
            var request = (CoinExSocketRequest)subscription.Request;
            if (request.Signed)
            {
                if (!Authenticate(subscription).Result.Success)
                    return false;
            }

            return Subscribe(subscription, request).Result.Success;
        }
       
        private void Configure(CoinExSocketClientOptions options)
        {
            subResponseTimeout = (int)Math.Round(options.SubscriptionResponseTimeout.TotalMilliseconds);
        }
        #endregion
        #endregion
    }
}
