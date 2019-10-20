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
    /// <summary>
    /// Client for the CoinEx socket API
    /// </summary>
    public class CoinExSocketClient: SocketClient, ICoinExSocketClient
    {
        #region fields
        private static CoinExSocketClientOptions defaultOptions = new CoinExSocketClientOptions();
        private static CoinExSocketClientOptions DefaultOptions => defaultOptions.Copy<CoinExSocketClientOptions>();
        
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
            AddGenericHandler("Pong", (connection, token) => { });
            SendPeriodic(TimeSpan.FromMinutes(1), con => new CoinExSocketRequest(NextId(), ServerSubject, PingAction));
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
            var result = await Query<string>(new CoinExSocketRequest(NextId(), ServerSubject, PingAction), false).ConfigureAwait(false);
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
            var result = await Query<long>(new CoinExSocketRequest(NextId(), ServerSubject, ServerTimeAction), false).ConfigureAwait(false);
            if (!result)
                return new CallResult<DateTime>(default, result.Error);
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
            return await Query<CoinExSocketMarketState>(new CoinExSocketRequest(NextId(), StateSubject, QueryAction, market, cyclePeriod), false).ConfigureAwait(false);
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

            return await Query<CoinExSocketMarketDepth>(new CoinExSocketRequest(NextId(), DepthSubject, QueryAction, market, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth)), false).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the latest transactions on a market
        /// </summary>
        /// <param name="market">The market to get the transactions for</param>
        /// <param name="limit">The limit of transactions</param>
        /// <param name="fromId">Return transactions since this id</param>
        /// <returns>List of transactions</returns>
        public CallResult<IEnumerable<CoinExSocketMarketTransaction>> GetMarketTransactions(string market, int limit, int? fromId = null) => GetMarketTransactionsAsync(market, limit, fromId).Result;
        /// <summary>
        /// Gets the latest transactions on a market
        /// </summary>
        /// <param name="market">The market to get the transactions for</param>
        /// <param name="limit">The limit of transactions</param>
        /// <param name="fromId">Return transactions since this id</param>
        /// <returns>List of transactions</returns>
        public async Task<CallResult<IEnumerable<CoinExSocketMarketTransaction>>> GetMarketTransactionsAsync(string market, int limit, int? fromId = null)
        {
            return await Query<IEnumerable<CoinExSocketMarketTransaction>>(new CoinExSocketRequest(NextId(), TransactionSubject, QueryAction, market, limit, fromId ?? 0), false).ConfigureAwait(false);
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
            return await Query<CoinExKline>(new CoinExSocketRequest(NextId(), KlineSubject, QueryAction, market, interval.ToSeconds()), false).ConfigureAwait(false);
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
            return await Query<Dictionary<string, CoinExBalance>>(new CoinExSocketRequest(NextId(), BalanceSubject, QueryAction, coins), true).ConfigureAwait(false);
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
            return await Query<CoinExSocketPagedResult<CoinExSocketOrder>>(
                new CoinExSocketRequest(NextId(), OrderSubject, QueryAction, market, int.Parse(JsonConvert.SerializeObject(type, new TransactionTypeIntConverter(false))), offset, limit), true).ConfigureAwait(false);
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
                if (!desResult)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid state update: " + desResult.Error);
                    return;
                }

                onMessage(market, desResult.Data.First().Value);
            });

            return await Subscribe(new CoinExSocketRequest(NextId(), StateSubject, SubscribeAction, market), null, false, internalHandler).ConfigureAwait(false);
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
                if (!desResult)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid state update: " + desResult.Error);
                    return;
                }

                onMessage(desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(NextId(), StateSubject, SubscribeAction), null, false, internalHandler).ConfigureAwait(false);
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
                if (data.Length != 3)
                {
                    log.Write(LogVerbosity.Warning, $"Received unexpected data format for depth update. Expected 3 objects, received {data.Length}. Data: " + data.ToString());
                    return;
                }

                var fullUpdate = (bool)data[0];
                var desResult = Deserialize<CoinExSocketMarketDepth>(data[1], false);
                if (!desResult)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid depth update: " + desResult.Error);
                    return;
                }

                onMessage(market, fullUpdate, desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(NextId(), DepthSubject, SubscribeAction, market, limit, CoinExHelpers.MergeDepthIntToString(mergeDepth)), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to market transaction updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketTransaction[]]: list of transactions</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToMarketTransactionUpdates(string market, Action<string, IEnumerable<CoinExSocketMarketTransaction>> onMessage) => SubscribeToMarketTransactionUpdatesAsync(market, onMessage).Result;
        /// <summary>
        /// Subscribe to market transaction updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExSocketMarketTransaction[]]: list of transactions</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToMarketTransactionUpdatesAsync(string market, Action<string, IEnumerable<CoinExSocketMarketTransaction>> onMessage)
        {
            var internalHandler = new Action<JToken[]>(data =>
            {
                if (data.Length != 2)
                {
                    log.Write(LogVerbosity.Warning, $"Received unexpected data format for order update. Expected 2 objects, received {data.Length}. Data: [{string.Join(",", data.Select(s => s.ToString()))}]");
                    return;
                }

                var desResult = Deserialize<IEnumerable<CoinExSocketMarketTransaction>>(data[1], false);
                if (!desResult)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid order update: " + desResult.Error);
                    return;
                }

                onMessage(market, desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(NextId(), TransactionSubject, SubscribeAction, market), null, false, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to kline updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="interval">The interval of the candle to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExKline[]]: list of klines updated klines</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToMarketKlineUpdates(string market, KlineInterval interval, Action<string, IEnumerable<CoinExKline>> onMessage) => SubscribeToMarketKlineUpdatesAsync(market, interval, onMessage).Result;
        /// <summary>
        /// Subscribe to kline updates for a market
        /// </summary>
        /// <param name="market">The market to receive updates for</param>
        /// <param name="interval">The interval of the candle to receive updates for</param>
        /// <param name="onMessage">Data handler, receives Param 1[string]: the market name, Param 2[CoinExKline[]]: list of klines updated klines</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToMarketKlineUpdatesAsync(string market, KlineInterval interval, Action<string, IEnumerable<CoinExKline>> onMessage)
        {
            var internalHandler = new Action<JToken[]>(data =>
            {
                if (data.Length > 2)
                {
                    log.Write(LogVerbosity.Warning, $"Received unexpected data format for kline update. Expected 1 or 2 objects, received {data.Length}. Data: [{string.Join(",", data.Select(s => s.ToString()))}]");
                    return;
                }

                var desResult = Deserialize<IEnumerable<CoinExKline>>(new JArray(data), false);
                if (!desResult)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid market kline update: " + desResult.Error);
                    return;
                }

                onMessage(market, desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(NextId(), KlineSubject, SubscribeAction, market, interval.ToSeconds()), null, false, internalHandler).ConfigureAwait(false);
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
                if (!desResult)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid balance update: " + desResult.Error);
                    return;
                }

                onMessage(desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(NextId(), BalanceSubject, SubscribeAction), null, true, internalHandler).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// </summary>
        /// <param name="markets">The markets to receive order updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[UpdateType]: the type of update, Param 2[CoinExSocketOrder]: the order that was updated</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public CallResult<UpdateSubscription> SubscribeToOrderUpdates(IEnumerable<string> markets, Action<UpdateType, CoinExSocketOrder> onMessage) => SubscribeToOrderUpdatesAsync(markets, onMessage).Result;
        /// <summary>
        /// Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished
        /// </summary>
        /// <param name="markets">The markets to receive order updates from</param>
        /// <param name="onMessage">Data handler, receives Param 1[UpdateType]: the type of update, Param 2[CoinExSocketOrder]: the order that was updated</param>
        /// <returns>A stream subscription. This stream subscription can be used to be notified when the socket is disconnected/reconnected</returns>
        public async Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(IEnumerable<string> markets, Action<UpdateType, CoinExSocketOrder> onMessage)
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
                if (!desResult)
                {
                    log.Write(LogVerbosity.Warning, "Received invalid order update: " + desResult.Error);
                    return;
                }

                onMessage(updateResult, desResult.Data);
            });

            return await Subscribe(new CoinExSocketRequest(NextId(), OrderSubject, SubscribeAction, markets), null, true, internalHandler).ConfigureAwait(false);
        }
        #endregion

        #region private
        private object[] GetAuthParameters()
        {
            if(authProvider!.Credentials.Key == null || authProvider.Credentials.Secret == null)
                throw new ArgumentException("ApiKey/Secret not provided");

            var tonce = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            var parameterString = $"access_id={authProvider.Credentials.Key.GetString()}&tonce={tonce}&secret_key={authProvider.Credentials.Secret.GetString()}";
            var auth = authProvider.Sign(parameterString);
            return new object[] { authProvider.Credentials.Key.GetString(), auth, tonce };
        }
        #endregion
        #endregion

        /// <inheritdoc />
        protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T>? callResult)
        {
            callResult = null;
            var cRequest = (CoinExSocketRequest) request;
            var idField = data["id"];
            if (idField == null)
                return false;

            if ((int)idField != cRequest.Id)
                return false;

            if (data["error"].Type != JTokenType.Null)
            {
                callResult = new CallResult<T>(default, new ServerError((int)data["error"]["code"], (string)data["error"]["message"]));
                return true;
            }
            else
            {
                var desResult = Deserialize<T>(data["result"]);
                if (!desResult)
                {
                    callResult = new CallResult<T>(default, desResult.Error);
                    return true;
                }

                callResult = new CallResult<T>(desResult.Data, null);
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

            var cRequest = (CoinExSocketRequest) request;
            if ((int) idField != cRequest.Id)
                return false;

            var subResponse = Deserialize<CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>>(message, false);
            if (!subResponse)
            {
                log.Write(LogVerbosity.Warning, "Subscription failed: " + subResponse.Error);
                callResult = new CallResult<object>(null, subResponse.Error);
                return true;
            }

            if (subResponse.Data.Error != null)
            {
                log.Write(LogVerbosity.Debug, $"Failed to subscribe: {subResponse.Data.Error.Code} {subResponse.Data.Error.Message}");
                callResult = new CallResult<object>(null, new ServerError(subResponse.Data.Error.Code, subResponse.Data.Error.Message));
                return true;
            }

            log.Write(LogVerbosity.Debug, "Subscription completed");
            callResult = new CallResult<object>(subResponse, null);
            return true;
        }

        /// <inheritdoc />
        protected override JToken ProcessTokenData(JToken data)
        {
            return data["params"];
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(JToken message, object request)
        {
            var cRequest = (CoinExSocketRequest)request;
            var method = message["method"];
            if (method == null)
                return false;

            var subject = ((string) method).Split(new [] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            return cRequest.Subject == subject;
        }

        /// <inheritdoc />
        protected override bool MessageMatchesHandler(JToken message, string identifier)
        {
            if (message.Type != JTokenType.Object)
                return false;
            return identifier == "Pong" && (string) message["result"] == "pong";
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> AuthenticateSocket(SocketConnection s)
        {
            if (authProvider == null)
                return new CallResult<bool>(false, new NoApiCredentialsError());

            var request = new CoinExSocketRequest(NextId(), ServerSubject, AuthenticateAction, GetAuthParameters());
            var result = new CallResult<bool>(false, new ServerError("No response from server"));
            await s.SendAndWait(request, ResponseTimeout, data =>
            {
                var idField = data["id"];
                if (idField == null)
                    return false;

                if ((int)idField != request.Id)
                    return false; // Not for this request

                var authResponse = Deserialize<CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>>(data, false);
                if (!authResponse)
                {
                    log.Write(LogVerbosity.Warning, "Authorization failed: " + authResponse.Error);
                    result = new CallResult<bool>(false, authResponse.Error);
                    return true;
                }

                if (authResponse.Data.Error != null)
                {
                    var error = new ServerError(authResponse.Data.Error.Code, authResponse.Data.Error.Message);
                    log.Write(LogVerbosity.Debug, "Failed to authenticate: " + error);
                    result = new CallResult<bool>(false, error);
                    return true;
                }

                if (authResponse.Data.Result.Status != SuccessString)
                {
                    log.Write(LogVerbosity.Debug, "Failed to authenticate: " + authResponse.Data.Result.Status);
                    result = new CallResult<bool>(false, new ServerError(authResponse.Data.Result.Status));
                    return true;
                }

                log.Write(LogVerbosity.Debug, "Authorization completed");
                result = new CallResult<bool>(true, null);
                return true;
            });

            return result;
        }

        /// <inheritdoc />
        protected override Task<bool> Unsubscribe(SocketConnection connection, SocketSubscription s)
        {
            return Task.FromResult(true);
        }
    }
}
