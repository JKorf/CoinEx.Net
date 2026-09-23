using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.FuturesApi
{
    internal partial class CoinExSocketClientFuturesSharedApi
    {

        #region Subscribe To Futures Order Updates

        async Task<WebSocketResult<UpdateSubscription>> IFuturesOrderSocketClient.SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<DataEvent<SharedFuturesOrder[]>> handler, CancellationToken ct)
            => await SubscribeToFuturesOrderUpdatesAsync(request, x => handler(x.ToType<SharedFuturesOrder[]>(x.Data)), ct).ConfigureAwait(false);

        public SubscribeFuturesOrderOptions SubscribeFuturesOrderOptions { get; } = new SubscribeFuturesOrderOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToFuturesOrderUpdatesAsync(SubscribeFuturesOrderRequest request, Action<DataEvent<SharedFuturesOrderUpdate[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeFuturesOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(_exchangeName, validationError);

            var result = await _api.SubscribeToOrderUpdatesAsync(
                update => handler(update.ToType<SharedFuturesOrderUpdate[]>(new[] {
                    new SharedFuturesOrderUpdate(
                        ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, update.Data.Order.Symbol),
                        update.Data.Order.Symbol,
                        update.Data.Order.Id.ToString(),
                        update.Data.Order.OrderType == Enums.OrderTypeV2.Limit ? SharedOrderType.Limit : update.Data.Order.OrderType == Enums.OrderTypeV2.Market ? SharedOrderType.Market : SharedOrderType.Other,
                        update.Data.Order.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        GetOrderStatus(update.Data),
                        update.Data.Order.CreateTime)
                    {
                        ClientOrderId = update.Data.Order.ClientOrderId?.ToString(),
                        OrderQuantity = new SharedOrderQuantity(update.Data.Order.Quantity, contractQuantity: update.Data.Order.Quantity),
                        QuantityFilled = new SharedOrderQuantity(update.Data.Order.QuantityFilled, update.Data.Order.ValueFilled, contractQuantity: update.Data.Order.QuantityFilled),
                        UpdateTime = update.Data.Order.UpdateTime,
                        OrderPrice = update.Data.Order.Price,
#pragma warning disable CS0618 // Type or member is obsolete
                        Fee = update.Data.Order.Fee,
                        FeeAsset = update.Data.Order.FeeAsset
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                })),
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion

        private SharedOrderStatus GetOrderStatus(CoinExFuturesOrderUpdate update)
        {
            if (update.Order.QuantityFilled == update.Order.Quantity)
                return SharedOrderStatus.Filled;

            if (update.Event != Enums.OrderUpdateType.Finish)
            {
                return SharedOrderStatus.Open;
            }
            else
            {
                if (update.Order.QuantityFilled != update.Order.Quantity)
                    return SharedOrderStatus.Canceled;

                return SharedOrderStatus.Filled;
            }
        }
    }
}
