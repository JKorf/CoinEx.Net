using CoinEx.Net.Clients.FuturesApi;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.SpotApiV2
{
    internal partial class CoinExRestClientSpotSharedApi
    {
        #region Spot Order client

        public PlaceSpotOrderOptions PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(_exchangeName);

        public SharedFeeDeductionType SpotFeeDeductionType => SharedFeeDeductionType.DeductFromOutput;
        public SharedFeeAssetType SpotFeeAssetType => SharedFeeAssetType.OutputAsset;
        public SharedOrderType[] SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        public SharedTimeInForce[] SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };

        public SharedQuantitySupport SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAndQuoteAsset,
                SharedQuantityType.BaseAndQuoteAsset);

        public string GenerateClientOrderId() => ExchangeHelpers.RandomString(20);

        async Task<ICallResult<SharedId>> IPlaceSpotOrder.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
            => await PlaceSpotOrderAsync(request, ct).ConfigureAwait(false);

        public async Task<HttpResult<SharedId>> PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var result = await _api.Trading.PlaceOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                AccountType.Spot,
                request.Side == SharedOrderSide.Buy ? OrderSide.Buy : OrderSide.Sell,
                GetOrderType(request.OrderType, request.TimeInForce),
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInQuoteAsset ?? 0,
                price: request.Price,
                clientOrderId: request.ClientOrderId,
                quantityAsset: request.OrderType == SharedOrderType.Market ? (request.Quantity?.QuantityInBaseAsset != null ? request.Symbol!.BaseAsset : request.Symbol!.QuoteAsset) : null,
                ct: ct
                ).ConfigureAwait(false);

            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            return HttpResult.Ok(result, new SharedId(result.Data.Id.ToString()));
        }

        public GetSpotOrderOptions GetSpotOrderOptions { get; } = new GetSpotOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedSpotOrder>> GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await _api.Trading.GetOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotOrder>(orders);

            return HttpResult.Ok(orders, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, orders.Data.Symbol),
                orders.Data.Symbol,
                orders.Data.Id.ToString(),
                ParseOrderType(orders.Data.OrderType),
                orders.Data.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(orders.Data.Status),
                orders.Data.CreateTime)
            {
                ClientOrderId = orders.Data.ClientOrderId,
                OrderPrice = orders.Data.Price,
                UpdateTime = orders.Data.UpdateTime,
                OrderQuantity = new SharedOrderQuantity(orders.Data.QuantityAsset == null || !orders.Data.Symbol!.EndsWith(orders.Data.QuantityAsset) ? orders.Data.Quantity : null, orders.Data.Symbol!.EndsWith(orders.Data.QuantityAsset!) ? orders.Data.Quantity : null),
                QuantityFilled = new SharedOrderQuantity(orders.Data.QuantityFilled, orders.Data.ValueFilled),
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = orders.Data.FeeBaseAsset > 0 ? orders.Data.FeeBaseAsset : orders.Data.FeeQuoteAsset,
                FeeAsset = orders.Data.FeeBaseAsset > 0 ? request.Symbol!.BaseAsset : orders.Data.FeeQuoteAsset > 0 ? request.Symbol!.QuoteAsset : null,
#pragma warning restore CS0618 // Type or member is obsolete
                TimeInForce = ParseTimeInForce(orders.Data.OrderType)
            });
        }

        public GetOpenSpotOrdersOptions GetOpenSpotOrdersOptions { get; } = new GetOpenSpotOrdersOptions(_exchangeName, true);
        public async Task<HttpResult<SharedSpotOrder[]>> GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = GetOpenSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            string? symbol = request.Symbol?.GetSymbol(FormatSymbol);

            var orders = await _api.Trading.GetOpenOrdersAsync(AccountType.Spot, symbol, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(orders);

            return HttpResult.Ok(orders, orders.Data.Items.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, x.Symbol), 
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.OrderType),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                SharedOrderStatus.Open,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId,
                OrderPrice = x.Price,
                UpdateTime = x.UpdateTime,
                OrderQuantity = new SharedOrderQuantity(x.QuantityAsset == null || !x.Symbol!.EndsWith(x.QuantityAsset) ? x.Quantity : null, x.Symbol!.EndsWith(x.QuantityAsset!) ? x.Quantity : null),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.ValueFilled),
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = x.FeeBaseAsset > 0 ? x.FeeBaseAsset : x.FeeQuoteAsset,
                FeeAsset = x.FeeBaseAsset > 0 ? request.Symbol?.BaseAsset : x.FeeQuoteAsset > 0 ? request.Symbol?.QuoteAsset : null,
#pragma warning restore CS0618 // Type or member is obsolete
                TimeInForce = ParseTimeInForce(x.OrderType)
            }).ToArray());
        }

        public GetSpotClosedOrdersOptions GetClosedSpotOrdersOptions { get; } = new GetSpotClosedOrdersOptions(_exchangeName, false, true, false, 500);
        public async Task<HttpResult<SharedSpotOrder[]>> GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetClosedSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            // Determine page token
            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 500;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Trading.GetClosedOrdersAsync(
                AccountType.Spot,
                request.Symbol!.GetSymbol(FormatSymbol),
                page: pageParams.Page,
                pageSize: limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromPage(pageParams),
                    result.Data.Items.Length,
                    result.Data.Items.Select(x => x.CreateTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Items, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                    .Select(x => new SharedSpotOrder(
                        ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, x.Symbol), 
                        x.Symbol,
                        x.Id.ToString(),
                        ParseOrderType(x.OrderType),
                        x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        SharedOrderStatus.Filled, // Canceled orders are not returned
                        x.CreateTime)
                        {
                            ClientOrderId = x.ClientOrderId,
                            OrderPrice = x.Price,
                            OrderQuantity = new SharedOrderQuantity(x.QuantityAsset == null || !x.Symbol!.EndsWith(x.QuantityAsset) ? x.Quantity : null, x.Symbol!.EndsWith(x.QuantityAsset!) ? x.Quantity : null),
                            QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.ValueFilled),
                            UpdateTime = x.UpdateTime,
#pragma warning disable CS0618 // Type or member is obsolete
                            Fee = x.FeeBaseAsset > 0 ? x.FeeBaseAsset : x.FeeQuoteAsset,
                            FeeAsset = x.FeeBaseAsset > 0 ? request.Symbol?.BaseAsset : x.FeeQuoteAsset > 0 ? request.Symbol?.QuoteAsset : null,
#pragma warning restore CS0618 // Type or member is obsolete
                        TimeInForce = ParseTimeInForce(x.OrderType)
                        })
                    .ToArray(), nextPageRequest);
        }

        public GetSpotOrderTradesOptions GetSpotOrderTradesOptions { get; } = new GetSpotOrderTradesOptions(_exchangeName, true);
        public async Task<HttpResult<SharedUserTrade[]>> GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await _api.Trading.GetOrderTradesAsync(request.Symbol!.GetSymbol(FormatSymbol), AccountType.Spot, orderId: orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedUserTrade[]>(orders);

            return HttpResult.Ok(orders, orders.Data.Items.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                new SharedOrderQuantity(x.Quantity),
                x.Price,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId,
                Role = x.Role == TransactionRole.Maker ? SharedRole.Maker : SharedRole.Taker,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
            }).ToArray());
        }

        Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetSpotUserTradeHistoryAsync(request, pageRequest, ct);
        GetSpotUserTradeHistoryOptions ISpotOrderRestClient.GetSpotUserTradesOptions => GetSpotUserTradeHistoryOptions;

        public GetSpotUserTradeHistoryOptions GetSpotUserTradeHistoryOptions { get; } = new GetSpotUserTradeHistoryOptions(_exchangeName, false, true, true, 500);
        public async Task<HttpResult<SharedUserTrade[]>> GetSpotUserTradeHistoryAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetSpotUserTradeHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            // Determine page token
            int limit = request.Limit ?? 500;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Trading.GetUserTradesAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                AccountType.Spot,
                startTime: request.StartTime,
                endTime: request.EndTime,
                page: pageParams.Page,
                pageSize: limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedUserTrade[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromPage(pageParams),
                    result.Data.Items.Length,
                    result.Data.Items.Select(x => x.CreateTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Items, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                    .Select(x => new SharedUserTrade(
                        ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, x.Symbol), 
                        x.Symbol,
                        x.OrderId.ToString(),
                        x.Id.ToString(),
                        x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        new SharedOrderQuantity(x.Quantity),
                        x.Price,
                        x.CreateTime)
                    {
                        ClientOrderId = x.ClientOrderId,
                        Role = x.Role == TransactionRole.Maker ? SharedRole.Maker : SharedRole.Taker,
                        Fee = x.Fee,
                        FeeAsset = x.FeeAsset,
                    })
                    .ToArray(), nextPageRequest);
        }
                
        public CancelSpotOrderOptions CancelSpotOrderOptions { get; } = new CancelSpotOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await _api.Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), AccountType.Spot, orderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.Id.ToString()));
        }

        private SharedOrderStatus ParseOrderStatus(OrderStatusV2? status)
        {
            if (status == null || status == OrderStatusV2.Open || status == OrderStatusV2.PartiallyFilled) return SharedOrderStatus.Open;
            if (status == OrderStatusV2.Canceled || status == OrderStatusV2.PartiallyCanceled) return SharedOrderStatus.Canceled;
            return SharedOrderStatus.Filled;
        }

        private SharedOrderType ParseOrderType(OrderTypeV2 type)
        {
            if (type == OrderTypeV2.Market) return SharedOrderType.Market;
            if (type == OrderTypeV2.PostOnly) return SharedOrderType.LimitMaker;

            return SharedOrderType.Limit;
        }

        private SharedTimeInForce? ParseTimeInForce(OrderTypeV2 type)
        {
            if (type == OrderTypeV2.ImmediateOrCancel) return SharedTimeInForce.ImmediateOrCancel;
            if (type == OrderTypeV2.FillOrKill) return SharedTimeInForce.FillOrKill;

            return null;
        }

        private OrderTypeV2 GetOrderType(SharedOrderType type, SharedTimeInForce? tif)
        {
            if (type == SharedOrderType.Market) return OrderTypeV2.Market;
            if (type == SharedOrderType.LimitMaker) return OrderTypeV2.PostOnly;
            if (tif == SharedTimeInForce.ImmediateOrCancel) return OrderTypeV2.ImmediateOrCancel;
            if (tif == SharedTimeInForce.FillOrKill ) return OrderTypeV2.FillOrKill;
            return OrderTypeV2.Limit;
        }

        #endregion

        #region Spot Client Id Order Client

        public GetSpotOrderByClientOrderIdOptions GetSpotOrderByClientOrderIdOptions { get; } = new GetSpotOrderByClientOrderIdOptions(_exchangeName, true);
        public async Task<HttpResult<SharedSpotOrder>> GetSpotOrderByClientOrderIdAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            var order = await _api.Trading.GetOpenOrdersAsync(AccountType.Spot, clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedSpotOrder>(order);

            var orderData = order.Data.Items.FirstOrDefault();
            if (orderData == null)
            {
                order = await _api.Trading.GetClosedOrdersAsync(AccountType.Spot, clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
                if (!order.Success)
                return HttpResult.Fail<SharedSpotOrder>(order);

                orderData = order.Data.Items.FirstOrDefault();                
            }

            if (orderData == null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

            return HttpResult.Ok(order, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, orderData.Symbol),
                orderData.Symbol,
                orderData.Id.ToString(),
                ParseOrderType(orderData.OrderType),
                orderData.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(orderData.Status),
                orderData.CreateTime)
            {
                ClientOrderId = orderData.ClientOrderId,
                OrderPrice = orderData.Price,
                OrderQuantity = new SharedOrderQuantity(orderData.QuantityAsset == null || !orderData.Symbol!.EndsWith(orderData.QuantityAsset) ? orderData.Quantity : null, orderData.Symbol!.EndsWith(orderData.QuantityAsset!) ? orderData.Quantity : null),
                QuantityFilled = new SharedOrderQuantity(orderData.QuantityFilled, orderData.ValueFilled),
                UpdateTime = orderData.UpdateTime,
#pragma warning disable CS0618 // Type or member is obsolete
                Fee = orderData.FeeBaseAsset > 0 ? orderData.FeeBaseAsset : orderData.FeeQuoteAsset,
                FeeAsset = orderData.FeeBaseAsset > 0 ? request.Symbol!.BaseAsset : orderData.FeeQuoteAsset > 0 ? request.Symbol!.QuoteAsset : null,
#pragma warning restore CS0618 // Type or member is obsolete
                TimeInForce = ParseTimeInForce(orderData.OrderType)
            });
        }

        public CancelSpotOrderByClientOrderIdOptions CancelSpotOrderByClientOrderIdOptions { get; } = new CancelSpotOrderByClientOrderIdOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelSpotOrderByClientOrderIdAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotOrderByClientOrderIdOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var order = await _api.Trading.CancelOrdersByClientOrderIdAsync(request.Symbol!.GetSymbol(FormatSymbol), AccountType.Spot, clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.FirstOrDefault()?.Id.ToString() ?? request.OrderId));
        }
        #endregion
    }
}
