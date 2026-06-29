using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;
using CryptoExchange.Net;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using System.Collections.Generic;
using System.Linq;
using CryptoExchange.Net.Objects.Errors;

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc />
    internal class CoinExRestClientFuturesApiTrading : ICoinExRestClientFuturesApiTrading
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly CoinExRestClientFuturesApi _baseClient;

        internal CoinExRestClientFuturesApiTrading(CoinExRestClientFuturesApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExFuturesOrder>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal? price = null,
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
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("side", side);
            parameters.Add("type", type);
            parameters.Add("amount", quantity);
            parameters.Add("price", price);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("is_hide", hide);
            parameters.Add("stp_mode", stpMode);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExStopId>> PlaceStopOrderAsync(
            string symbol,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal triggerPrice,
            TriggerPriceType triggerPriceType,
            decimal? price = null,
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
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("side", side);
            parameters.Add("type", type);
            parameters.Add("trigger_price_type", triggerPriceType);
            parameters.Add("amount", quantity);
            parameters.Add("trigger_price", triggerPrice);
            parameters.Add("price", price);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("is_hide", hide);
            parameters.Add("stp_mode", stpMode);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExStopId>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CallResult<CoinExFuturesOrder>[]>> PlaceMultipleOrdersAsync(
            IEnumerable<CoinExFuturesPlaceOrderRequest> requests,
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/batch-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            var resultData = await _baseClient.SendAsync<CoinExBatchResult<CoinExFuturesOrder>[]>(request, parameters, ct, weight: requests.Count()).ConfigureAwait(false);
            if (!resultData.Success)
                return HttpResult.Fail<CallResult<CoinExFuturesOrder>[]>(resultData);

            var result = new List<CallResult<CoinExFuturesOrder>>();
            foreach (var item in resultData.Data!)
            {
                if (item.Code != 0)
                    result.Add(CallResult<CoinExFuturesOrder>.Fail(new ServerError(item.Code, _baseClient.GetErrorInfo(item.Code, item.Message!))));
                else
                    result.Add(CallResult<CoinExFuturesOrder>.Ok(item.Data!));
            }

            if (result.All(x => !x.Success))
                return HttpResult.Fail<CallResult<CoinExFuturesOrder>[]>(resultData, new ServerError(new ErrorInfo(ErrorType.AllOrdersFailed, "All orders failed")), result.ToArray());

            return HttpResult.Ok(resultData, result.ToArray());
        }

        /// <inheritdoc />
        public async Task<HttpResult<CallResult<CoinExStopId>[]>> PlaceMultipleStopOrdersAsync(
            IEnumerable<CoinExFuturesPlaceStopOrderRequest> requests,
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/batch-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
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
                return HttpResult.Fail(resultData, new ServerError(new ErrorInfo(ErrorType.AllOrdersFailed, "All orders failed")), result.ToArray());

            return HttpResult.Ok(resultData, result.ToArray());
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExFuturesOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/order-status", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExFuturesOrder>>> GetOpenOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
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
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("side", side);
            parameters.Add("market", symbol);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/pending-order", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExFuturesOrder>>> GetClosedOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
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
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("side", side);
            parameters.Add("market", symbol);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/finished-order", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
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
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("side", side);
            parameters.Add("market", symbol);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/pending-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(string? symbol = null, OrderSide? side = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("side", side);
            parameters.Add("market", symbol);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/finished-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExFuturesOrder>> EditOrderAsync(
            string symbol,
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
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("amount", quantity);
            parameters.Add("price", price);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/modify-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExStopId>> EditStopOrderAsync(
            string symbol,
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
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("amount", quantity);
            parameters.Add("trigger_price", triggerPrice);
            parameters.Add("price", price);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/modify-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExStopId>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult> CancelAllOrdersAsync(string symbol, OrderSide? side = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("side", side);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-all-order", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExFuturesOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.Add("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-order", CoinExExchange.RateLimiter.CoinExRestFuturesCancel, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExStopOrder>> CancelStopOrderAsync(string symbol, long stopOrderId, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "stop_id", stopOrderId }
            };
            parameters.Add("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesCancel, 1, true);
            return await _baseClient.SendAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBatchResult<CoinExFuturesOrder>[]>> CancelOrderByClientOrderIdAsync(string symbol, string clientOrderId, CancellationToken ct = default)
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
            parameters.Add("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-order-by-client-id", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync<CoinExBatchResult<CoinExFuturesOrder>[]>(request, parameters, ct).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBatchResult<CoinExStopOrder>[]>> CancelStopOrderByClientOrderIdAsync(string symbol, string clientStopOrderId, CancellationToken ct = default)
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
            parameters.Add("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-stop-order-by-client-id", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync<CoinExBatchResult<CoinExStopOrder>[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBatchResult<CoinExFuturesOrder>[]>> CancelOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_ids", orderIds.ToArray() }
            };
            parameters.Add("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-batch-order", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync<CoinExBatchResult<CoinExFuturesOrder>[]>(request, parameters, ct, weight: orderIds.Count()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBatchResult<CoinExStopOrder>[]>> CancelStopOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "stop_ids", orderIds.ToArray() }
            };
            parameters.Add("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-batch-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync<CoinExBatchResult<CoinExStopOrder>[]>(request, parameters, ct, weight: orderIds.Count()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExUserTrade>>> GetUserTradesAsync(string symbol, OrderSide? side = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("side", side);
            parameters.Add("start_time", startTime);
            parameters.Add("end_Time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/user-deals", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExUserTrade>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExUserTrade>>> GetOrderTradesAsync(string symbol, long orderId, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);

            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/order-deals", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExUserTrade>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExPosition>>> GetPositionsAsync(string? symbol = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/pending-position", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExPosition>>> GetPositionHistoryAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("start_time", startTime);
            parameters.Add("end_time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/finished-position", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExFuturesOrder>> ClosePositionAsync(string symbol, OrderTypeV2 orderType, decimal? price = null, decimal? quantity = null, string? clientOrderId = null, bool? hidden = null, CancellationToken ct = default)
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
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("type", orderType);
            parameters.Add("price", price);
            parameters.Add("amount", quantity);
            parameters.Add("client_id", clientOrderId);
            parameters.Add("is_hide", hidden);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/close-position", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPosition>> AdjustPositionMarginAsync(string symbol, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("amount", quantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/adjust-position-margin", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPosition>> SetStopLossAsync(string symbol, PriceType stopLossType, decimal stopLossPrice, decimal? stopLossQuantity = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("stop_loss_type", stopLossType);
            parameters.Add("stop_loss_price", stopLossPrice);
            parameters.Add("stop_loss_amount", stopLossQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/set-position-stop-loss", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPosition>> EditStopLossAsync(string symbol, long stopLossOrderId, PriceType? stopLossType = null, decimal? stopLossPrice = null, decimal? stopLossQuantity = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "stop_loss_id", stopLossOrderId }
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("stop_loss_type", stopLossType);
            parameters.Add("stop_loss_price", stopLossPrice);
            parameters.Add("stop_loss_amount", stopLossQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/modify-position-stop-loss", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPosition>> CancelStopLossAsync(string symbol, long? stopLossOrderId = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
            };
            parameters.Add("stop_loss_id", stopLossOrderId);
            parameters.Add("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-position-stop-loss", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPosition>> SetTakeProfitAsync(string symbol, PriceType takeProfitType, decimal takeProfitPrice, decimal? takeProfitQuantity = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("take_profit_type", takeProfitType);
            parameters.Add("take_profit_price", takeProfitPrice);
            parameters.Add("take_profit_amount", takeProfitQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/set-position-take-profit", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPosition>> EditTakeProfitAsync(string symbol, long takeProfitOrderId, PriceType? takeProfitType = null, decimal? takeProfitPrice = null, decimal? takeProfitQuantity = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "take_profit_id", takeProfitOrderId }
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("take_profit_type", takeProfitType);
            parameters.Add("take_profit_price", takeProfitPrice);
            parameters.Add("take_profit_amount", takeProfitQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/modify-position-take-profit", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPosition>> CancelTakeProfitAsync(string symbol, long? takeProfitOrderId = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
            };
            parameters.Add("take_profit_id", takeProfitOrderId);
            parameters.Add("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/futures/cancel-position-take-profit", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExPositionMargin>>> GetMarginHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "position_id", positionId },
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("start_time", startTime);
            parameters.Add("end_time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/position-margin-history", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPositionMargin>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExPositionFundingRate>>> GetFundingRateHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "position_id", positionId },
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("start_time", startTime);
            parameters.Add("end_time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/position-funding-history", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPositionFundingRate>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExPositionAdl>>> GetAutoDeleverageHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "position_id", positionId },
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("start_time", startTime);
            parameters.Add("end_time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/position-adl-history", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPositionAdl>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExPositionSettlement>>> GetAutoSettlementHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "position_id", positionId },
            };
            parameters.Add("market_type", AccountType.Futures);
            parameters.Add("start_time", startTime);
            parameters.Add("end_time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/futures/position-settle-history", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPositionSettlement>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
