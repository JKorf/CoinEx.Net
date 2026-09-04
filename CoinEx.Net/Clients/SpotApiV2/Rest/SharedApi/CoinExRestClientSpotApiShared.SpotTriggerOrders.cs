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
        #region Place Spot Trigger Order

        async Task<ICallResult<SharedId>> IPlaceSpotTriggerOrder.PlaceSpotTriggerOrderAsync(PlaceSpotTriggerOrderRequest request, CancellationToken ct)
            => await PlaceSpotTriggerOrderAsync(request, ct).ConfigureAwait(false);

        public PlaceSpotTriggerOrderOptions PlaceSpotTriggerOrderOptions { get; } = new PlaceSpotTriggerOrderOptions(_exchangeName, false);

        public async Task<HttpResult<SharedId>> PlaceSpotTriggerOrderAsync(PlaceSpotTriggerOrderRequest request, CancellationToken ct)
        {
            var validationError = PlaceSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var clientOrderId = request.ClientOrderId ?? ExchangeHelpers.RandomString(32);
            var result = await _api.Trading.PlaceStopOrderAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                AccountType.Spot,
                request.OrderSide == SharedOrderSide.Buy ? OrderSide.Buy : OrderSide.Sell,
                request.OrderPrice == null ? OrderTypeV2.Market : OrderTypeV2.Limit,
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInQuoteAsset ?? 0,
                price: request.OrderPrice,
                triggerPrice: request.TriggerPrice,
                quantityAsset: request.OrderPrice == null ? (request.Quantity != null ? request.Symbol!.BaseAsset : request.Symbol!.QuoteAsset) : null,
                clientOrderId: clientOrderId,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            // Return
            return HttpResult.Ok(result, new SharedId(clientOrderId));
        }

        #endregion

        #region Get Spot Trigger Order

        async Task<ICallResult<SharedSpotTriggerOrder>> IGetSpotTriggerOrder.GetSpotTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
            => await GetSpotTriggerOrderAsync(request, ct).ConfigureAwait(false);

        public GetSpotTriggerOrderOptions GetSpotTriggerOrderOptions { get; } = new GetSpotTriggerOrderOptions(_exchangeName, true)
        {
            RequestNotes = "Only pending trigger orders can be requested, executed trigger orders are not available in the API"
        };
        public async Task<HttpResult<SharedSpotTriggerOrder>> GetSpotTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = GetSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTriggerOrder>(Exchange, validationError);

            var status = SharedTriggerOrderStatus.Active;
            var orders = await _api.Trading.GetOpenStopOrdersAsync(AccountType.Spot, clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(orders);

            CoinExStopOrder order;
            if (orders.Data.Items.Any())
            {
                order = orders.Data.Items.Single();
            }
            else
            {
                orders = await _api.Trading.GetClosedStopOrdersAsync(AccountType.Spot, request.Symbol!.GetSymbol(FormatSymbol), pageSize: 1000, ct: ct).ConfigureAwait(false);
                if (!orders.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(orders);

                order = orders.Data.Items.SingleOrDefault(x => x.ClientOrderId == request.OrderId)!;
                if (order == null)
                    return HttpResult.Fail<SharedSpotTriggerOrder>(orders, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                status = SharedTriggerOrderStatus.Filled;
            }

            return HttpResult.Ok(orders, new SharedSpotTriggerOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, order.Symbol),
                order.Symbol,
                order.ClientOrderId?.ToString() ?? order.StopOrderId.ToString(),
                ParseOrderType(order.Type),
                order.Side == OrderSide.Buy ? SharedTriggerOrderDirection.Enter: SharedTriggerOrderDirection.Exit,
                status,
                order.TriggerPrice,
                order.CreateTime)
            {
                OrderPrice = order.Price,
                UpdateTime = order.UpdateTime,
                OrderQuantity = new SharedOrderQuantity(order.QuantityAsset == null || !order.Symbol!.EndsWith(order.QuantityAsset) ? order.Quantity : null, order.Symbol!.EndsWith(order.QuantityAsset!) ? order.Quantity : null),
                ClientOrderId = order.ClientOrderId
            });
        }

        #endregion

        #region Cancel Spot Trigger Order

        async Task<ICallResult<SharedId>> ICancelSpotTriggerOrder.CancelSpotTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
            => await CancelSpotTriggerOrderAsync(request, ct).ConfigureAwait(false);

        public CancelSpotTriggerOrderOptions CancelSpotTriggerOrderOptions { get; } = new CancelSpotTriggerOrderOptions(_exchangeName, true);
        public async Task<HttpResult<SharedId>> CancelSpotTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = CancelSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var order = await _api.Trading.CancelStopOrdersByClientOrderIdAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                AccountType.Spot,
                request.OrderId,
                ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));
        }

        #endregion

    }
}
