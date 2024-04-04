using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net;
using CoinEx.Net.Interfaces.Clients.FuturesApi;

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc />
    public class CoinExRestClientFuturesApiTrading : ICoinExRestClientFuturesApiTrading
    {
        private readonly CoinExRestClientFuturesApi _baseClient;

        internal CoinExRestClientFuturesApiTrading(CoinExRestClientFuturesApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesOrder>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal? price = null,
            string? clientOrderId = null,
            bool? hide = null,
            CancellationToken ct = default)
        {
            clientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddEnum("side", side);
            parameters.AddEnum("type", type);
            parameters.AddString("amount", quantity);
            parameters.AddOptionalString("price", price);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("is_hide", hide);
            return await _baseClient.ExecuteAsync<CoinExFuturesOrder>(_baseClient.GetUri("v2/futures/order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopId>> PlaceStopOrderAsync(
            string symbol,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal triggerPrice,
            TriggerPriceType triggerPriceType,
            decimal? price = null,
            string? clientOrderId = null,
            bool? hide = null,
            CancellationToken ct = default)
        {
            clientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddEnum("side", side);
            parameters.AddEnum("type", type);
            parameters.AddEnum("trigger_price_type", triggerPriceType);
            parameters.AddString("amount", quantity);
            parameters.AddString("trigger_price", triggerPrice);
            parameters.AddOptionalString("price", price);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("is_hide", hide);
            return await _baseClient.ExecuteAsync<CoinExStopId>(_baseClient.GetUri("v2/futures/stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            return await _baseClient.ExecuteAsync<CoinExFuturesOrder>(_baseClient.GetUri("v2/futures/order-status"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExFuturesOrder>>> GetOpenOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExFuturesOrder>(_baseClient.GetUri("v2/futures/pending-order"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExFuturesOrder>>> GetClosedOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExFuturesOrder>(_baseClient.GetUri("v2/futures/finished-order"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExStopOrder>(_baseClient.GetUri("v2/futures/pending-stop-order"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExStopOrder>(_baseClient.GetUri("v2/futures/finished-stop-order"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesOrder>> EditOrderAsync(
            string symbol,
            long orderId,
            decimal quantity,
            decimal? price = null,
            CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddString("amount", quantity);
            parameters.AddOptionalString("price", price);
            return await _baseClient.ExecuteAsync<CoinExFuturesOrder>(_baseClient.GetUri("v2/futures/modify-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopId>> EditStopOrderAsync(
            string symbol,
            long stopOrderId,
            decimal quantity,
            decimal triggerPrice,
            decimal? price = null,
            CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "stop_id", stopOrderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddString("amount", quantity);
            parameters.AddString("trigger_price", triggerPrice);
            parameters.AddOptionalString("price", price);
            return await _baseClient.ExecuteAsync<CoinExStopId>(_baseClient.GetUri("v2/futures/modify-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> CancelAllOrdersAsync(string symbol, OrderSide? side = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            return await _baseClient.ExecuteAsync(_baseClient.GetUri("v2/futures/cancel-all-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            return await _baseClient.ExecuteAsync<CoinExFuturesOrder>(_baseClient.GetUri("v2/futures/cancel-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopOrder>> CancelStopOrderAsync(string symbol, long stopOrderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "stop_id", stopOrderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            return await _baseClient.ExecuteAsync<CoinExStopOrder>(_baseClient.GetUri("v2/spot/cancel-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesOrder>> CancelOrderByClientOrderIdAsync(string symbol, string clientOrderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "client_id", clientOrderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            return await _baseClient.ExecuteAsync<CoinExFuturesOrder>(_baseClient.GetUri("v2/futures/cancel-order-by-client-id"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopOrder>> CancelStopOrderByClientOrderIdAsync(string symbol, string clientStopOrderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "client_id", clientStopOrderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            return await _baseClient.ExecuteAsync<CoinExStopOrder>(_baseClient.GetUri("v2/futures/cancel-stop-order-by-client-id"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetUserTradesAsync(string symbol, OrderSide? side = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_Time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExUserTrade>(_baseClient.GetUri("v2/futures/user-deals"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetOrderTradesAsync(string symbol, long orderId, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExUserTrade>(_baseClient.GetUri("v2/futures/order-deals"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        // TODO Batch endpoints
    }
}
