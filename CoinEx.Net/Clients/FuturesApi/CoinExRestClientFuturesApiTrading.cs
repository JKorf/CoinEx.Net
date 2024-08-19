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

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc />
    internal class CoinExRestClientFuturesApiTrading : ICoinExRestClientFuturesApiTrading
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
            SelfTradePreventionMode? stpMode = null,
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
            parameters.AddOptionalEnum("stp_mode", stpMode);
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
            SelfTradePreventionMode? stpMode = null,
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
            parameters.AddOptionalEnum("stp_mode", stpMode);
            return await _baseClient.ExecuteAsync<CoinExStopId>(_baseClient.GetUri("v2/futures/stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExFuturesOrder>>>> PlaceMultipleOrdersAsync(
            IEnumerable<CoinExFuturesPlaceOrderRequest> requests,
            CancellationToken ct = default)
        {
            foreach (var order in requests)
                order.ClientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new ParameterCollection()
            {
                { "orders", requests }
            };
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExBatchResult<CoinExFuturesOrder>>>(_baseClient.GetUri("v2/futures/batch-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExStopId>>>> PlaceMultipleStopOrdersAsync(
            IEnumerable<CoinExFuturesPlaceStopOrderRequest> requests,
            CancellationToken ct = default)
        {
            foreach (var order in requests)
                order.ClientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new ParameterCollection()
            {
                { "orders", requests }
            };
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExBatchResult<CoinExStopId>>>(_baseClient.GetUri("v2/futures/batch-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<CoinExStopOrder>(_baseClient.GetUri("v2/futures/cancel-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            
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
        public async Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExFuturesOrder>>>> CancelOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_ids", orderIds }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExBatchResult<CoinExFuturesOrder>>>(_baseClient.GetUri("v2/futures/cancel-batch-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExStopOrder>>>> CancelStopOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "stop_ids", orderIds }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExBatchResult<CoinExStopOrder>>>(_baseClient.GetUri("v2/futures/cancel-batch-stop-order"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExPosition>>> GetPositionsAsync(string? symbol = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExPosition>(_baseClient.GetUri("v2/futures/pending-position"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExPosition>(_baseClient.GetUri("v2/futures/finished-position"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesOrder>> ClosePositionAsync(string symbol, OrderTypeV2 orderType, decimal? price = null, decimal? quantity = null, string? clientOrderId = null, bool? hidden = null, CancellationToken ct = default)
        {
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
            return await _baseClient.ExecuteAsync<CoinExFuturesOrder>(_baseClient.GetUri("v2/futures/close-position"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<CoinExPosition>(_baseClient.GetUri("v2/futures/adjust-position-margin"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> SetStopLossAsync(string symbol, PriceType stopLossType, decimal stopLossPrice, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddEnum("stop_loss_type", stopLossType);
            parameters.AddString("stop_loss_price", stopLossPrice);
            return await _baseClient.ExecuteAsync<CoinExPosition>(_baseClient.GetUri("v2/futures/set-position-stop-loss"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPosition>> SetTakeProfitAsync(string symbol, PriceType takeProfitType, decimal takeProfitPrice, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            parameters.AddEnum("take_profit_type", takeProfitType);
            parameters.AddString("take_profit_price", takeProfitPrice);
            return await _baseClient.ExecuteAsync<CoinExPosition>(_baseClient.GetUri("v2/futures/set-position-take-profit"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExPositionMargin>(_baseClient.GetUri("v2/futures/position-margin-history"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExPositionFundingRate>(_baseClient.GetUri("v2/futures/position-funding-history"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExPositionAdl>(_baseClient.GetUri("v2/futures/position-adl-history"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExPositionSettlement>(_baseClient.GetUri("v2/futures/position-settle-history"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
        // TODO Batch endpoints
    }
}
