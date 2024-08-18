using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis.Enums;
using CryptoExchange.Net.SharedApis.Interfaces;
using CryptoExchange.Net.SharedApis.Models.Rest;
using CryptoExchange.Net.SharedApis.RequestModels;
using CryptoExchange.Net.SharedApis.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Enums;
using CryptoExchange.Net.SharedApis.Models;

namespace CoinEx.Net.Clients.SpotApiV2
{
    internal partial class CoinExRestClientSpotApi : ICoinExRestClientSpotApiShared
    {
        public string Exchange => CoinExExchange.ExchangeName;

        public IEnumerable<SharedOrderType> SupportedOrderType { get; } = new[]
        {
            SharedOrderType.Limit,
            SharedOrderType.Market,
            SharedOrderType.LimitMaker
        };

        public IEnumerable<SharedTimeInForce> SupportedTimeInForce { get; } = new[]
        {
            SharedTimeInForce.GoodTillCanceled,
            SharedTimeInForce.ImmediateOrCancel,
            SharedTimeInForce.FillOrKill
        };

        public SharedQuantitySupport OrderQuantitySupport { get; } =
            new SharedQuantitySupport(
                SharedQuantityType.BaseAssetQuantity,
                SharedQuantityType.BaseAssetQuantity,
                SharedQuantityType.Both,
                SharedQuantityType.Both);

        async Task<ExchangeWebResult<IEnumerable<SharedKline>>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeWebResult<IEnumerable<SharedKline>>(Exchange, new ArgumentError("Interval not supported"));

            // Determine the amount of data points we need to match the requested time
            var apiLimit = 1000;
            int limit = request.Filter?.Limit ?? apiLimit;
            if (request.Filter?.StartTime.HasValue == true)
                limit = (int)Math.Ceiling((DateTime.UtcNow - request.Filter.StartTime!.Value).TotalSeconds / (int)request.Interval);

            if (limit > apiLimit)
            {
                // Not available via the API
                var cutoff = DateTime.UtcNow.AddSeconds(-(int)request.Interval * apiLimit);
                return new ExchangeWebResult<IEnumerable<SharedKline>>(Exchange, new ArgumentError($"Time filter outside of supported range. Can only request the most recent {apiLimit} klines i.e. data later than {cutoff} at this interval"));
            }

            // Pagination not supported, no time filter available

            // Get data
            var result = await ExchangeData.GetKlinesAsync(
                request.GetSymbol(FormatSymbol),
                interval,
                limit: limit,
                ct: ct
                ).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedKline>>(Exchange, default);

            // Filter the data based on requested timestamps
            var data = result.Data;
            if (request.Filter?.StartTime.HasValue == true)
                data = data.Where(d => d.OpenTime >= request.Filter.StartTime.Value);
            if (request.Filter?.EndTime.HasValue == true)
                data = data.Where(d => d.OpenTime < request.Filter.EndTime.Value);
            if (request.Filter?.Limit.HasValue == true)
                data = data.Take(request.Filter.Limit.Value);

            return result.AsExchangeResult(Exchange, data.Select(x => new SharedKline(x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice, x.Volume)));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedSpotSymbol>>> ISpotSymbolRestClient.GetSpotSymbolsAsync(SharedRequest request, CancellationToken ct)
        {
            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedSpotSymbol>>(Exchange, default);

            return result.AsExchangeResult(Exchange, result.Data.Select(s => new SharedSpotSymbol(s.BaseAsset, s.QuoteAsset, s.Name)
            {
                MinTradeQuantity = s.MinOrderQuantity,
                PriceDecimals = s.PricePrecision,
                QuantityDecimals = s.QuantityPrecision
            }));
        }

        async Task<ExchangeWebResult<SharedTicker>> ITickerRestClient.GetTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var result = await ExchangeData.GetTickersAsync(new[] { request.GetSymbol(FormatSymbol) }, ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedTicker>(Exchange, default);

            var ticker = result.Data.Single();
            return result.AsExchangeResult(Exchange, new SharedTicker(ticker.Symbol, ticker.LastPrice, ticker.HighPrice, ticker.LowPrice));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedTicker>>> ITickerRestClient.GetTickersAsync(SharedRequest request, CancellationToken ct)
        {
            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTicker>>(Exchange, default);

            return result.AsExchangeResult<IEnumerable<SharedTicker>>(Exchange, result.Data.Select(x => new SharedTicker(x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice)));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedTrade>>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var result = await ExchangeData.GetTradeHistoryAsync(
                request.GetSymbol(FormatSymbol),
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, default);

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedTrade(x.Quantity, x.Price, x.Timestamp)));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedBalance>>> IBalanceRestClient.GetBalancesAsync(SharedRequest request, CancellationToken ct)
        {
            var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedBalance>>(Exchange, default);

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedBalance(x.Asset, x.Available, x.Available + x.Frozen)));
        }

        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.PlaceOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            if (request.OrderType == SharedOrderType.Other)
                throw new ArgumentException("OrderType can't be `Other`", nameof(request.OrderType));

            var result = await Trading.PlaceOrderAsync(
                request.GetSymbol(FormatSymbol),
                AccountType.Spot,
                request.Side == SharedOrderSide.Buy ? OrderSide.Buy : OrderSide.Sell,
                GetOrderType(request.OrderType, request.TimeInForce),
                quantity: request.Quantity ?? request.QuoteQuantity ?? 0,
                price: request.Price,
                clientOrderId: request.ClientOrderId,
                quantityAsset: request.OrderType == SharedOrderType.Market ? (request.Quantity != null ? request.BaseAsset : request.QuoteAsset) : null
                ).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, default);

            return result.AsExchangeResult(Exchange, new SharedId(result.Data.Id.ToString()));
        }

        async Task<ExchangeWebResult<SharedSpotOrder>> ISpotOrderRestClient.GetOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, new ArgumentError("Invalid order id"));

            var orders = await Trading.GetOrderAsync(request.GetSymbol(FormatSymbol), orderId).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedSpotOrder>(Exchange, default);

            return orders.AsExchangeResult(Exchange, new SharedSpotOrder(
                orders.Data.Symbol,
                orders.Data.Id.ToString(),
                ParseOrderType(orders.Data.OrderType),
                orders.Data.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(orders.Data.Status),
                orders.Data.CreateTime)
            {
                ClientOrderId = orders.Data.ClientOrderId,
                Price = orders.Data.Price,
                Quantity = orders.Data.QuantityAsset == null || orders.Data.QuantityAsset == request.BaseAsset ? orders.Data.Quantity : null,
                QuantityFilled = orders.Data.QuantityFilled,
                UpdateTime = orders.Data.UpdateTime,
                QuoteQuantity = orders.Data.QuantityAsset == request.QuoteAsset ? orders.Data.Quantity : null,
                QuoteQuantityFilled = orders.Data.ValueFilled,
                Fee = orders.Data.FeeBaseAsset > 0 ? orders.Data.FeeBaseAsset : orders.Data.FeeQuoteAsset,
                FeeAsset = orders.Data.FeeBaseAsset > 0 ? request.BaseAsset : orders.Data.FeeQuoteAsset > 0 ? request.QuoteAsset : null,
                TimeInForce = ParseTimeInForce(orders.Data.OrderType)
            });
        }

        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetOpenOrdersAsync(GetSpotOpenOrdersRequest request, CancellationToken ct)
        {
            string? symbol = null;
            if (request.BaseAsset != null && request.QuoteAsset != null)
                symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, request.ApiType);

            var orders = await Trading.GetOpenOrdersAsync(AccountType.Spot, symbol).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, default);

            return orders.AsExchangeResult(Exchange, orders.Data.Items.Select(x => new SharedSpotOrder(
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.OrderType),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId,
                Price = x.Price,
                Quantity = x.QuantityAsset == null || x.QuantityAsset == request.BaseAsset ? x.Quantity : null,
                QuantityFilled = x.QuantityFilled,
                UpdateTime = x.UpdateTime,
                QuoteQuantity = x.QuantityAsset == request.QuoteAsset ? x.Quantity : null,
                QuoteQuantityFilled = x.ValueFilled,
                Fee = x.FeeBaseAsset > 0 ? x.FeeBaseAsset : x.FeeQuoteAsset,
                FeeAsset = x.FeeBaseAsset > 0 ? request.BaseAsset : x.FeeQuoteAsset > 0 ? request.QuoteAsset : null,
                TimeInForce = ParseTimeInForce(x.OrderType)
            }));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetClosedOrdersAsync(GetSpotClosedOrdersRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            // Determine page token
            int page = 1;
            int pageSize = request.Filter?.Limit ?? 500;
            if (pageToken is PageToken token)
            {
                page = token.Page;
                pageSize = token.PageSize;
            }

            // Get data
            var orders = await Trading.GetClosedOrdersAsync(
                AccountType.Spot,
                request.GetSymbol(FormatSymbol),
                page: page,
                pageSize: pageSize).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedSpotOrder>>(Exchange, default);

            // Get next token
            PageToken? nextToken = null;
            if (orders.Data.HasNext == true)
                nextToken = new PageToken(page + 1, pageSize);

            return orders.AsExchangeResult(Exchange, orders.Data.Items.Select(x => new SharedSpotOrder(
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.OrderType),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId,
                Price = x.Price,
                Quantity = x.QuantityAsset == null || x.QuantityAsset == request.BaseAsset ? x.Quantity : null,
                QuantityFilled = x.QuantityFilled,
                UpdateTime = x.UpdateTime,
                QuoteQuantity = x.QuantityAsset == request.QuoteAsset ? x.Quantity : null,
                QuoteQuantityFilled = x.ValueFilled,
                Fee = x.FeeBaseAsset > 0 ? x.FeeBaseAsset : x.FeeQuoteAsset,
                FeeAsset = x.FeeBaseAsset > 0 ? request.BaseAsset : x.FeeQuoteAsset > 0 ? request.QuoteAsset : null,
                TimeInForce = ParseTimeInForce(x.OrderType)
            }), nextToken);
        }

        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, new ArgumentError("Invalid order id"));

            var orders = await Trading.GetOrderTradesAsync(request.GetSymbol(FormatSymbol), AccountType.Spot, orderId: orderId).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, default);

            return orders.AsExchangeResult(Exchange, orders.Data.Items.Select(x => new SharedUserTrade(
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.Quantity,
                x.Price,
                x.CreateTime)
            {
                Role = x.Role == TransactionRole.Maker ? SharedRole.Maker : SharedRole.Taker,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
            }));
        }

        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            // Determine page token
            int page = 1;
            int pageSize = request.Filter?.Limit ?? 500;
            if (pageToken is PageToken token)
            {
                page = token.Page;
                pageSize = token.PageSize;
            }

            // Get data
            var orders = await Trading.GetUserTradesAsync(
                request.GetSymbol(FormatSymbol),
                AccountType.Spot,
                startTime: request.Filter?.StartTime,
                endTime: request.Filter?.EndTime,
                page: page,
                pageSize: pageSize).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<IEnumerable<SharedUserTrade>>(Exchange, default);

            // Get next token
            PageToken? nextToken = null;
            if (orders.Data.HasNext)
                nextToken = new PageToken(page + 1, pageSize);

            return orders.AsExchangeResult(Exchange, orders.Data.Items.Select(x => new SharedUserTrade(
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.Quantity,
                x.Price,
                x.CreateTime)
            {
                Role = x.Role == TransactionRole.Maker ? SharedRole.Maker : SharedRole.Taker,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
            }), nextToken);
        }

        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.CancelOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.GetSymbol(FormatSymbol), AccountType.Spot, orderId).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, default);

            return order.AsExchangeResult(Exchange, new SharedId(order.Data.Id.ToString()));
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
    }
}
