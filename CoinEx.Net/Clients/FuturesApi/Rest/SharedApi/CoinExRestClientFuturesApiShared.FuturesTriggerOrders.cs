using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CryptoExchange.Net;
using CoinEx.Net.Objects.Models.V2;
using System.Drawing;
using CryptoExchange.Net.Objects.Errors;

namespace CoinEx.Net.Clients.FuturesApi
{
    internal partial class CoinExRestClientFuturesSharedApi
    {
        #region Place Futures Trigger Order

        async Task<ICallResult<SharedId>> IPlaceFuturesTriggerOrder.PlaceFuturesTriggerOrderAsync(PlaceFuturesTriggerOrderRequest request, CancellationToken ct)
            => await PlaceFuturesTriggerOrderAsync(request, ct).ConfigureAwait(false);

        public PlaceFuturesTriggerOrderOptions PlaceFuturesTriggerOrderOptions { get; } = new PlaceFuturesTriggerOrderOptions(_exchangeName, false)
        {
            ParameterRuleOverwrites = [
                    RequestParameterRuleOverride<PlaceFuturesTriggerOrderRequest>.NotSupported(x => x.Leverage),
                    RequestParameterRuleOverride<PlaceFuturesTriggerOrderRequest>.NotSupported(x => x.MarginMode),
                ]
        };
        public async Task<HttpResult<SharedId>> PlaceFuturesTriggerOrderAsync(PlaceFuturesTriggerOrderRequest request, CancellationToken ct)
        {
            var side = GetOrderSide(request);
            var validationError = PlaceFuturesTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var clientOrderId = request.ClientOrderId ?? ExchangeHelpers.RandomString(32);
            var result = await _api.Trading.PlaceStopOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                side,
                request.OrderPrice == null ? OrderTypeV2.Market : OrderTypeV2.Limit,
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInContracts ?? 0,
                price: request.OrderPrice,
                triggerPrice: request.TriggerPrice,
                triggerPriceType: GetTriggerPriceType(request),
                clientOrderId: clientOrderId,
                reduceOnly: request.ReduceOnly,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            // Return
            return HttpResult.Ok(result, new SharedId(clientOrderId));
        }

        #endregion

        #region Get Futures Trigger Order

        async Task<ICallResult<SharedFuturesTriggerOrder>> IGetFuturesTriggerOrder.GetFuturesTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
            => await GetFuturesTriggerOrderAsync(request, ct).ConfigureAwait(false);

        public GetFuturesTriggerOrderOptions GetFuturesTriggerOrderOptions { get; } = new GetFuturesTriggerOrderOptions(_exchangeName, true)
        {
            RequestNotes = "Only pending trigger orders can be requested, executed trigger orders are not available in the API"
        };
        public async Task<HttpResult<SharedFuturesTriggerOrder>> GetFuturesTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTriggerOrder>(Exchange, validationError);

            var status = SharedTriggerOrderStatus.Active;
            var orders = await _api.Trading.GetOpenStopOrdersAsync(clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedFuturesTriggerOrder>(orders);

            CoinExStopOrder order;
            if (orders.Data.Items.Any())
            {
                order = orders.Data.Items.Single();
            }
            else
            {
                orders = await _api.Trading.GetClosedStopOrdersAsync(request.Symbol!.GetSymbol(FormatSymbol), pageSize: 1000, ct: ct).ConfigureAwait(false);
                if (!orders.Success)
                return HttpResult.Fail<SharedFuturesTriggerOrder>(orders);

                order = orders.Data.Items.SingleOrDefault(x => x.ClientOrderId == request.OrderId)!;
                if (order == null)
                    return HttpResult.Fail<SharedFuturesTriggerOrder>(orders, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                status = SharedTriggerOrderStatus.Filled;
            }

            return HttpResult.Ok(orders, new SharedFuturesTriggerOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, order.Symbol),
                order.Symbol,
                order.ClientOrderId?.ToString() ?? order.StopOrderId.ToString(),
                ParseOrderType(order.Type),
                order.Side == OrderSide.Buy ? SharedTriggerOrderDirection.Enter : SharedTriggerOrderDirection.Exit,
                status,
                order.TriggerPrice,
                null,
                order.CreateTime)
            {
                OrderPrice = order.Price == 0 ? null : order.Price,
                UpdateTime = order.UpdateTime,
                OrderQuantity = new SharedOrderQuantity(order.Quantity, contractQuantity: order.Quantity),
                ClientOrderId = order.ClientOrderId
            });
        }

        #endregion

        #region Cancel Futures Trigger Order

        async Task<ICallResult<SharedId>> ICancelFuturesTriggerOrder.CancelFuturesTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
            => await CancelFuturesTriggerOrderAsync(request, ct).ConfigureAwait(false);

        public CancelFuturesTriggerOrderOptions CancelFuturesTriggerOrderOptions { get; } = new CancelFuturesTriggerOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelFuturesTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var order = await _api.Trading.CancelStopOrderByClientOrderIdAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                request.OrderId,
                ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));
        }

        #endregion

        private TriggerPriceType GetTriggerPriceType(PlaceFuturesTriggerOrderRequest request)
        {
            if (request.TriggerPriceType == null || request.TriggerPriceType == SharedTriggerPriceType.LastPrice)
                return TriggerPriceType.LastPrice;

            if (request.TriggerPriceType == SharedTriggerPriceType.IndexPrice)
                return TriggerPriceType.IndexPrice;

            return TriggerPriceType.MarkPrice;
        }

        private OrderSide GetOrderSide(PlaceFuturesTriggerOrderRequest request)
        {
            if (request.PositionSide == SharedPositionSide.Long)
                return request.OrderDirection == SharedTriggerOrderDirection.Enter ? OrderSide.Buy : OrderSide.Sell;

            return request.OrderDirection == SharedTriggerOrderDirection.Enter ? OrderSide.Sell : OrderSide.Buy;
        }
    }
}
