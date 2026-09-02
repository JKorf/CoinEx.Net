using CoinEx.Net.Clients.FuturesApi;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
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

namespace CoinEx.Net.Clients.SpotApiV2
{
    internal partial class CoinExSocketClientSpotSharedApi
    {
        #region Spot Order client

        async Task<WebSocketResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrder[]>> handler, CancellationToken ct)
            => await SubscribeToSpotOrderUpdatesAsync(request, x => handler(x.ToType<SharedSpotOrder[]>(x.Data)), ct).ConfigureAwait(false);

        public SubscribeSpotOrderOptions SubscribeSpotOrderOptions { get; } = new SubscribeSpotOrderOptions(_exchangeName, false);
        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToSpotOrderUpdatesAsync(SubscribeSpotOrderRequest request, Action<DataEvent<SharedSpotOrderUpdate[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(_exchangeName, validationError);
            var result = await _api.SubscribeToOrderUpdatesAsync(
                update => handler(update.ToType(new[] {
                    new SharedSpotOrderUpdate(
                        ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, update.Data.Order.Symbol),
                        update.Data.Order.Symbol,
                        update.Data.Order.Id.ToString(),
                        update.Data.Order.OrderType == Enums.OrderTypeV2.Limit ? SharedOrderType.Limit : update.Data.Order.OrderType == Enums.OrderTypeV2.Market ? SharedOrderType.Market : SharedOrderType.Other,
                        update.Data.Order.Side == Enums.OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        GetOrderStatus(update.Data),
                        update.Data.Order.CreateTime)
                    {
                        ClientOrderId = update.Data.Order.ClientOrderId?.ToString(),
                        OrderQuantity = update.Data.Order.OrderType == OrderTypeV2.Market ? null : new SharedOrderQuantity(update.Data.Order.Quantity), // For market orders there is no way to know if quantity is in base or quote asset..
                        QuantityFilled = new SharedOrderQuantity(update.Data.Order.QuantityFilled, update.Data.Order.ValueFilled),
                        UpdateTime = update.Data.Order.UpdateTime,
                        OrderPrice = update.Data.Order.Price == 0 ? null : update.Data.Order.Price,
#pragma warning disable CS0618 // Type or member is obsolete
                        Fee = update.Data.Order.FeeBaseAsset + update.Data.Order.FeeQuoteAsset
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                })),
                ct: ct).ConfigureAwait(false);

            return result;
        }
        #endregion

        private SharedOrderStatus GetOrderStatus(CoinExOrderUpdate update)
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
