﻿using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis.Enums;
using CryptoExchange.Net.SharedApis.Interfaces.Socket;
using CryptoExchange.Net.SharedApis.Models.Socket;
using CryptoExchange.Net.SharedApis.RequestModels;
using CryptoExchange.Net.SharedApis.ResponseModels;
using CryptoExchange.Net.SharedApis.SubscribeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.SpotApiV2
{
    internal partial class CoinExSocketClientSpotApi : ICoinExSocketClientSpotApiShared
    {
        public string Exchange => CoinExExchange.ExchangeName;

        async Task<CallResult<UpdateSubscription>> ITickerSocketClient.SubscribeToTickerUpdatesAsync(TickerSubscribeRequest request, Action<DataEvent<SharedTicker>> handler, CancellationToken ct)
        {
            var symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, request.ApiType);
            var result = await SubscribeToTickerUpdatesAsync(new[] { symbol }, update =>
            {
                var ticker = update.Data.Single();
                handler(update.As(new SharedTicker(symbol, ticker.LastPrice, ticker.HighPrice, ticker.LowPrice)));
            }, ct).ConfigureAwait(false);

            return result;
        }

        async Task<CallResult<UpdateSubscription>> ITradeSocketClient.SubscribeToTradeUpdatesAsync(TradeSubscribeRequest request, Action<DataEvent<IEnumerable<SharedTrade>>> handler, CancellationToken ct)
        {
            var symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, request.ApiType);
            var result = await SubscribeToTradeUpdatesAsync(symbol, update => handler(update.As(update.Data.Select(x => new SharedTrade(x.Quantity, x.Price, x.Timestamp)))), ct).ConfigureAwait(false);

            return result;
        }

        async Task<CallResult<UpdateSubscription>> IBookTickerSocketClient.SubscribeToBookTickerUpdatesAsync(BookTickerSubscribeRequest request, Action<DataEvent<SharedBookTicker>> handler, CancellationToken ct)
        {
            var symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, request.ApiType);
            var result = await SubscribeToBookPriceUpdatesAsync(symbol, update => handler(update.As(new SharedBookTicker(update.Data.BestAskPrice, update.Data.BestAskQuantity, update.Data.BestBidPrice, update.Data.BestBidQuantity))), ct).ConfigureAwait(false);

            return result;
        }

        async Task<CallResult<UpdateSubscription>> IBalanceSocketClient.SubscribeToBalanceUpdatesAsync(SharedRequest request, Action<DataEvent<IEnumerable<SharedBalance>>> handler, CancellationToken ct)
        {
            var result = await SubscribeToBalanceUpdatesAsync(
                update => handler(update.As(update.Data.Select(x => new SharedBalance(x.Asset, x.Available, x.Available + x.Frozen)))),
                ct: ct).ConfigureAwait(false);

            return result;
        }

        async Task<CallResult<UpdateSubscription>> ISpotOrderSocketClient.SubscribeToOrderUpdatesAsync(SharedRequest request, Action<DataEvent<IEnumerable<SharedSpotOrder>>> handler, CancellationToken ct)
        {
            var result = await SubscribeToOrderUpdatesAsync(
                update => handler(update.As<IEnumerable<SharedSpotOrder>>(new[] {
                    new SharedSpotOrder(
                        update.Data.Order.Symbol,
                        update.Data.Order.Id.ToString(),
                        update.Data.Order.OrderType == Enums.OrderTypeV2.Limit ? CryptoExchange.Net.SharedApis.Enums.SharedOrderType.Limit : update.Data.Order.OrderType == Enums.OrderTypeV2.Market ? CryptoExchange.Net.SharedApis.Enums.SharedOrderType.Market : CryptoExchange.Net.SharedApis.Enums.SharedOrderType.Other,
                        update.Data.Order.Side == Enums.OrderSide.Buy ? CryptoExchange.Net.SharedApis.Enums.SharedOrderSide.Buy : CryptoExchange.Net.SharedApis.Enums.SharedOrderSide.Sell,
                        GetOrderStatus(update.Data),
                        update.Data.Order.CreateTime)
                    {
                        ClientOrderId = update.Data.Order.ClientOrderId?.ToString(),
                        Quantity = update.Data.Order.Quantity,
                        QuantityFilled = update.Data.Order.QuantityFilled,
                        QuoteQuantityFilled = update.Data.Order.ValueFilled,
                        UpdateTime = update.Data.Order.UpdateTime,
                        Price = update.Data.Order.Price
                    }
                })),
                ct: ct).ConfigureAwait(false);

            return result;
        }

        private SharedOrderStatus GetOrderStatus(CoinExOrderUpdate update)
        {
            if (update.Order.QuantityFilled == update.Order.Quantity)
                return SharedOrderStatus.Filled;

            if (update.Event != Enums.OrderUpdateType.Finish) 
            {
                if (update.Order.QuantityFilled != 0)
                    return SharedOrderStatus.PartiallyFilled;
                else
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
