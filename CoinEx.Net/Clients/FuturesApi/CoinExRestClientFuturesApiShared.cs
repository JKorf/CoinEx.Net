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

namespace CoinEx.Net.Clients.FuturesApi
{
    internal partial class CoinExRestClientFuturesApi : ICoinExRestClientFuturesApiShared
    {
        private const string _topicId = "CoinExFutures";
        public string Exchange => CoinExExchange.ExchangeName;
        public TradingMode[] SupportedTradingModes { get; } = new[] { TradingMode.PerpetualLinear, TradingMode.PerpetualInverse };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();

        #region Balance Client
        EndpointOptions<GetBalancesRequest> IBalanceRestClient.GetBalancesOptions { get; } = new EndpointOptions<GetBalancesRequest>(true);

        async Task<ExchangeWebResult<SharedBalance[]>> IBalanceRestClient.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = ((IBalanceRestClient)this).GetBalancesOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedBalance[]>(Exchange, validationError);

            var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedBalance[]>(Exchange, null, default);

            return result.AsExchangeResult<SharedBalance[]>(Exchange, SupportedTradingModes, result.Data.Select(x => new SharedBalance(x.Asset, x.Available, x.Available + x.Frozen)).ToArray());
        }

        #endregion

        #region Ticker client

        EndpointOptions<GetTickerRequest> IFuturesTickerRestClient.GetFuturesTickerOptions { get; } = new EndpointOptions<GetTickerRequest>(false);
        async Task<ExchangeWebResult<SharedFuturesTicker>> IFuturesTickerRestClient.GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTickerRestClient)this).GetFuturesTickerOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesTicker>(Exchange, validationError);

            var resultTicker = ExchangeData.GetTickersAsync(new[] { request.Symbol.GetSymbol(FormatSymbol) }, ct: ct);
            var resultFunding = ExchangeData.GetFundingRatesAsync(new[] { request.Symbol.GetSymbol(FormatSymbol) }, ct: ct);
            await Task.WhenAll(resultTicker, resultFunding).ConfigureAwait(false);
            if (!resultTicker.Result)
                return resultTicker.Result.AsExchangeResult<SharedFuturesTicker>(Exchange, null, default);
            if (!resultFunding.Result)
                return resultFunding.Result.AsExchangeResult<SharedFuturesTicker>(Exchange, null, default);

            var ticker = resultTicker.Result.Data.SingleOrDefault();
            var funding = resultFunding.Result.Data.SingleOrDefault();

            if (ticker == null || funding == null)
                return resultTicker.Result.AsExchangeError<SharedFuturesTicker>(Exchange, new ServerError("Not found"));

            return resultTicker.Result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesTicker(ExchangeSymbolCache.ParseSymbol(_topicId, ticker.Symbol), ticker.Symbol, ticker.LastPrice, ticker.HighPrice, ticker.LowPrice, ticker.Volume, ticker.OpenPrice == 0 ? null : Math.Round(ticker.LastPrice / ticker.OpenPrice * 100 - 100, 2))
            {
                IndexPrice = ticker.IndexPrice,
                MarkPrice = ticker.MarkPrice,
                FundingRate = funding.NextFundingRate,
                NextFundingTime = funding.NextFundingTime
            });
        }

        EndpointOptions<GetTickersRequest> IFuturesTickerRestClient.GetFuturesTickersOptions { get; } = new EndpointOptions<GetTickersRequest>(false);
        async Task<ExchangeWebResult<SharedFuturesTicker[]>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTickerRestClient)this).GetFuturesTickersOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesTicker[]>(Exchange, validationError);

            var resultTickers = ExchangeData.GetTickersAsync(ct: ct);
            var resultFunding = ExchangeData.GetFundingRatesAsync(ct: ct);
            await Task.WhenAll(resultTickers, resultFunding).ConfigureAwait(false);
            if (!resultTickers.Result)
                return resultTickers.Result.AsExchangeResult<SharedFuturesTicker[]>(Exchange, null, default);
            if (!resultFunding.Result)
                return resultFunding.Result.AsExchangeResult<SharedFuturesTicker[]>(Exchange, null, default);

            return resultTickers.Result.AsExchangeResult<SharedFuturesTicker[]>(Exchange, SupportedTradingModes, resultTickers.Result.Data.Select(x =>
            {
                var funding = resultFunding.Result.Data.Single(p => p.Symbol == x.Symbol);
                return new SharedFuturesTicker(ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol), x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice, x.Volume, x.OpenPrice == 0 ? null : Math.Round(x.LastPrice / x.OpenPrice * 100 - 100, 2))
                {
                    IndexPrice = x.IndexPrice,
                    MarkPrice = x.MarkPrice,
                    FundingRate = funding.NextFundingRate,
                    NextFundingTime = funding.NextFundingTime
                };
            }).ToArray());
        }

        #endregion

        #region Book Ticker client

        EndpointOptions<GetBookTickerRequest> IBookTickerRestClient.GetBookTickerOptions { get; } = new EndpointOptions<GetBookTickerRequest>(false);
        async Task<ExchangeWebResult<SharedBookTicker>> IBookTickerRestClient.GetBookTickerAsync(GetBookTickerRequest request, CancellationToken ct)
        {
            var validationError = ((IBookTickerRestClient)this).GetBookTickerOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedBookTicker>(Exchange, validationError);

            var resultTicker = await ExchangeData.GetOrderBookAsync(request.Symbol.GetSymbol(FormatSymbol), 5, ct: ct).ConfigureAwait(false);
            if (!resultTicker)
                return resultTicker.AsExchangeResult<SharedBookTicker>(Exchange, null, default);

            return resultTicker.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedBookTicker(
                ExchangeSymbolCache.ParseSymbol(_topicId, resultTicker.Data.Symbol),
                resultTicker.Data.Symbol,
                resultTicker.Data.Data.Asks[0].Price,
                resultTicker.Data.Data.Asks[0].Quantity,
                resultTicker.Data.Data.Bids[0].Price,
                resultTicker.Data.Data.Bids[0].Quantity));
        }

        #endregion

        #region Futures Symbol client

        EndpointOptions<GetSymbolsRequest> IFuturesSymbolRestClient.GetFuturesSymbolsOptions { get; } = new EndpointOptions<GetSymbolsRequest>(false);
        async Task<ExchangeWebResult<SharedFuturesSymbol[]>> IFuturesSymbolRestClient.GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesSymbolRestClient)this).GetFuturesSymbolsOptions.ValidateRequest(Exchange, request, request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesSymbol[]>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolsAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFuturesSymbol[]>(Exchange, null, default);

            IEnumerable<CoinExFuturesSymbol> data = result.Data;
            if (request.TradingMode.HasValue)
            {
                data = data.Where(x =>
                    request.TradingMode == TradingMode.PerpetualLinear ? x.ContractType == ContractType.Linear : x.ContractType == ContractType.Inverse);
            }

            var response = result.AsExchangeResult<SharedFuturesSymbol[]>(Exchange, 
                request.TradingMode == null ? SupportedTradingModes: new[] { request.TradingMode.Value },
                data.Select(s => new SharedFuturesSymbol(
                s.ContractType == ContractType.Inverse ? TradingMode.PerpetualInverse : TradingMode.PerpetualLinear,
                s.BaseAsset, s.QuoteAsset, s.Symbol, s.TradingAvailable)
            {
                MinTradeQuantity = s.MinOrderQuantity,
                QuantityDecimals = s.QuantityPrecision,
                PriceDecimals = s.PricePrecision,
                ContractSize = 1
            }).ToArray());

            ExchangeSymbolCache.UpdateSymbolInfo(_topicId, response.Data);
            return response;
        }

        #endregion

        #region Futures Order Client

        SharedFeeDeductionType IFuturesOrderRestClient.FuturesFeeDeductionType => SharedFeeDeductionType.AddToCost;
        SharedFeeAssetType IFuturesOrderRestClient.FuturesFeeAssetType => SharedFeeAssetType.InputAsset;
        SharedOrderType[] IFuturesOrderRestClient.FuturesSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market };
        SharedTimeInForce[] IFuturesOrderRestClient.FuturesSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };
        SharedQuantitySupport IFuturesOrderRestClient.FuturesSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset);

        string IFuturesOrderRestClient.GenerateClientOrderId() => ExchangeHelpers.RandomString(20);

        PlaceFuturesOrderOptions IFuturesOrderRestClient.PlaceFuturesOrderOptions { get; } = new PlaceFuturesOrderOptions(false);
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderRestClient.PlaceFuturesOrderAsync(PlaceFuturesOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).PlaceFuturesOrderOptions.ValidateRequest(
                Exchange,
                request,
                request.Symbol.TradingMode,
                SupportedTradingModes,
                ((IFuturesOrderRestClient)this).FuturesSupportedOrderTypes,
                ((IFuturesOrderRestClient)this).FuturesSupportedTimeInForce,
                ((IFuturesOrderRestClient)this).FuturesSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var result = await Trading.PlaceOrderAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                GetOrderSide(request.Side, request.PositionSide),
                GetOrderType(request.OrderType, request.TimeInForce),
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInContracts ?? 0,
                price: request.Price,
                clientOrderId: request.ClientOrderId,
                ct: ct).ConfigureAwait(false);

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(result.Data.Id.ToString()));
        }

        EndpointOptions<GetOrderRequest> IFuturesOrderRestClient.GetFuturesOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true);
        async Task<ExchangeWebResult<SharedFuturesOrder>> IFuturesOrderRestClient.GetFuturesOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedFuturesOrder>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.GetOrderAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedFuturesOrder>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, order.Data.Symbol),
                order.Data.Symbol,
                order.Data.Id.ToString(),
                ParseOrderType(order.Data.OrderType),
                order.Data.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(order.Data.Status ?? OrderStatusV2.Open),
                order.Data.CreateTime)
            {
                ClientOrderId = order.Data.ClientOrderId,
                OrderPrice = order.Data.Price,
                OrderQuantity = new SharedOrderQuantity(order.Data.Quantity, contractQuantity: order.Data.Quantity),
                QuantityFilled = new SharedOrderQuantity(order.Data.QuantityFilled, order.Data.ValueFilled, contractQuantity: order.Data.QuantityFilled),
                TimeInForce = ParseTimeInForce(order.Data.OrderType),
                UpdateTime = order.Data.UpdateTime,
                Fee = order.Data.Fee,
                FeeAsset = order.Data.FeeAsset
            });
        }

        EndpointOptions<GetOpenOrdersRequest> IFuturesOrderRestClient.GetOpenFuturesOrdersOptions { get; } = new EndpointOptions<GetOpenOrdersRequest>(true);
        async Task<ExchangeWebResult<SharedFuturesOrder[]>> IFuturesOrderRestClient.GetOpenFuturesOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetOpenFuturesOrdersOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesOrder[]>(Exchange, validationError);

            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var orders = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedFuturesOrder[]>(Exchange, null, default);

            return orders.AsExchangeResult<SharedFuturesOrder[]>(Exchange, request.Symbol == null ? SupportedTradingModes : new[] { request.Symbol.TradingMode }, orders.Data.Items.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol), 
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.OrderType),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status ?? OrderStatusV2.Open),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId,
                OrderPrice = x.Price,
                OrderQuantity = new SharedOrderQuantity(x.Quantity, contractQuantity: x.Quantity),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.ValueFilled, contractQuantity: x.QuantityFilled),
                TimeInForce = ParseTimeInForce(x.OrderType),
                UpdateTime = x.UpdateTime,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset
            }).ToArray());
        }

        PaginatedEndpointOptions<GetClosedOrdersRequest> IFuturesOrderRestClient.GetClosedFuturesOrdersOptions { get; } = new PaginatedEndpointOptions<GetClosedOrdersRequest>(SharedPaginationSupport.Descending, true, 50, true);
        async Task<ExchangeWebResult<SharedFuturesOrder[]>> IFuturesOrderRestClient.GetClosedFuturesOrdersAsync(GetClosedOrdersRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetClosedFuturesOrdersOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesOrder[]>(Exchange, validationError);

            // Determine page token
            int page = 1;
            int pageSize = request.Limit ?? 50;
            if (pageToken is PageToken token)
            {
                page = token.Page;
                pageSize = token.PageSize;
            }

            // Get data
            var orders = await Trading.GetClosedOrdersAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                page: page,
                pageSize: pageSize,
                ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedFuturesOrder[]>(Exchange, null, default);

            // Get next token
            PageToken? nextToken = null;
            if (orders.Data.HasNext)
                nextToken = new PageToken(page + 1, pageSize);

            return orders.AsExchangeResult<SharedFuturesOrder[]>(Exchange, SupportedTradingModes ,orders.Data.Items.Select(x => new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol), 
                x.Symbol,
                x.Id.ToString(),
                ParseOrderType(x.OrderType),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(x.Status ?? OrderStatusV2.Open),
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId,
                OrderPrice = x.Price,
                OrderQuantity = new SharedOrderQuantity(x.Quantity, contractQuantity: x.Quantity),
                QuantityFilled = new SharedOrderQuantity(x.QuantityFilled, x.ValueFilled, contractQuantity: x.QuantityFilled),
                TimeInForce = ParseTimeInForce(x.OrderType),
                UpdateTime = x.UpdateTime,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset
            }).ToArray(), nextToken);
        }

        EndpointOptions<GetOrderTradesRequest> IFuturesOrderRestClient.GetFuturesOrderTradesOptions { get; } = new EndpointOptions<GetOrderTradesRequest>(true);
        async Task<ExchangeWebResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesOrderTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, new ArgumentError("Invalid order id"));

            var orders = await Trading.GetOrderTradesAsync(request.Symbol.GetSymbol(FormatSymbol), orderId: orderId, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, null, default);

            return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, request.Symbol.TradingMode,orders.Data.Items.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Quantity,
                x.Price,
                x.CreateTime)
            {
                Price = x.Price,
                Quantity = x.Quantity,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
                Role = x.Role == TransactionRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray());
        }

        PaginatedEndpointOptions<GetUserTradesRequest> IFuturesOrderRestClient.GetFuturesUserTradesOptions { get; } = new PaginatedEndpointOptions<GetUserTradesRequest>(SharedPaginationSupport.Descending, true, 50, true);
        async Task<ExchangeWebResult<SharedUserTrade[]>> IFuturesOrderRestClient.GetFuturesUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesUserTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedUserTrade[]>(Exchange, validationError);

            // Determine page token
            int page = 1;
            int pageSize = request.Limit ?? 50;
            if (pageToken is PageToken token)
            {
                page = token.Page;
                pageSize = token.PageSize;
            }

            // Get data
            var orders = await Trading.GetUserTradesAsync(request.Symbol.GetSymbol(FormatSymbol),
                startTime: request.StartTime,
                endTime: request.EndTime,
                page: page,
                pageSize: pageSize,
                ct: ct
                ).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, null, default);

            // Get next token
            PageToken? nextToken = null;
            if (orders.Data.HasNext)
                nextToken = new PageToken(page + 1, pageSize);

            return orders.AsExchangeResult<SharedUserTrade[]>(Exchange, request.Symbol.TradingMode,orders.Data.Items.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Quantity,
                x.Price,
                x.CreateTime)
            {
                Price = x.Price,
                Quantity = x.Quantity,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
                Role = x.Role == TransactionRole.Maker ? SharedRole.Maker : SharedRole.Taker
            }).ToArray(), nextToken);
        }

        EndpointOptions<CancelOrderRequest> IFuturesOrderRestClient.CancelFuturesOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderRestClient.CancelFuturesOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).CancelFuturesOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return new ExchangeWebResult<SharedId>(Exchange, new ArgumentError("Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(order.Data.Id.ToString()));
        }

        EndpointOptions<GetPositionsRequest> IFuturesOrderRestClient.GetPositionsOptions { get; } = new EndpointOptions<GetPositionsRequest>(true);
        async Task<ExchangeWebResult<SharedPosition[]>> IFuturesOrderRestClient.GetPositionsAsync(GetPositionsRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetPositionsOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedPosition[]>(Exchange, validationError);

            var result = await Trading.GetPositionsAsync(symbol: request.Symbol?.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedPosition[]>(Exchange, null, default);

            return result.AsExchangeResult<SharedPosition[]>(Exchange, request.Symbol == null ? SupportedTradingModes : new[] { request.Symbol.TradingMode }, result.Data.Items.Select(x => new SharedPosition(ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol), x.Symbol, x.OpenInterest, x.UpdateTime)
            {
                UnrealizedPnl = x.UnrealizedPnl,
                LiquidationPrice = x.LiquidationPrice,
                AverageOpenPrice = x.AverageEntryPrice,
                Leverage = x.Leverage,
                StopLossPrice = x.StopLossPrice == 0 ? null : x.StopLossPrice,
                TakeProfitPrice = x.TakeProfitPrice == 0 ? null : x.TakeProfitPrice,
                PositionSide = x.Side == PositionSide.Short ? SharedPositionSide.Short : SharedPositionSide.Long
            }).ToArray());
        }

        EndpointOptions<ClosePositionRequest> IFuturesOrderRestClient.ClosePositionOptions { get; } = new EndpointOptions<ClosePositionRequest>(true);
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderRestClient.ClosePositionAsync(ClosePositionRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).ClosePositionOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var symbol = request.Symbol.GetSymbol(FormatSymbol);
            var result = await Trading.ClosePositionAsync(symbol, OrderTypeV2.Market, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(result.Data.Id.ToString()));
        }

        private OrderSide GetOrderSide(SharedOrderSide side, SharedPositionSide? posSide)
        {
            if (posSide == null)
                return side == SharedOrderSide.Sell ? OrderSide.Sell : OrderSide.Buy;

            if (posSide == SharedPositionSide.Long)
            {
                if (side == SharedOrderSide.Buy) return OrderSide.Buy;
                return OrderSide.Sell;
            }

            if (side == SharedOrderSide.Buy) return OrderSide.Sell;
            return OrderSide.Sell;
        }

        private OrderTypeV2 GetOrderType(SharedOrderType type, SharedTimeInForce? tif)
        {
            if (type == SharedOrderType.LimitMaker) return OrderTypeV2.PostOnly;
            if (type == SharedOrderType.Market) return OrderTypeV2.Market;

            if (tif == SharedTimeInForce.FillOrKill) return OrderTypeV2.FillOrKill;
            if (tif == SharedTimeInForce.ImmediateOrCancel) return OrderTypeV2.ImmediateOrCancel;
            return OrderTypeV2.Limit;
        }

        private SharedOrderStatus ParseOrderStatus(OrderStatusV2 status)
        {
            if (status == OrderStatusV2.Open || status == OrderStatusV2.PartiallyFilled) return SharedOrderStatus.Open;
            if (status == OrderStatusV2.Canceled || status == OrderStatusV2.PartiallyCanceled) return SharedOrderStatus.Canceled;
            return SharedOrderStatus.Filled;
        }

        private SharedOrderType ParseOrderType(OrderTypeV2 type)
        {
            if (type == OrderTypeV2.Market) return SharedOrderType.Market;
            if (type == OrderTypeV2.Limit) return SharedOrderType.Limit;
            if (type == OrderTypeV2.PostOnly) return SharedOrderType.LimitMaker;

            return SharedOrderType.Other;
        }

        private SharedTimeInForce? ParseTimeInForce(OrderTypeV2 tif)
        {
            if (tif == OrderTypeV2.ImmediateOrCancel) return SharedTimeInForce.ImmediateOrCancel;
            if (tif == OrderTypeV2.FillOrKill) return SharedTimeInForce.FillOrKill;

            return null;
        }

        #endregion

        #region Futures Client Id Order Client

        EndpointOptions<GetOrderRequest> IFuturesOrderClientIdClient.GetFuturesOrderByClientOrderIdOptions { get; } = new EndpointOptions<GetOrderRequest>(true);
        async Task<ExchangeWebResult<SharedFuturesOrder>> IFuturesOrderClientIdClient.GetFuturesOrderByClientOrderIdAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).GetFuturesOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesOrder>(Exchange, validationError);

            var order = await Trading.GetOpenOrdersAsync(clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedFuturesOrder>(Exchange, null, default);

            var orderData = order.Data.Items.FirstOrDefault();
            if (orderData == null)
            {
                order = await Trading.GetClosedOrdersAsync(clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
                if (!order)
                    return order.AsExchangeResult<SharedFuturesOrder>(Exchange, null, default);

                orderData = order.Data.Items.FirstOrDefault();
            }

            if (orderData == null)
                return new ExchangeWebResult<SharedFuturesOrder>(Exchange, new ServerError("Order not found"));

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, orderData.Symbol),
                orderData.Symbol,
                orderData.Id.ToString(),
                ParseOrderType(orderData.OrderType),
                orderData.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                ParseOrderStatus(orderData.Status ?? OrderStatusV2.Open),
                orderData.CreateTime)
            {
                ClientOrderId = orderData.ClientOrderId,
                OrderPrice = orderData.Price,
                OrderQuantity = new SharedOrderQuantity(orderData.Quantity, contractQuantity: orderData.Quantity),
                QuantityFilled = new SharedOrderQuantity(orderData.QuantityFilled, orderData.ValueFilled, contractQuantity: orderData.QuantityFilled),
                TimeInForce = ParseTimeInForce(orderData.OrderType),
                UpdateTime = orderData.UpdateTime,
                Fee = orderData.Fee,
                FeeAsset = orderData.FeeAsset
            });
        }

        EndpointOptions<CancelOrderRequest> IFuturesOrderClientIdClient.CancelFuturesOrderByClientOrderIdOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> IFuturesOrderClientIdClient.CancelFuturesOrderByClientOrderIdAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesOrderRestClient)this).CancelFuturesOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var order = await Trading.CancelOrderByClientOrderIdAsync(request.Symbol.GetSymbol(FormatSymbol), clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(order.Data.FirstOrDefault()?.Data?.Id.ToString() ?? request.OrderId));
        }
        #endregion

        #region Klines client

        GetKlinesOptions IKlineRestClient.GetKlinesOptions { get; } = new GetKlinesOptions(SharedPaginationSupport.Descending, true, 1000, false,
            SharedKlineInterval.OneMinute,
            SharedKlineInterval.FiveMinutes,
            SharedKlineInterval.FifteenMinutes,
            SharedKlineInterval.ThirtyMinutes,
            SharedKlineInterval.OneHour,
            SharedKlineInterval.TwoHours,
            SharedKlineInterval.FourHours,
            SharedKlineInterval.SixHours,
            SharedKlineInterval.TwelveHours,
            SharedKlineInterval.OneDay,
            SharedKlineInterval.OneWeek);

        async Task<ExchangeWebResult<SharedKline[]>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeWebResult<SharedKline[]>(Exchange, new ArgumentError("Interval not supported"));

            var validationError = ((IKlineRestClient)this).GetKlinesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedKline[]>(Exchange, validationError);

            var result = await ExchangeData.GetKlinesAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                interval,
                request.Limit ?? 1000,
                ct: ct
                ).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedKline[]>(Exchange, null, default);

            return result.AsExchangeResult<SharedKline[]>(Exchange, request.Symbol.TradingMode, result.Data.Reverse().Select(x => new SharedKline(x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice, x.Volume)).ToArray());
        }

        #endregion

        #region Mark Price Klines client

        GetKlinesOptions IMarkPriceKlineRestClient.GetMarkPriceKlinesOptions { get; } = new GetKlinesOptions(SharedPaginationSupport.Descending, true, 1000, false);

        async Task<ExchangeWebResult<SharedFuturesKline[]>> IMarkPriceKlineRestClient.GetMarkPriceKlinesAsync(GetKlinesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeWebResult<SharedFuturesKline[]>(Exchange, new ArgumentError("Interval not supported"));

            var validationError = ((IKlineRestClient)this).GetKlinesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesKline[]>(Exchange, validationError);

            var result = await ExchangeData.GetKlinesAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                interval,
                request.Limit ?? 1000,
                priceType: PriceType.MarkPrice,
                ct: ct
                ).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFuturesKline[]>(Exchange, null, default);

            return result.AsExchangeResult<SharedFuturesKline[]>(Exchange, request.Symbol.TradingMode, result.Data.Reverse().Select(x => new SharedFuturesKline(x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice)).ToArray());
        }

        #endregion

        #region Index Price Klines client

        GetKlinesOptions IIndexPriceKlineRestClient.GetIndexPriceKlinesOptions { get; } = new GetKlinesOptions(SharedPaginationSupport.Descending, true, 1000, false);

        async Task<ExchangeWebResult<SharedFuturesKline[]>> IIndexPriceKlineRestClient.GetIndexPriceKlinesAsync(GetKlinesRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeWebResult<SharedFuturesKline[]>(Exchange, new ArgumentError("Interval not supported"));

            var validationError = ((IKlineRestClient)this).GetKlinesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesKline[]>(Exchange, validationError);

            var result = await ExchangeData.GetKlinesAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                interval,
                request.Limit ?? 1000,
                priceType: PriceType.IndexPrice,
                ct: ct
                ).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFuturesKline[]>(Exchange, null, default);

            return result.AsExchangeResult<SharedFuturesKline[]>(Exchange, request.Symbol.TradingMode, result.Data.Reverse().Select(x => new SharedFuturesKline(x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice)).ToArray());
        }

        #endregion

        #region Recent Trade client

        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(1000, false);
        async Task<ExchangeWebResult<SharedTrade[]>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = ((IRecentTradeRestClient)this).GetRecentTradesOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedTrade[]>(Exchange, validationError);

            var result = await ExchangeData.GetTradeHistoryAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedTrade[]>(Exchange, null, default);

            return result.AsExchangeResult<SharedTrade[]>(Exchange, request.Symbol.TradingMode, result.Data.Select(x => new SharedTrade(x.Quantity, x.Price, x.Timestamp)
            {
                Side = x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell
            }).ToArray());
        }

        #endregion

        #region Leverage client
        SharedLeverageSettingMode ILeverageRestClient.LeverageSettingType => SharedLeverageSettingMode.PerSymbol;

        EndpointOptions<GetLeverageRequest> ILeverageRestClient.GetLeverageOptions { get; } = new EndpointOptions<GetLeverageRequest>(true)
        {
            RequestNotes = "Leverage can only be retrieved when there is an open position, else it will return 0"
        };
        async Task<ExchangeWebResult<SharedLeverage>> ILeverageRestClient.GetLeverageAsync(GetLeverageRequest request, CancellationToken ct)
        {
            var validationError = ((ILeverageRestClient)this).GetLeverageOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedLeverage>(Exchange, validationError);

            var result = await Trading.GetPositionsAsync(symbol: request.Symbol.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedLeverage>(Exchange, null, default);

            var sideData =  result.Data.Items.FirstOrDefault(); // Leverage can't be changed per side, so doesn't matter which we take
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedLeverage(sideData?.Leverage ?? 0)
            {
                Side = request.PositionSide
            });
        }

        SetLeverageOptions ILeverageRestClient.SetLeverageOptions { get; } = new SetLeverageOptions()
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(SetLeverageRequest.MarginMode), typeof(SharedMarginMode), "Isolated or cross margin", SharedMarginMode.Cross)
            }
        };
        async Task<ExchangeWebResult<SharedLeverage>> ILeverageRestClient.SetLeverageAsync(SetLeverageRequest request, CancellationToken ct)
        {
            var validationError = ((ILeverageRestClient)this).SetLeverageOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedLeverage>(Exchange, validationError);

            var result = await Account.SetLeverageAsync(
                symbol: request.Symbol.GetSymbol(FormatSymbol),
                request.MarginMode == SharedMarginMode.Isolated ? MarginMode.Isolated : MarginMode.Cross,
                (int)request.Leverage,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedLeverage>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedLeverage(result.Data.Leverage));
        }
        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(new[] { 5, 10, 20, 50 }, false);
        async Task<ExchangeWebResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = ((IOrderBookRestClient)this).GetOrderBookOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                limit: request.Limit ?? 20,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedOrderBook>(Exchange, null, default);

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedOrderBook(result.Data.Data.Asks, result.Data.Data.Bids));
        }

        #endregion

        #region Open Interest client

        EndpointOptions<GetOpenInterestRequest> IOpenInterestRestClient.GetOpenInterestOptions { get; } = new EndpointOptions<GetOpenInterestRequest>(true);
        async Task<ExchangeWebResult<SharedOpenInterest>> IOpenInterestRestClient.GetOpenInterestAsync(GetOpenInterestRequest request, CancellationToken ct)
        {
            var validationError = ((IOpenInterestRestClient)this).GetOpenInterestOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedOpenInterest>(Exchange, validationError);

            var result = await ExchangeData.GetSymbolsAsync(new[] { request.Symbol.GetSymbol(FormatSymbol) }, ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedOpenInterest>(Exchange, null, default);

            var symbol = result.Data.SingleOrDefault();
            if (symbol == null)
                return result.AsExchangeError<SharedOpenInterest>(Exchange, new ServerError("Symbol not found"));

            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedOpenInterest(symbol.OpenInterestVolume));
        }

        #endregion

        #region Funding Rate client
        GetFundingRateHistoryOptions IFundingRateRestClient.GetFundingRateHistoryOptions { get; } = new GetFundingRateHistoryOptions(SharedPaginationSupport.Descending, true, 100, false);

        async Task<ExchangeWebResult<SharedFundingRate[]>> IFundingRateRestClient.GetFundingRateHistoryAsync(GetFundingRateHistoryRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IFundingRateRestClient)this).GetFundingRateHistoryOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFundingRate[]>(Exchange, validationError);

            int page = 1;
            int pageSize = 100;
            if (pageToken is PageToken token)
            {
                page = token.Page;
                pageSize = token.PageSize;
            }

            // Get data
            var result = await ExchangeData.GetFundingRateHistoryAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                startTime: request.StartTime,
                endTime: request.EndTime,
                page: page,
                pageSize: pageSize,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFundingRate[]>(Exchange, null, default);

            PageToken? nextToken = null;
            if (result.Data.HasNext)
                nextToken = new PageToken(page + 1, pageSize);

            // Return
            return result.AsExchangeResult<SharedFundingRate[]>(Exchange, request.Symbol.TradingMode,result.Data.Items.Select(x => new SharedFundingRate(x.ActualFundingRate, x.FundingTime ?? default)).ToArray(), nextToken);
        }
        #endregion

        #region Position History client

        GetPositionHistoryOptions IPositionHistoryRestClient.GetPositionHistoryOptions { get; } = new GetPositionHistoryOptions(SharedPaginationSupport.Descending, true, 100);
        async Task<ExchangeWebResult<SharedPositionHistory[]>> IPositionHistoryRestClient.GetPositionHistoryAsync(GetPositionHistoryRequest request, INextPageToken? pageToken, CancellationToken ct)
        {
            var validationError = ((IPositionHistoryRestClient)this).GetPositionHistoryOptions.ValidateRequest(Exchange, request, request.Symbol?.TradingMode ?? request.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedPositionHistory[]>(Exchange, validationError);

            // Determine page token
            int page = 1;
            int pageSize = request.Limit ?? 100;
            if (pageToken is PageToken token)
            {
                page = token.Page;
                pageSize = token.PageSize;
            }

            // Get data
            var orders = await Trading.GetPositionHistoryAsync(
                symbol: request.Symbol?.GetSymbol(FormatSymbol),
                startTime: request.StartTime,
                endTime: request.EndTime,
                page: page,
                pageSize: pageSize,
                ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedPositionHistory[]>(Exchange, null, default);

            // Get next token
            PageToken? nextToken = null;
            if (orders.Data.HasNext)
                nextToken = new PageToken(page + 1, pageSize);

            return orders.AsExchangeResult<SharedPositionHistory[]>(Exchange, request.Symbol == null ? SupportedTradingModes : new[] { request.Symbol.TradingMode }, orders.Data.Items.Select(x => new SharedPositionHistory(
                ExchangeSymbolCache.ParseSymbol(_topicId, x.Symbol),
                x.Symbol,
                x.Side == PositionSide.Long ? SharedPositionSide.Long : SharedPositionSide.Short,
                x.AverageEntryPrice,
                x.SettlePrice,
                x.AthPositionQuantity,
                x.RealizedPnl,
                x.UpdateTime ?? x.CreateTime)
            {
                Leverage = x.Leverage,
                PositionId = x.Id.ToString()
            }).ToArray(), nextToken);
        }
        #endregion

        #region Fee Client
        EndpointOptions<GetFeeRequest> IFeeRestClient.GetFeeOptions { get; } = new EndpointOptions<GetFeeRequest>(true);

        async Task<ExchangeWebResult<SharedFee>> IFeeRestClient.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = ((IFeeRestClient)this).GetFeeOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFee>(Exchange, validationError);

            // Get data
            var result = await Account.GetTradingFeesAsync(request.Symbol.GetSymbol(FormatSymbol), ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedFee>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFee(result.Data.MakerFeeRate * 100, result.Data.TakerFeeRate * 100));
        }
        #endregion

        #region Futures Trigger Order Client
        PlaceFuturesTriggerOrderOptions IFuturesTriggerOrderRestClient.PlaceFuturesTriggerOrderOptions { get; } = new PlaceFuturesTriggerOrderOptions(false)
        {
        };
        async Task<ExchangeWebResult<SharedId>> IFuturesTriggerOrderRestClient.PlaceFuturesTriggerOrderAsync(PlaceFuturesTriggerOrderRequest request, CancellationToken ct)
        {
            var side = GetOrderSide(request);
            var validationError = ((IFuturesTriggerOrderRestClient)this).PlaceFuturesTriggerOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes, side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell, ((IFuturesOrderRestClient)this).FuturesSupportedOrderQuantity);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var clientOrderId = request.ClientOrderId ?? ExchangeHelpers.RandomString(32);
            var result = await Trading.PlaceStopOrderAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                side,
                request.OrderPrice == null ? OrderTypeV2.Market : OrderTypeV2.Limit,
                quantity: request.Quantity?.QuantityInBaseAsset ?? request.Quantity?.QuantityInContracts ?? 0,
                price: request.OrderPrice,
                triggerPrice: request.TriggerPrice,
                triggerPriceType: GetTriggerPriceType(request),
                clientOrderId: clientOrderId,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(clientOrderId));
        }

        EndpointOptions<GetOrderRequest> IFuturesTriggerOrderRestClient.GetFuturesTriggerOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true)
        {
            RequestNotes = "Only pending trigger orders can be requested, executed trigger orders are not available in the API"
        };
        async Task<ExchangeWebResult<SharedFuturesTriggerOrder>> IFuturesTriggerOrderRestClient.GetFuturesTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTriggerOrderRestClient)this).GetFuturesTriggerOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedFuturesTriggerOrder>(Exchange, validationError);

            var status = SharedTriggerOrderStatus.Active;
            var orders = await Trading.GetOpenStopOrdersAsync(clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!orders)
                return orders.AsExchangeResult<SharedFuturesTriggerOrder>(Exchange, null, default);

            CoinExStopOrder order;
            if (orders.Data.Items.Any())
            {
                order = orders.Data.Items.Single();
            }
            else
            {
                orders = await Trading.GetClosedStopOrdersAsync(request.Symbol.GetSymbol(FormatSymbol), pageSize: 1000, ct: ct).ConfigureAwait(false);
                if (!orders)
                    return orders.AsExchangeResult<SharedFuturesTriggerOrder>(Exchange, null, default);

                order = orders.Data.Items.SingleOrDefault(x => x.ClientOrderId == request.OrderId)!;
                if (order == null)
                    return orders.AsExchangeError<SharedFuturesTriggerOrder>(Exchange, new ServerError("Order not found"));

                status = SharedTriggerOrderStatus.Filled;
            }

            return orders.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedFuturesTriggerOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, order.Symbol),
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

        EndpointOptions<CancelOrderRequest> IFuturesTriggerOrderRestClient.CancelFuturesTriggerOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> IFuturesTriggerOrderRestClient.CancelFuturesTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTriggerOrderRestClient)this).CancelFuturesTriggerOrderOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            var order = await Trading.CancelStopOrderByClientOrderIdAsync(
                request.Symbol.GetSymbol(FormatSymbol),
                request.OrderId,
                ct: ct).ConfigureAwait(false);
            if (!order)
                return order.AsExchangeResult<SharedId>(Exchange, null, default);

            return order.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(request.OrderId));
        }

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
        #endregion

        #region Tp/SL Client
        EndpointOptions<SetTpSlRequest> IFuturesTpSlRestClient.SetFuturesTpSlOptions { get; } = new EndpointOptions<SetTpSlRequest>(true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(PlaceFuturesTriggerOrderRequest.PositionMode), typeof(SharedPositionMode), "PositionMode the account is in", SharedPositionMode.OneWay)
            }
        };

        async Task<ExchangeWebResult<SharedId>> IFuturesTpSlRestClient.SetFuturesTpSlAsync(SetTpSlRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTpSlRestClient)this).SetFuturesTpSlOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            WebCallResult<CoinExPosition> result;
            if (request.TpSlSide == SharedTpSlSide.TakeProfit)
            {
                result = await Trading.SetTakeProfitAsync(
                    request.Symbol.GetSymbol(FormatSymbol),
                    PriceType.LastPrice,
                    request.TriggerPrice,
                    ct: ct).ConfigureAwait(false);
            }
            else
            {
                result = await Trading.SetTakeProfitAsync(
                                request.Symbol.GetSymbol(FormatSymbol),
                                PriceType.LastPrice,
                                request.TriggerPrice,
                                ct: ct).ConfigureAwait(false);
            }

            if (!result)
                return result.AsExchangeResult<SharedId>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, new SharedId(""));
        }

        EndpointOptions<CancelTpSlRequest> IFuturesTpSlRestClient.CancelFuturesTpSlOptions { get; } = new EndpointOptions<CancelTpSlRequest>(true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(CancelTpSlRequest.TpSlSide), typeof(SharedTpSlSide), "Take profit / stop loss side to cancel", SharedTpSlSide.TakeProfit)
            }
        };

        async Task<ExchangeWebResult<bool>> IFuturesTpSlRestClient.CancelFuturesTpSlAsync(CancelTpSlRequest request, CancellationToken ct)
        {
            var validationError = ((IFuturesTpSlRestClient)this).CancelFuturesTpSlOptions.ValidateRequest(Exchange, request, request.Symbol.TradingMode, SupportedTradingModes);
            if (validationError != null)
                return new ExchangeWebResult<bool>(Exchange, validationError);

            WebCallResult<CoinExPosition> result;
            if (request.TpSlSide == SharedTpSlSide.TakeProfit)
            {
                result = await Trading.SetTakeProfitAsync(
                    request.Symbol.GetSymbol(FormatSymbol),
                    PriceType.LastPrice,
                    0,
                    ct: ct).ConfigureAwait(false);
            }
            else
            {
                result = await Trading.SetStopLossAsync(
                                request.Symbol.GetSymbol(FormatSymbol),
                                PriceType.LastPrice,
                                0,
                                ct: ct).ConfigureAwait(false);
            }
            if (!result)
                return result.AsExchangeResult<bool>(Exchange, null, default);

            // Return
            return result.AsExchangeResult(Exchange, request.Symbol.TradingMode, true);
        }

        #endregion
    }
}
