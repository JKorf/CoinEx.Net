using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models;
using CryptoExchange.Net.CommonObjects;
using CoinEx.Net.Interfaces.Clients.SpotApiV1;

namespace CoinEx.Net.Clients.SpotApiV1
{
    /// <inheritdoc />
    internal class CoinExRestClientSpotApiTrading : ICoinExRestClientSpotApiTrading
    {
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

        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiTrading(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal quantity,

            decimal? price = null,
            decimal? stopPrice = null,
            bool? immediateOrCancel = null,
            OrderOption? orderOption = null,
            string? clientOrderId = null,
            string? sourceId = null,
            CancellationToken ct = default)
        {
            var endpoint = "";
            if (type == OrderType.Limit)
                endpoint = PlaceLimitOrderEndpoint;
            else if (type == OrderType.Market)
                endpoint = PlaceMarketOrderEndpoint;
            else if (type == OrderType.StopLimit)
                endpoint = PlaceStopLimitOrderEndpoint;
            else if (type == OrderType.StopMarket)
                endpoint = PlaceStopMarketOrderEndpoint;

            if (immediateOrCancel == true)
            {
                if (type != OrderType.Limit)
                    throw new ArgumentException("ImmediateOrCancel only valid for limit orders");

                endpoint = PlaceImmediateOrCancelOrderEndpoint;
            }

            clientOrderId ??= ExchangeHelpers.AppendRandomString("x-" + _baseClient._brokerId + "-", 32);

            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("price", price?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("option", orderOption.HasValue ? JsonConvert.SerializeObject(orderOption, new OrderOptionConverter(false)) : null);
            parameters.AddOptionalParameter("stop_price", stopPrice?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("client_id", clientOrderId);
            parameters.AddOptionalParameter("source_id", sourceId);

            var result = await _baseClient.Execute<CoinExOrder>(_baseClient.GetUrl(endpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderPlaced(new OrderId { SourceObject = result.Data, Id = result.Data.Id.ToString(CultureInfo.InvariantCulture) });

            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string? symbol = null, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "page", page ?? 1 },
                { "limit", limit ?? 100 }
            };

            parameters.AddOptionalParameter("market", symbol);

            return await _baseClient.ExecutePaged<CoinExOrder>(_baseClient.GetUrl(OpenOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenStopOrdersAsync(string symbol, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "page", page ?? 1 },
                { "limit", limit ?? 100 }
            };
            parameters.AddOptionalParameter("market", symbol);
            return await _baseClient.ExecutePaged<CoinExOrder>(_baseClient.GetUrl(OpenStopOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetClosedOrdersAsync(string symbol, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page ?? 1 },
                { "limit", limit ?? 100 }
            };

            return await _baseClient.ExecutePaged<CoinExOrder>(_baseClient.GetUrl(FinishedOrdersEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "id", orderId }
            };

            return await _baseClient.Execute<CoinExOrder>(_baseClient.GetUrl(OrderStatusEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrderTrade>>> GetOrderTradesAsync(long orderId, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "id", orderId },
                { "page", page ?? 1 },
                { "limit", limit ?? 100  }
            };

            return await _baseClient.ExecutePaged<CoinExOrderTrade>(_baseClient.GetUrl(OrderDetailsEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExOrderTradeExtended>>> GetUserTradesAsync(string symbol, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "page", page ?? 1 },
                { "limit", limit ?? 100 }
            };

            return await _baseClient.ExecutePaged<CoinExOrderTradeExtended>(_baseClient.GetUrl(UserTransactionsEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "id", orderId }
            };

            var result = await _baseClient.Execute<CoinExOrder>(_baseClient.GetUrl(CancelOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderCanceled(new OrderId { SourceObject = result.Data, Id = result.Data.Id.ToString(CultureInfo.InvariantCulture) });
            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult> CancelAllOrdersAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
            };

            return await _baseClient.Execute(_baseClient.GetUrl(CancelOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> CancelAllStopOrdersAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
            };

            return await _baseClient.Execute(_baseClient.GetUrl(CancelStopOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        }

    }
}
