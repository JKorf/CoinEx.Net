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
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
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
            clientOrderId = LibraryHelpers.ApplyBrokerId(clientOrderId, CoinExExchange.ClientOrderId, 32, _baseClient.ClientOptions.AllowAppendingClientOrderId);

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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            var result = await _baseClient.SendAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
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
            clientOrderId = LibraryHelpers.ApplyBrokerId(clientOrderId, CoinExExchange.ClientOrderId, 32, _baseClient.ClientOptions.AllowAppendingClientOrderId); 

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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/stop-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            var result = await _baseClient.SendAsync<CoinExStopId>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBatchOrderResult[]>> PlaceMultipleOrdersAsync(
            IEnumerable<CoinExPlaceOrderRequest> requests,
            CancellationToken ct = default)
        {
            foreach(var order in requests)
                order.ClientOrderId = LibraryHelpers.ApplyBrokerId(order.ClientOrderId, CoinExExchange.ClientOrderId, 32, _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new ParameterCollection()
            {
                { "orders", requests.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/batch-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            var result = await _baseClient.SendAsync<CoinExBatchOrderResult[]>(request, parameters, ct, weight: requests.Count()).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBatchResult<CoinExStopId>[]>> PlaceMultipleStopOrdersAsync(
            IEnumerable<CoinExPlaceStopOrderRequest> requests,
            CancellationToken ct = default)
        {
            foreach (var order in requests)
                order.ClientOrderId = LibraryHelpers.ApplyBrokerId(order.ClientOrderId, CoinExExchange.ClientOrderId, 32, _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new ParameterCollection()
            {
                { "orders", requests.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/batch-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            var result = await _baseClient.SendAsync<CoinExBatchResult<CoinExStopId>[]>(request, parameters, ct, weight: requests.Count()).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/order-status", CoinExExchange.RateLimiter.CoinExRestSpotQuery, 1, true);
            return await _baseClient.SendAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
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

            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/pending-order", CoinExExchange.RateLimiter.CoinExRestSpotQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
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

            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/finished-order", CoinExExchange.RateLimiter.CoinExRestSpotHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            if (clientOrderId != null)
                clientOrderId = LibraryHelpers.ApplyBrokerId(clientOrderId, CoinExExchange.ClientOrderId, 32, _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/pending-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotQuery, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            if (clientOrderId != null)
                clientOrderId = LibraryHelpers.ApplyBrokerId(clientOrderId, CoinExExchange.ClientOrderId, 32, _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new ParameterCollection();
            parameters.AddEnum("market_type", accountType);
            parameters.AddOptionalEnum("side", side);
            parameters.AddOptional("market", symbol);
            parameters.AddOptional("client_id", clientOrderId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/finished-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/modify-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            return await _baseClient.SendAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/modify-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotOrder, 1, true);
            return await _baseClient.SendAsync<CoinExStopId>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/cancel-all-order", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
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

            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/cancel-order", CoinExExchange.RateLimiter.CoinExRestSpotCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBatchResult<CoinExOrder>[]>> CancelOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "order_ids", orderIds.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/cancel-batch-order", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExBatchResult<CoinExOrder>[]>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/cancel-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExStopOrder>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder[]>> CancelOrdersByClientOrderIdAsync(string symbol, AccountType accountType, string clientOrderId, CancellationToken ct = default)
        {
            clientOrderId = LibraryHelpers.ApplyBrokerId(clientOrderId, CoinExExchange.ClientOrderId, 32, _baseClient.ClientOptions.AllowAppendingClientOrderId);

            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "client_id", clientOrderId }
            };
            parameters.AddEnum("market_type", accountType);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/cancel-order-by-client-id", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExOrder[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExStopOrder[]>> CancelStopOrdersByClientOrderIdAsync(string symbol, AccountType accountType, string clientStopOrderId, CancellationToken ct = default)
        {
            clientStopOrderId = LibraryHelpers.ApplyBrokerId(clientStopOrderId, CoinExExchange.ClientOrderId, 32, _baseClient.ClientOptions.AllowAppendingClientOrderId);
            
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "client_id", clientStopOrderId }
            };
            parameters.AddEnum("market_type", accountType);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/cancel-stop-order-by-client-id", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExStopOrder[]>(request, parameters, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBatchResult<CoinExStopOrder>[]>> CancelStopOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "stop_ids", orderIds.ToArray() }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/spot/cancel-batch-stop-order", CoinExExchange.RateLimiter.CoinExRestSpotBatchCancel, 1, true);
            var result = await _baseClient.SendAsync<CoinExBatchResult<CoinExStopOrder>[]>(request, parameters, ct, weight: orderIds.Count()).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/user-deals", CoinExExchange.RateLimiter.CoinExRestSpotHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExUserTrade>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/order-deals", CoinExExchange.RateLimiter.CoinExRestSpotHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExUserTrade>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
