using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net;
using System.Collections.Generic;
using System.Linq;

namespace CoinEx.Net.Clients.SpotApiV2
{
    /// <inheritdoc />
    internal class CoinExRestClientSpotApiTrading : ICoinExRestClientSpotApiTrading
    {
        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiTrading(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> PlaceOrderAsync(
            string symbol, 
            AccountType accountType,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal? price = null,
            string? quantityAsset = null,
            string? clientOrderId = null,
            bool? hide = null,
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default)
        {
            clientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", accountType);
            parameters.AddEnum("side", side);
            parameters.AddEnum("type", type);
            parameters.AddString("amount", quantity);
            parameters.AddOptionalString("price", price);
            parameters.AddOptional("ccy", quantityAsset);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("is_hide", hide);
            parameters.AddOptionalEnum("stp_mode", stpMode);
            var result = await _baseClient.ExecuteAsync<CoinExOrder>(_baseClient.GetUri("v2/spot/order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderPlaced(new CryptoExchange.Net.CommonObjects.OrderId { Id = result.Data.Id.ToString(), SourceObject = result.Data });
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopId>> PlaceStopOrderAsync(
            string symbol,
            AccountType accountType,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal triggerPrice,
            decimal? price = null,
            string? quantityAsset = null,
            string? clientOrderId = null,
            bool? hide = null,
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default)
        {
            clientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", accountType);
            parameters.AddEnum("side", side);
            parameters.AddEnum("type", type);
            parameters.AddString("amount", quantity);
            parameters.AddString("trigger_price", triggerPrice);
            parameters.AddOptionalString("price", price);
            parameters.AddOptional("ccy", quantityAsset);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("is_hide", hide);
            parameters.AddOptionalEnum("stp_mode", stpMode);
            var result = await _baseClient.ExecuteAsync<CoinExStopId>(_baseClient.GetUri("v2/spot/stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderPlaced(new CryptoExchange.Net.CommonObjects.OrderId { Id = result.Data.StopOrderId.ToString(), SourceObject = result.Data });
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBatchOrderResult>>> PlaceMultipleOrdersAsync(
            IEnumerable<CoinExPlaceOrderRequest> requests,
            CancellationToken ct = default)
        {
            foreach(var order in requests)
                order.ClientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new ParameterCollection()
            {
                { "orders", requests }
            };
            var result = await _baseClient.ExecuteAsync<IEnumerable<CoinExBatchOrderResult>>(_baseClient.GetUri("v2/spot/batch-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
            {
                foreach(var order in result.Data.Where(x => x.Success))
                    _baseClient.InvokeOrderPlaced(new CryptoExchange.Net.CommonObjects.OrderId { Id = order.Id.ToString(), SourceObject = order });
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExStopId>>>> PlaceMultipleStopOrdersAsync(
            IEnumerable<CoinExPlaceStopOrderRequest> requests,
            CancellationToken ct = default)
        {
            foreach (var order in requests)
                order.ClientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new ParameterCollection()
            {
                { "orders", requests }
            };
            var result = await _baseClient.ExecuteAsync<IEnumerable<CoinExBatchResult<CoinExStopId>>>(_baseClient.GetUri("v2/spot/batch-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
            {
                foreach (var order in result.Data.Where(x => x.Success))
                    _baseClient.InvokeOrderPlaced(new CryptoExchange.Net.CommonObjects.OrderId { Id = order.Data!.StopOrderId.ToString(), SourceObject = order });
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            return await _baseClient.ExecuteAsync<CoinExOrder>(_baseClient.GetUri("v2/spot/order-status"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExOrder>>> GetOpenOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExOrder>(_baseClient.GetUri("v2/spot/pending-order"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExOrder>>> GetClosedOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExOrder>(_baseClient.GetUri("v2/spot/finished-order"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExStopOrder>(_baseClient.GetUri("v2/spot/pending-stop-order"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExStopOrder>(_baseClient.GetUri("v2/spot/finished-stop-order"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> EditOrderAsync(
            string symbol,
            AccountType accountType,
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
            parameters.AddEnum("market_type", accountType);
            parameters.AddString("amount", quantity);
            parameters.AddOptionalString("price", price);
            return await _baseClient.ExecuteAsync<CoinExOrder>(_baseClient.GetUri("v2/spot/modify-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopId>> EditStopOrderAsync(
            string symbol,
            AccountType accountType,
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
            parameters.AddEnum("market_type", accountType);
            parameters.AddString("amount", quantity);
            parameters.AddString("trigger_price", triggerPrice);
            parameters.AddOptionalString("price", price);
            return await _baseClient.ExecuteAsync<CoinExStopId>(_baseClient.GetUri("v2/spot/modify-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> CancelAllOrdersAsync(string symbol, AccountType accountType, OrderSide? side = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
            };
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptionalEnum("side", side);
            return await _baseClient.ExecuteAsync(_baseClient.GetUri("v2/spot/cancel-all-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, AccountType accountType, long orderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.AddEnum("market_type", accountType);
            var result = await _baseClient.ExecuteAsync<CoinExOrder>(_baseClient.GetUri("v2/spot/cancel-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderCanceled(new CryptoExchange.Net.CommonObjects.OrderId { Id = result.Data.Id.ToString(), SourceObject = result.Data });
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExOrder>>>> CancelOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_ids", orderIds }
            };
            var result = await _baseClient.ExecuteAsync<IEnumerable<CoinExBatchResult<CoinExOrder>>>(_baseClient.GetUri("v2/spot/cancel-batch-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
            {
                foreach(var order in result.Data)
                    _baseClient.InvokeOrderCanceled(new CryptoExchange.Net.CommonObjects.OrderId { Id = order.Data!.Id.ToString(), SourceObject = result.Data });
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopOrder>> CancelStopOrderAsync(string symbol, AccountType accountType, long stopOrderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "stop_id", stopOrderId }
            };
            parameters.AddEnum("market_type", accountType);
            var result = await _baseClient.ExecuteAsync<CoinExStopOrder>(_baseClient.GetUri("v2/spot/cancel-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderCanceled(new CryptoExchange.Net.CommonObjects.OrderId { Id = result.Data.StopOrderId.ToString(), SourceObject = result.Data });
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> CancelOrderByClientOrderIdAsync(string symbol, AccountType accountType, string clientOrderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "client_id", clientOrderId }
            };
            parameters.AddEnum("market_type", accountType);
            var result = await _baseClient.ExecuteAsync<CoinExOrder>(_baseClient.GetUri("v2/spot/cancel-order-by-client-id"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderCanceled(new CryptoExchange.Net.CommonObjects.OrderId { Id = result.Data.Id.ToString(), SourceObject = result.Data });
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopOrder>> CancelStopOrderByClientOrderIdAsync(string symbol, AccountType accountType, string clientStopOrderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "client_id", clientStopOrderId }
            };
            parameters.AddEnum("market_type", accountType);
            var result = await _baseClient.ExecuteAsync<CoinExStopOrder>(_baseClient.GetUri("v2/spot/cancel-stop-order-by-client-id"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderCanceled(new CryptoExchange.Net.CommonObjects.OrderId { Id = result.Data.StopOrderId.ToString(), SourceObject = result.Data });
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExStopOrder>>>> CancelStopOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "stop_ids", orderIds }
            };
            var result = await _baseClient.ExecuteAsync<IEnumerable<CoinExBatchResult<CoinExStopOrder>>>(_baseClient.GetUri("v2/spot/cancel-batch-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
            {
                foreach (var order in result.Data)
                    _baseClient.InvokeOrderCanceled(new CryptoExchange.Net.CommonObjects.OrderId { Id = order.Data!.StopOrderId.ToString(), SourceObject = result.Data });
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetUserTradesAsync(string symbol, AccountType accountType, OrderSide? side = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_Time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExUserTrade>(_baseClient.GetUri("v2/spot/user-deals"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetOrderTradesAsync(string symbol, AccountType accountType, long orderId, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExUserTrade>(_baseClient.GetUri("v2/spot/order-deals"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        // TODO Batch endpoints
    }
}
