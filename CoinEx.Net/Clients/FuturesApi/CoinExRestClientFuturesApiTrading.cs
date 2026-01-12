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
        public async Task<WebCallResult<CoinExFuturesOrder>> PlaceOrderAsync(
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
            parameters.AddOptionalEnum("stp_mode", stpMode);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
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
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default)
        {
            clientOrderId = LibraryHelpers.ApplyBrokerId(
                clientOrderId,
                LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                32,
                _baseClient.ClientOptions.AllowAppendingClientOrderId);

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
            parameters.AddOptionalEnum("stp_mode", stpMode);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExStopId>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CallResult<CoinExFuturesOrder>[]>> PlaceMultipleOrdersAsync(
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

            var parameters = new ParameterCollection()
            {
                { "orders", requests.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/batch-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            var resultData = await _baseClient.SendAsync<CoinExBatchResult<CoinExFuturesOrder>[]>(request, parameters, ct, weight: requests.Count()).ConfigureAwait(false);
            if (!resultData)
                return resultData.As<CallResult<CoinExFuturesOrder>[]>(default);

            var result = new List<CallResult<CoinExFuturesOrder>>();
            foreach (var item in resultData.Data!)
            {
                if (item.Code != 0)
                    result.Add(new CallResult<CoinExFuturesOrder>(new ServerError(item.Code, _baseClient.GetErrorInfo(item.Code, item.Message!))));
                else
                    result.Add(new CallResult<CoinExFuturesOrder>(item.Data!));
            }

            if (result.All(x => !x.Success))
                return resultData.AsErrorWithData(new ServerError(new ErrorInfo(ErrorType.AllOrdersFailed, "All orders failed")), result.ToArray());

            return resultData.As(result.ToArray());
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CallResult<CoinExStopId>[]>> PlaceMultipleStopOrdersAsync(
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

            var parameters = new ParameterCollection()
            {
                { "orders", requests.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/batch-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            var resultData = await _baseClient.SendAsync<CoinExBatchResult<CoinExStopId>[]>(request, parameters, ct, weight: requests.Count()).ConfigureAwait(false);
            if (!resultData)
                return resultData.As<CallResult<CoinExStopId>[]>(default);

            var result = new List<CallResult<CoinExStopId>>();
            foreach (var item in resultData.Data!)
            {
                if (item.Code != 0)
                    result.Add(new CallResult<CoinExStopId>(new ServerError(item.Code, _baseClient.GetErrorInfo(item.Code, item.Message!))));
                else
                    result.Add(new CallResult<CoinExStopId>(item.Data!));
            }

            if (result.All(x => !x.Success))
                return resultData.AsErrorWithData(new ServerError(new ErrorInfo(ErrorType.AllOrdersFailed, "All orders failed")), result.ToArray());

            return resultData.As(result.ToArray());
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_id", orderId }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/order-status", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExFuturesOrder>>> GetOpenOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            if (clientOrderId != null)
            {
                clientOrderId = LibraryHelpers.ApplyBrokerId(
                    clientOrderId,
                    LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                    32,
                    _baseClient.ClientOptions.AllowAppendingClientOrderId);
            }

            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/pending-order", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/finished-order", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            if (clientOrderId != null)
            {
                clientOrderId = LibraryHelpers.ApplyBrokerId(
                    clientOrderId,
                    LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                    32,
                    _baseClient.ClientOptions.AllowAppendingClientOrderId);
            }

            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/pending-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(string? symbol = null, OrderSide? side = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/finished-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/modify-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/modify-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExStopId>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-all-order", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-order", CoinExExchange.RateLimiter.CoinExRestFuturesCancel, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
            
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesCancel, 1, true);
            return await _baseClient.SendAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBatchResult<CoinExFuturesOrder>[]>> CancelOrderByClientOrderIdAsync(string symbol, string clientOrderId, CancellationToken ct = default)
        {
            clientOrderId = LibraryHelpers.ApplyBrokerId(
                clientOrderId,
                LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                32,
                _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "client_id", clientOrderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-order-by-client-id", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync<CoinExBatchResult<CoinExFuturesOrder>[]>(request, parameters, ct).ConfigureAwait(false);
            
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBatchResult<CoinExStopOrder>[]>> CancelStopOrderByClientOrderIdAsync(string symbol, string clientStopOrderId, CancellationToken ct = default)
        {
            clientStopOrderId = LibraryHelpers.ApplyBrokerId(
                clientStopOrderId,
                LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                32,
                _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "client_id", clientStopOrderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-stop-order-by-client-id", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync<CoinExBatchResult<CoinExStopOrder>[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBatchResult<CoinExFuturesOrder>[]>> CancelOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_ids", orderIds.ToArray() }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-batch-order", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync<CoinExBatchResult<CoinExFuturesOrder>[]>(request, parameters, ct, weight: orderIds.Count()).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBatchResult<CoinExStopOrder>[]>> CancelStopOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "stop_ids", orderIds.ToArray() }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-batch-stop-order", CoinExExchange.RateLimiter.CoinExRestFuturesBatchCancel, 1, true);
            return await _baseClient.SendAsync<CoinExBatchResult<CoinExStopOrder>[]>(request, parameters, ct, weight: orderIds.Count()).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/user-deals", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExUserTrade>(request, parameters, ct).ConfigureAwait(false);
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

            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/order-deals", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExUserTrade>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExPosition>>> GetPositionsAsync(string? symbol = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/pending-position", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExPosition>>> GetPositionHistoryAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/finished-position", CoinExExchange.RateLimiter.CoinExRestFuturesHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesOrder>> ClosePositionAsync(string symbol, OrderTypeV2 orderType, decimal? price = null, decimal? quantity = null, string? clientOrderId = null, bool? hidden = null, CancellationToken ct = default)
        {
            clientOrderId = LibraryHelpers.ApplyBrokerId(
                clientOrderId,
                LibraryHelpers.GetClientReference(() => _baseClient.ClientOptions.BrokerId, _baseClient.Exchange),
                32,
                _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddEnum("type", orderType);
            parameters.AddOptionalString("price", price);
            parameters.AddOptionalString("amount", quantity);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("is_hide", hidden);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/close-position", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExFuturesOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> AdjustPositionMarginAsync(string symbol, decimal quantity, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddString("amount", quantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/adjust-position-margin", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> SetStopLossAsync(string symbol, PriceType stopLossType, decimal stopLossPrice, decimal? stopLossQuantity = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddEnum("stop_loss_type", stopLossType);
            parameters.AddString("stop_loss_price", stopLossPrice);
            parameters.AddOptionalString("stop_loss_amount", stopLossQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/set-position-stop-loss", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> EditStopLossAsync(string symbol, long stopLossOrderId, PriceType? stopLossType = null, decimal? stopLossPrice = null, decimal? stopLossQuantity = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "stop_loss_id", stopLossOrderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("stop_loss_type", stopLossType);
            parameters.AddOptionalString("stop_loss_price", stopLossPrice);
            parameters.AddOptionalString("stop_loss_amount", stopLossQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/modify-position-stop-loss", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> CancelStopLossAsync(string symbol, long? stopLossOrderId = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
            };
            parameters.AddOptional("stop_loss_id", stopLossOrderId);
            parameters.AddEnum("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-position-stop-loss", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> SetTakeProfitAsync(string symbol, PriceType takeProfitType, decimal takeProfitPrice, decimal? takeProfitQuantity = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddEnum("take_profit_type", takeProfitType);
            parameters.AddString("take_profit_price", takeProfitPrice);
            parameters.AddOptionalString("take_profit_amount", takeProfitQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/set-position-take-profit", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> EditTakeProfitAsync(string symbol, long takeProfitOrderId, PriceType? takeProfitType = null, decimal? takeProfitPrice = null, decimal? takeProfitQuantity = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "take_profit_id", takeProfitOrderId }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalEnum("take_profit_type", takeProfitType);
            parameters.AddOptionalString("take_profit_price", takeProfitPrice);
            parameters.AddOptionalString("take_profit_amount", takeProfitQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/modify-position-take-profit", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> CancelTakeProfitAsync(string symbol, long? takeProfitOrderId = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
            };
            parameters.AddOptional("take_profit_id", takeProfitOrderId);
            parameters.AddEnum("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/cancel-position-take-profit", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExPosition>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExPositionMargin>>> GetMarginHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "position_id", positionId },
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/position-margin-history", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPositionMargin>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExPositionFundingRate>>> GetFundingRateHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "position_id", positionId },
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/position-funding-history", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPositionFundingRate>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExPositionAdl>>> GetAutoDeleverageHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "position_id", positionId },
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/position-adl-history", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPositionAdl>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExPositionSettlement>>> GetAutoSettlementHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "position_id", positionId },
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/position-settle-history", CoinExExchange.RateLimiter.CoinExRestFuturesQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExPositionSettlement>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
