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
using CryptoExchange.Net.Objects.Errors;

namespace CoinEx.Net.Clients.SpotApiV2
{
    /// <inheritdoc />
    internal class CoinExRestClientSpotApiTrading : ICoinExRestClientSpotApiTrading
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiTrading(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExOrder>> PlaceOrderAsync(
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
            clientOrderId = LibraryHelpers.ApplyBrokerId(
                clientOrderId,
                LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                32,
                _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("market_type", accountType);
            parameters.Add("side", side);
            parameters.Add("type", type);
            parameters.Add("amount", quantity);
            parameters.Add("price", price);
            parameters.Add("ccy", quantityAsset);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("is_hide", hide);
            parameters.Add("stp_mode", stpMode);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            var result = await _baseClient.SendAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExStopId>> PlaceStopOrderAsync(
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
            clientOrderId = LibraryHelpers.ApplyBrokerId(
                clientOrderId,
                LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                32,
                _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("market_type", accountType);
            parameters.Add("side", side);
            parameters.Add("type", type);
            parameters.Add("amount", quantity);
            parameters.Add("trigger_price", triggerPrice);
            parameters.Add("price", price);
            parameters.Add("ccy", quantityAsset);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("is_hide", hide);
            parameters.Add("stp_mode", stpMode);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/stop-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            var result = await _baseClient.SendAsync<CoinExStopId>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CallResult<CoinExBatchOrderResult>[]>> PlaceMultipleOrdersAsync(
            IEnumerable<CoinExPlaceOrderRequest> requests,
            CancellationToken ct = default)
        {
            foreach (var order in requests)
            {
                order.ClientOrderId = LibraryHelpers.ApplyBrokerId(
                    order.ClientOrderId,
                    LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                    32,
                    _baseClient.ClientOptions.AllowAppendingClientOrderId);
            }

            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "orders", requests.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/batch-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            var resultData = await _baseClient.SendAsync<CoinExBatchResult<CoinExBatchOrderResult>[]>(request, parameters, ct, weight: requests.Count()).ConfigureAwait(false);
            if (!resultData.Success)
                return HttpResult.Fail<CallResult<CoinExBatchOrderResult>[]>(resultData);

            var result = new List<CallResult<CoinExBatchOrderResult>>();
            foreach (var item in resultData.Data!)
            {
                if (item.Code != 0)
                    result.Add(CallResult<CoinExBatchOrderResult>.Fail(new ServerError(item.Code, _baseClient.GetErrorInfo(item.Code, item.Message!))));
                else
                    result.Add(CallResult<CoinExBatchOrderResult>.Ok(item.Data!));
            }

            if (result.All(x => !x.Success))
                return HttpResult.Fail<CallResult<CoinExBatchOrderResult>[]>(resultData, new ServerError(new ErrorInfo (ErrorType.AllOrdersFailed, "All orders failed")), result.ToArray());

            return HttpResult.Ok(resultData, result.ToArray());
        }

        /// <inheritdoc />
        public async Task<HttpResult<CallResult<CoinExStopId>[]>> PlaceMultipleStopOrdersAsync(
            IEnumerable<CoinExPlaceStopOrderRequest> requests,
            CancellationToken ct = default)
        {
            foreach (var order in requests)
            {
                order.ClientOrderId = LibraryHelpers.ApplyBrokerId(
                    order.ClientOrderId,
                    LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                    32,
                    _baseClient.ClientOptions.AllowAppendingClientOrderId);
            }

            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "orders", requests.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/batch-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            var resultData = await _baseClient.SendAsync<CoinExBatchResult<CoinExStopId>[]>(request, parameters, ct, weight: requests.Count()).ConfigureAwait(false);
            if (!resultData.Success)
                return HttpResult.Fail<CallResult<CoinExStopId>[]>(resultData);

            var result = new List<CallResult<CoinExStopId>>();
            foreach (var item in resultData.Data!)
            {
                if (item.Code != 0)
                    result.Add(CallResult<CoinExStopId>.Fail(new ServerError(item.Code, _baseClient.GetErrorInfo(item.Code, item.Message!))));
                else
                    result.Add(CallResult<CoinExStopId>.Ok(item.Data!));
            }

            if (result.All(x => !x.Success))
                return HttpResult.Fail<CallResult<CoinExStopId>[]>(resultData, new ServerError(new ErrorInfo(ErrorType.AllOrdersFailed, "All orders failed")), result.ToArray());

            return HttpResult.Ok(resultData, result.ToArray());
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/order-status", CoinExExchange.RateLimiter.CoinExRestSpotQuery, 1, true);
            return await _baseClient.SendAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExOrder>>> GetOpenOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market_type", accountType);
            parameters.Add("side", side);
            parameters.Add("market", symbol);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);

            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/pending-order", CoinExExchange.RateLimiter.CoinExRestSpotQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExOrder>>> GetClosedOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market_type", accountType);
            parameters.Add("side", side);
            parameters.Add("market", symbol);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);

            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/finished-order", CoinExExchange.RateLimiter.CoinExRestSpotHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            if (clientOrderId != null)
            {
                clientOrderId = LibraryHelpers.ApplyBrokerId(
                    clientOrderId,
                    LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                    32,
                    _baseClient.ClientOptions.AllowAppendingClientOrderId);
            }

            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market_type", accountType);
            parameters.Add("side", side);
            parameters.Add("market", symbol);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/pending-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market_type", accountType);
            parameters.Add("side", side);
            parameters.Add("market", symbol);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/finished-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExOrder>> EditOrderAsync(
            string symbol,
            AccountType accountType,
            long orderId,
            decimal quantity,
            decimal? price = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.Add("market_type", accountType);
            parameters.Add("amount", quantity);
            parameters.Add("price", price);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/modify-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            return await _baseClient.SendAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExStopId>> EditStopOrderAsync(
            string symbol,
            AccountType accountType,
            long stopOrderId,
            decimal quantity,
            decimal triggerPrice,
            decimal? price = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "stop_id", stopOrderId }
            };
            parameters.Add("market_type", accountType);
            parameters.Add("amount", quantity);
            parameters.Add("trigger_price", triggerPrice);
            parameters.Add("price", price);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/modify-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            return await _baseClient.SendAsync<CoinExStopId>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult> CancelAllOrdersAsync(string symbol, AccountType accountType, OrderSide? side = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
            };
            parameters.Add("market_type", accountType);
            parameters.Add("side", side);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/cancel-all-order", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExOrder>> CancelOrderAsync(string symbol, AccountType accountType, long orderId, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.Add("market_type", accountType);

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/cancel-order", CoinExExchange.RateLimiter.CoinExRestSpotCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBatchResult<CoinExOrder>[]>> CancelOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_ids", orderIds.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/cancel-batch-order", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExBatchResult<CoinExOrder>[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExStopOrder>> CancelStopOrderAsync(string symbol, AccountType accountType, long stopOrderId, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "stop_id", stopOrderId }
            };
            parameters.Add("market_type", accountType);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/cancel-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExOrder[]>> CancelOrdersByClientOrderIdAsync(string symbol, AccountType accountType, string clientOrderId, CancellationToken ct = default)
        {
            clientOrderId = LibraryHelpers.ApplyBrokerId(
                clientOrderId,
                LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                32,
                _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "client_id", clientOrderId }
            };
            parameters.Add("market_type", accountType);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/cancel-order-by-client-id", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExOrder[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExStopOrder[]>> CancelStopOrdersByClientOrderIdAsync(string symbol, AccountType accountType, string clientStopOrderId, CancellationToken ct = default)
        {
            clientStopOrderId = LibraryHelpers.ApplyBrokerId(
                clientStopOrderId,
                LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                32,
                _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "client_id", clientStopOrderId }
            };
            parameters.Add("market_type", accountType);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/cancel-stop-order-by-client-id", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExStopOrder[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBatchResult<CoinExStopOrder>[]>> CancelStopOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "stop_ids", orderIds.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/spot/cancel-batch-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExBatchResult<CoinExStopOrder>[]>(request, parameters, ct, weight: orderIds.Count()).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExUserTrade>>> GetUserTradesAsync(string symbol, AccountType accountType, OrderSide? side = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("market_type", accountType);
            parameters.Add("side", side);
            parameters.Add("start_time", startTime);
            parameters.Add("end_Time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/user-deals", CoinExExchange.RateLimiter.CoinExRestSpotHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExUserTrade>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExUserTrade>>> GetOrderTradesAsync(string symbol, AccountType accountType, long orderId, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.Add("market_type", accountType);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/order-deals", CoinExExchange.RateLimiter.CoinExRestSpotHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExUserTrade>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
