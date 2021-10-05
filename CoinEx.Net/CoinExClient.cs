using CoinEx.Net.Converters;
using CoinEx.Net.Objects;
using CryptoExchange.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ExchangeInterfaces;
using Newtonsoft.Json.Linq;
using CryptoExchange.Net.Interfaces;

namespace CoinEx.Net
{
    /// <summary>
    /// Client for the CoinEx REST API
    /// </summary>
    public class CoinExClient : RestClient, ICoinExClient, IExchangeClient
    {
        #region fields
        private static CoinExClientOptions defaultOptions = new CoinExClientOptions();
        private static CoinExClientOptions DefaultOptions => defaultOptions.Copy<CoinExClientOptions>();

        private const string AssetConfigEndpoint = "common/asset/config";
        private const string CurrencyRateEndpoint = "common/currency/rate";

        private const string MarketListEndpoint = "market/list";
        private const string MarketStatisticsEndpoint = "market/ticker";
        private const string MarketStatisticsListEndpoint = "market/ticker/all";
        private const string MarketDepthEndpoint = "market/depth";
        private const string MarketDealsEndpoint = "market/deals";
        private const string MarketKlinesEndpoint = "market/kline";
        private const string MarketInfoEndpoint = "market/info";

        private const string AccountInfoEndpoint = "balance/info";
        private const string WithdrawalHistoryEndpoint = "balance/coin/withdraw";
        private const string DepositHistoryEndpoint = "balance/coin/deposit";
        private const string WithdrawEndpoint = "balance/coin/withdraw";
        private const string CancelWithdrawalEndpoint = "balance/coin/withdraw";
        private const string DepositAddressEndpoint = "balance/deposit/address/";

        private const string PlaceLimitOrderEndpoint = "order/limit";
        private const string PlaceMarketOrderEndpoint = "order/market";
        private const string PlaceStopLimitOrderEndpoint = "order/stop/limit";
        private const string PlaceStopMarketOrderEndpoint = "order/stop/market";
        private const string PlaceImmediateOrCancelOrderEndpoint = "order/ioc";

        private const string FinishedOrdersEndpoint = "order/finished";
        private const string OpenOrdersEndpoint = "order/pending";
        private const string OpenStopOrdersEndpoint = "order/stop/pending";
        private const string OrderStatusEndpoint = "order/status";
        private const string OrderDetailsEndpoint = "order/deals";
        private const string UserTransactionsEndpoint = "order/user/deals";
        private const string CancelOrderEndpoint = "order/pending";
        private const string CancelStopOrderEndpoint = "order/stop/pending";
        private const string MiningDifficultyEndpoint = "order/mining/difficulty";

        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is cancelled via this client. Note that this does not trigger when using CancelAllOrdersAsync
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderCanceled;
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExClient with default options
        /// </summary>
        public CoinExClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of CoinExClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public CoinExClient(CoinExClientOptions options): base("CoinEx", options, options.ApiCredentials == null ? null : new CoinExAuthenticationProvider(options.ApiCredentials, options.NonceProvider))
        {
            manualParseError = true;
            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;
        }
        #endregion

        #region methods
        #region public
        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(CoinExClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        /// <param name="nonceProvider">Optional nonce provider for signing requests. Careful providing a custom provider; once a nonce is sent to the server, every request after that needs a higher nonce than that</param>
        public void SetApiCredentials(string apiKey, string apiSecret, INonceProvider? nonceProvider = null)
        {
            SetAuthenticationProvider(new CoinExAuthenticationProvider(new ApiCredentials(apiKey, apiSecret), nonceProvider));
        }
                
        /// <summary>
        /// Gets a list of symbols active on CoinEx
        /// </summary>
        /// <returns>List of symbol names</returns>
        public async Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await Execute<IEnumerable<string>>(GetUrl(MarketListEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the exchange rates of currencies
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<Dictionary<string, decimal>>> GetCurrencyRateAsync(CancellationToken ct = default)
        {
            return await Execute<Dictionary<string, decimal>>(GetUrl(CurrencyRateEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the asset configs
        /// </summary>
        /// <param name="assetType">Optionally only return a certain type of asset, for example BCH</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<Dictionary<string, CoinExAssetConfig>>> GetAssetConfigAsync(string? assetType = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", assetType);

            return await Execute< Dictionary<string, CoinExAssetConfig>>(GetUrl(AssetConfigEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the state of a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the symbol</returns>
        public async Task<WebCallResult<CoinExSymbolState>> GetSymbolStateAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };

            return await Execute<CoinExSymbolState>(GetUrl(MarketStatisticsEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the states of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all symbols</returns>
        public async Task<WebCallResult<CoinExSymbolStatesList>> GetSymbolStatesAsync(CancellationToken ct = default)
        {
            var data = await Execute<CoinExSymbolStatesList>(GetUrl(MarketStatisticsListEndpoint), HttpMethod.Get, ct)
                .ConfigureAwait(false);
            if (!data)
                return data;

            foreach (var item in data.Data.Tickers)
                item.Value.Symbol = item.Key;
            return data;
        }

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order book for a symbol</returns>
        public async Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int mergeDepth, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit?.ValidateIntValues(nameof(limit), 5, 10, 20);

            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "merge", CoinExHelpers.MergeDepthIntToString(mergeDepth) }
            };
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<CoinExOrderBook>(GetUrl(MarketDepthEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the latest trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="fromId">The id from which on to return trades</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        public async Task<WebCallResult<IEnumerable<CoinExSymbolTrade>>> GetTradesHistoryAsync(string symbol, long? fromId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();

            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };
            parameters.AddOptionalParameter("last_id", fromId);

            return await Execute<IEnumerable<CoinExSymbolTrade>>(GetUrl(MarketDealsEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves market data for the exchange
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        public async Task<WebCallResult<Dictionary<string, CoinExMarket>>> GetMarketInfoAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };
            return await Execute<Dictionary<string, CoinExMarket>>(GetUrl(MarketInfoEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves market data for the exchange
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        public async Task<WebCallResult<Dictionary<string, CoinExMarket>>> GetMarketInfoAsync(CancellationToken ct = default)
        {
            return await Execute<Dictionary<string, CoinExMarket>>(GetUrl(MarketInfoEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves kline data for a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a symbol</returns>
        public async Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)) }
            };
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<IEnumerable<CoinExKline>>(GetUrl(MarketKlinesEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of balances</returns>
        public async Task<WebCallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            var result = await Execute<Dictionary<string, CoinExBalance>>(GetUrl(AccountInfoEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
            if (result)
            {
                foreach (var b in result.Data)
                    b.Value.Symbol = b.Key;
            }

            return result;
        }

        /// <summary>
        /// Retrieves a list of deposits. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
		public async Task<WebCallResult<CoinExPagedResult<CoinExDeposit>>> GetDepositHistoryAsync(string? coin = null, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", coin);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await ExecutePaged<CoinExDeposit>(GetUrl(DepositHistoryEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to get history for</param>
        /// <param name="coinWithdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<CoinExPagedResult<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? coin = null, long? coinWithdrawId = null, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", coin);
            parameters.AddOptionalParameter("coin_withdraw_id", coinWithdrawId);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await Execute<CoinExPagedResult<CoinExWithdrawal>>(GetUrl(WithdrawalHistoryEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        /// <summary>
        /// Withdraw coins from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coin">The coin to withdraw</param>
        /// <param name="localTransfer">Is it a local transfer between users or onchain</param>
        /// <param name="coinAddress">The address to withdraw to</param>
        /// <param name="amount">The amount to withdraw. This is the amount AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The withdrawal object</returns>
        public async Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string coin, string coinAddress, bool localTransfer, decimal amount, CancellationToken ct = default)
        {
            coin.ValidateNotNull(nameof(coin));
            coinAddress.ValidateNotNull(nameof(coinAddress));
            var parameters = new Dictionary<string, object>
            {
                { "coin_type", coin },
                { "coin_address", coinAddress },
                { "transfer_method", localTransfer ? "local": "onchain" },
                { "actual_amount", amount.ToString(CultureInfo.InvariantCulture) }
            };

            return await Execute<CoinExWithdrawal>(GetUrl(WithdrawEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// </summary>
        /// <param name="coinWithdrawId">The id of the withdrawal to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<WebCallResult<bool>> CancelWithdrawalAsync(long coinWithdrawId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "coin_withdraw_id", coinWithdrawId }
            };

            var result = await Execute<object>(GetUrl(CancelWithdrawalEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
            return result.As(result.Success);
        }

        /// <summary>
        /// Get the deposit address of an asset
        /// </summary>
        /// <param name="asset">The asset to deposit</param>
        /// <param name="smartContractName">Name of the network to use</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string? smartContractName = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("smart_contract_name", smartContractName);

            return await Execute<CoinExDepositAddress>(GetUrl(DepositAddressEndpoint + asset), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Places a limit order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="orderOption">Option for the order</param>
        /// <param name="clientId">Client id which can be used to match the order</param>
        /// <param name="sourceId">User defined number</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        public async Task<WebCallResult<CoinExOrder>> PlaceLimitOrderAsync(string symbol, TransactionType type, decimal amount, decimal price, OrderOption? orderOption = null, string? clientId = null, string? sourceId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("option", orderOption.HasValue ? JsonConvert.SerializeObject(orderOption, new OrderOptionConverter(false)): null);
            parameters.AddOptionalParameter("client_id", clientId);
            parameters.AddOptionalParameter("source_id", sourceId);

            var result = await Execute<CoinExOrder>(GetUrl(PlaceLimitOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                OnOrderPlaced?.Invoke(result.Data);

            return result;
        }

        /// <summary>
        /// Places a stop-limit order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="stopPrice">The stop-price of a single unit of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="orderOption">Option for the order</param>
        /// <param name="clientId">Client id which can be used to match the order</param>
        /// <param name="sourceId">User defined number</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        public async Task<WebCallResult<CoinExOrder>> PlaceStopLimitOrderAsync(string symbol, TransactionType type, decimal amount, decimal stopPrice, decimal price, OrderOption? orderOption = null, string? clientId = null, string? sourceId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) },
                { "stop_price", stopPrice.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("option", orderOption.HasValue ? JsonConvert.SerializeObject(orderOption, new OrderOptionConverter(false)) : null);
            parameters.AddOptionalParameter("client_id", clientId);
            parameters.AddOptionalParameter("source_id", sourceId);

            var result = await Execute<CoinExOrder>(GetUrl(PlaceStopLimitOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                OnOrderPlaced?.Invoke(result.Data);

            return result;
        }

        /// <summary>
        /// Places a stop-market order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="stopPrice">The stop-price of a single unit of the order</param>
        /// <param name="orderOption">Option for the order</param>
        /// <param name="clientId">Client id which can be used to match the order</param>
        /// <param name="sourceId">User defined number</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        public async Task<WebCallResult<CoinExOrder>> PlaceStopMarketOrderAsync(string symbol, TransactionType type, decimal amount, decimal stopPrice, OrderOption? orderOption = null, string? clientId = null, string? sourceId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "stop_price", stopPrice.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("option", orderOption.HasValue ? JsonConvert.SerializeObject(orderOption, new OrderOptionConverter(false)) : null);
            parameters.AddOptionalParameter("client_id", clientId);
            parameters.AddOptionalParameter("source_id", sourceId);

            var result = await Execute<CoinExOrder>(GetUrl(PlaceStopMarketOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                OnOrderPlaced?.Invoke(result.Data);

            return result;
        }

        /// <summary>
        /// Places a market order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order, specified in the base asset. For example on a ETHBTC symbol the value should be how much BTC should be spend to buy ETH</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        public async Task<WebCallResult<CoinExOrder>> PlaceMarketOrderAsync(string symbol, TransactionType type, decimal amount, string? sourceId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            var result = await Execute<CoinExOrder>(GetUrl(PlaceMarketOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                OnOrderPlaced?.Invoke(result.Data);
            return result;
        }

        /// <summary>
        /// Places an order which should be filled immediately up on placing, otherwise it will be canceled. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="type">Type of transaction</param>
        /// <param name="amount">The amount of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="sourceId">Client id which can be used to match the order</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<CoinExOrder>> PlaceImmediateOrCancelOrderAsync(string symbol, TransactionType type, decimal amount, decimal price, string? sourceId = null, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(type, new TransactionTypeConverter(false)) },
                { "amount", amount.ToString(CultureInfo.InvariantCulture) },
                { "price", price.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("source_id", sourceId);

            var result = await Execute<CoinExOrder>(GetUrl(PlaceImmediateOrCancelOrderEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                OnOrderPlaced?.Invoke(result.Data);
            return result;
        }

        /// <summary>
        /// Retrieves a list of open orders for a symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrder>(GetUrl(OpenOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of open stop orders for a symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenStopOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrder>(GetUrl(OpenStopOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of executed orders for a symbol in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a symbol</returns>
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetExecutedOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrder>(GetUrl(FinishedOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        public async Task<WebCallResult<CoinExOrder>> GetOrderAsync(long orderId, string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "id", orderId }
            };

            return await Execute<CoinExOrder>(GetUrl(OrderStatusEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrderTrade>>> GetOrderTradesAsync(long orderId, int page, int limit, CancellationToken ct = default)
        {
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "id", orderId },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrderTrade>(GetUrl(OrderDetailsEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a list of trades you executed on a specific symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrderTradeExtended>>> GetUserTradesAsync(string symbol, int page, int limit, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            limit.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page },
                { "limit", limit }
            };

            return await ExecutePaged<CoinExOrderTradeExtended>(GetUrl(UserTransactionsEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the canceled order</returns>
        public async Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "id", orderId }
            };

            var result = await Execute<CoinExOrder>(GetUrl(CancelOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
            if (result)
                OnOrderCanceled?.Invoke(result.Data);
            return result;
        }

        /// <summary>
        /// Cancels all orders. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the orders are on</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Execution statut</returns>
        public async Task<WebCallResult> CancelAllOrdersAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
            };

            return await Execute(GetUrl(CancelOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels all stop orders. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the orders are on</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Execution statut</returns>
        public async Task<WebCallResult> CancelAllStopOrdersAsync(string symbol, CancellationToken ct = default)
        {
            symbol.ValidateCoinExSymbol();
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
            };

            return await Execute(GetUrl(CancelStopOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Mining difficulty</returns>
        public async Task<WebCallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync(CancellationToken ct = default)
        {
            return await Execute<CoinExMiningDifficulty>(GetUrl(MiningDifficultyEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }
        #endregion

        #region private

        /// <inheritdoc />
        protected override Task<ServerError?> TryParseErrorAsync(JToken data)
        {
            if (data["code"] != null && data["message"] != null)
            {
                if (data["code"]!.Value<int>() != 0)
                {
                    return Task.FromResult((ServerError?)ParseErrorResponse(data));
                }
            }

            return Task.FromResult((ServerError?) null);
        }

        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (error["code"] == null || error["message"] == null)
                return new ServerError(error.ToString());

            return new ServerError((int)error["code"]!, (string)error["message"]!);
        }

        private async Task<WebCallResult<T>> Execute<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<T>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }
        private async Task<WebCallResult> Execute(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) 
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<object>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        private async Task<WebCallResult<CoinExPagedResult<T>>> ExecutePaged<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<CoinExPagedResult<T>>>(uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        private static WebCallResult<T> GetResult<T>(WebCallResult<CoinExApiResult<T>> result) where T : class
        {
            if (result.Error != null || result.Data == null)
                return WebCallResult<T>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error ?? new UnknownError("No data received"));

            return result.As(result.Data.Data);
        }

        private static WebCallResult GetResult(WebCallResult<CoinExApiResult<object>> result) 
        {
            if (result.Error != null || result.Data == null)
                return WebCallResult.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error ?? new UnknownError("No data received"));

            return new WebCallResult(result.ResponseStatusCode, result.ResponseHeaders, null);
        }
        private Uri GetUrl(string endpoint)
        {
            return new Uri(BaseAddress + endpoint);
        }
        #endregion
        #endregion

        #region common interface

        /// <summary>
        /// Get the name of a symbol for CoinEx based on the base and quote asset
        /// </summary>
        /// <param name="baseAsset"></param>
        /// <param name="quoteAsset"></param>
        /// <returns></returns>
        public string GetSymbolName(string baseAsset, string quoteAsset) => (baseAsset + quoteAsset).ToUpperInvariant();
#pragma warning disable 1066
        async Task<WebCallResult<IEnumerable<ICommonSymbol>>> IExchangeClient.GetSymbolsAsync()
        {
            var symbols = await GetMarketInfoAsync().ConfigureAwait(false);
            return symbols.As<IEnumerable<ICommonSymbol>>(symbols.Data?.Select(d => d.Value));
        }

        async Task<WebCallResult<ICommonTicker>> IExchangeClient.GetTickerAsync(string symbol)
        {
            var tickers = await GetSymbolStateAsync(symbol).ConfigureAwait(false);
            return tickers.As<ICommonTicker>(tickers.Data?.Ticker);
        }

        async Task<WebCallResult<IEnumerable<ICommonTicker>>> IExchangeClient.GetTickersAsync()
        {
            var tickers = await GetSymbolStatesAsync().ConfigureAwait(false);
            return tickers.As<IEnumerable<ICommonTicker>>(tickers.Data?.Tickers.Select(d => d.Value));
        }

        async Task<WebCallResult<IEnumerable<ICommonKline>>> IExchangeClient.GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null)
        {
            if(startTime != null || endTime != null)
                return WebCallResult<IEnumerable<ICommonKline>>.CreateErrorResult(new ArgumentError($"CoinEx does not support the {nameof(startTime)}/{nameof(endTime)} parameters for the method {nameof(IExchangeClient.GetKlinesAsync)}"));

            var klines = await GetKlinesAsync(symbol, GetKlineIntervalFromTimespan(timespan), limit).ConfigureAwait(false);
            return klines.As<IEnumerable<ICommonKline>>(klines.Data);
        }

        async Task<WebCallResult<ICommonOrderBook>> IExchangeClient.GetOrderBookAsync(string symbol)
        {
            var book = await GetOrderBookAsync(symbol, 0).ConfigureAwait(false);
            return book.As<ICommonOrderBook>(book.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonRecentTrade>>> IExchangeClient.GetRecentTradesAsync(string symbol)
        {
            var trades = await GetTradesHistoryAsync(symbol).ConfigureAwait(false);
            return trades.As<IEnumerable<ICommonRecentTrade>>(trades.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.PlaceOrderAsync(string symbol, IExchangeClient.OrderSide side, IExchangeClient.OrderType type, decimal quantity, decimal? price = null, string? accountId = null)
        {
            if(price == null && type == IExchangeClient.OrderType.Limit)
                return WebCallResult<ICommonOrderId>.CreateErrorResult(new ArgumentError($"Price parameter null while placing a limit order"));

            WebCallResult<CoinExOrder> result;
            if(type == IExchangeClient.OrderType.Limit)
                result = await PlaceLimitOrderAsync(symbol, side == IExchangeClient.OrderSide.Sell ? TransactionType.Sell: TransactionType.Buy, quantity, price!.Value).ConfigureAwait(false);
            else
                result = await PlaceMarketOrderAsync(symbol, side == IExchangeClient.OrderSide.Sell ? TransactionType.Sell : TransactionType.Buy, quantity).ConfigureAwait(false);

            return result.As<ICommonOrderId>(result.Data);
        }

        async Task<WebCallResult<ICommonOrder>> IExchangeClient.GetOrderAsync(string orderId, string? symbol = null)
        {
            if (string.IsNullOrEmpty(symbol))
                return WebCallResult<ICommonOrder>.CreateErrorResult(new ArgumentError($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(IExchangeClient.GetOrderAsync)}"));

            var order = await GetOrderAsync(long.Parse(orderId), symbol!).ConfigureAwait(false);
            return order.As<ICommonOrder>(order.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonTrade>>> IExchangeClient.GetTradesAsync(string orderId, string? symbol = null)
        {
            var result = await GetOrderTradesAsync(long.Parse(orderId), 1, 100).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonTrade>>(result.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetOpenOrdersAsync(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(IExchangeClient.GetOpenOrdersAsync)}");

            var openOrders = await GetOpenOrdersAsync(symbol!, 1, 100).ConfigureAwait(false);
            return openOrders.As<IEnumerable<ICommonOrder>>(openOrders.Data?.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonOrder>>> IExchangeClient.GetClosedOrdersAsync(string? symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException($"CoinEx needs the {nameof(symbol)} parameter for the method {nameof(IExchangeClient.GetClosedOrdersAsync)}");

            var result = await GetExecutedOrdersAsync(symbol!, 1, 100).ConfigureAwait(false);
            return result.As<IEnumerable<ICommonOrder>>(result.Data?.Data);
        }

        async Task<WebCallResult<ICommonOrderId>> IExchangeClient.CancelOrderAsync(string orderId, string? symbol)
        {
            if (symbol == null)
                return WebCallResult<ICommonOrderId>.CreateErrorResult(new ArgumentError(nameof(symbol) + " required for CoinEx " + nameof(IExchangeClient.CancelOrderAsync)));

            var result = await CancelOrderAsync(symbol, long.Parse(orderId)).ConfigureAwait(false);
            return result.As<ICommonOrderId>(result.Data);
        }

        async Task<WebCallResult<IEnumerable<ICommonBalance>>> IExchangeClient.GetBalancesAsync(string? accountId = null)
        {
            var balances = await GetBalancesAsync().ConfigureAwait(false);
            return balances.As<IEnumerable<ICommonBalance>>(balances.Data?.Select(d => d.Value));
        }
#pragma warning restore 1066

        private static KlineInterval GetKlineIntervalFromTimespan(TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.FromMinutes(1)) return KlineInterval.OneMinute;
            if (timeSpan == TimeSpan.FromMinutes(3)) return KlineInterval.ThreeMinute;
            if (timeSpan == TimeSpan.FromMinutes(5)) return KlineInterval.FiveMinute;
            if (timeSpan == TimeSpan.FromMinutes(15)) return KlineInterval.FiveMinute;
            if (timeSpan == TimeSpan.FromMinutes(30)) return KlineInterval.ThirtyMinute;
            if (timeSpan == TimeSpan.FromHours(1)) return KlineInterval.OneHour;
            if (timeSpan == TimeSpan.FromHours(2)) return KlineInterval.TwoHour;
            if (timeSpan == TimeSpan.FromHours(4)) return KlineInterval.FourHour;
            if (timeSpan == TimeSpan.FromHours(6)) return KlineInterval.SixHour;
            if (timeSpan == TimeSpan.FromHours(12)) return KlineInterval.TwelveHour;
            if (timeSpan == TimeSpan.FromDays(1)) return KlineInterval.OneDay;
            if (timeSpan == TimeSpan.FromDays(3)) return KlineInterval.ThreeDay;
            if (timeSpan == TimeSpan.FromDays(7)) return KlineInterval.OneWeek;

            throw new ArgumentException("Unsupported timespan for CoinEx Klines, check supported intervals using CoinEx.Net.Objects.KlineInterval");
        }
        #endregion
    }
}
