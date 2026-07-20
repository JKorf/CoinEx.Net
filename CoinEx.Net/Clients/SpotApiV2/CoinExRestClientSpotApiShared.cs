using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Enums;
using CryptoExchange.Net;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net.Objects.Errors;

namespace CoinEx.Net.Clients.SpotApiV2
{
    internal partial class CoinExRestClientSpotApi : ICoinExRestClientSpotApiShared
    {
        private const string _topicId = "CoinExSpot";
        private const string _exchangeName = "CoinEx";
        public TradingMode[] SupportedTradingModes => new[] { TradingMode.Spot };

        public void SetDefaultExchangeParameter(string key, object value) => ExchangeParameters.SetStaticParameter(Exchange, key, value);
        public void ResetDefaultExchangeParameters() => ExchangeParameters.ResetStaticParameters();
        public SharedClientInfo Discover() => SharedUtils.GetClientInfo(CoinExExchange.Metadata, this);

        #region Kline client

        GetKlinesOptions IKlineRestClient.GetKlinesOptions { get; } = new GetKlinesOptions(_exchangeName, true, true, true, 1000, false,
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
            SharedKlineInterval.OneWeek)
        {
        };

        async Task<HttpResult<SharedKline[]>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;

            var validationError = SharedClient.GetKlinesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedKline[]>(Exchange, validationError);

            var direction = request.Direction ?? DataDirection.Ascending;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 1000;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, false);

            // Get data
            var result = await ExchangeData.GetKlinesAsync(
                symbol,
                interval,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: limit,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedKline[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () =>
                    {
                        if (direction == DataDirection.Ascending)
                            return Pagination.NextPageFromTime(pageParams, result.Data.Max(x => x.OpenTime));
                        else
                            return Pagination.NextPageFromTime(pageParams, result.Data.Min(x => x.OpenTime));
                    },
                    result.Data.Length,
                    result.Data.Select(x => x.OpenTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data, x => x.OpenTime, request.StartTime, request.EndTime, direction)
                   .Select(x => 
                       new SharedKline(
                           request.Symbol, 
                           symbol,
                           x.OpenTime,
                           x.ClosePrice,
                           x.HighPrice,
                           x.LowPrice,
                           x.OpenPrice,
                           x.Volume))
                   .ToArray(), nextPageRequest);
        }

        #endregion

        #region Spot Symbol client

        SharedSymbolCatalog? ISpotSymbolRestClient.SpotSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_exchangeName, _topicId, EnvironmentName, null);
        GetSpotSymbolsOptions ISpotSymbolRestClient.GetSpotSymbolsOptions { get; } = new GetSpotSymbolsOptions(_exchangeName, false);
        async Task<HttpResult<SharedSpotSymbol[]>> ISpotSymbolRestClient.GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotSymbol[]>(Exchange, validationError);

            var symbolsTask = ExchangeData.GetSymbolsAsync(ct: ct);
            var assetsTask = ExchangeData.GetAssetsAsync(ct: ct);
            await Task.WhenAll(symbolsTask, assetsTask).ConfigureAwait(false);
            var symbolsResult = symbolsTask.Result;
            var assetsResult = assetsTask.Result;
            if (!symbolsResult.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(symbolsResult);
            if (!assetsResult.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(assetsResult);

            var data = symbolsResult.Data
                .Select(x => ParseSymbol(x, assetsResult.Data))
                .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicId, EnvironmentName, null, data);
            return HttpResult.Ok(symbolsResult, SharedUtils.ApplySymbolFilter(data, request));
        }

        private SharedSpotSymbol ParseSymbol(CoinExSymbol s, CoinExAsset[] assets)
        {
            var result = new SharedSpotSymbol(s.BaseAsset, s.QuoteAsset, s.Name, true)
            {
                MinTradeQuantity = s.MinOrderQuantity,
                PriceDecimals = s.PricePrecision,
                QuantityDecimals = s.QuantityPrecision,
                DisplayName = s.Name
            };

            if (LibraryHelpers.IsCommodity(result.BaseAsset))
            {
                result.BaseAssetType = SharedAssetType.TradFi;
                result.BaseAssetSubType = SharedAssetSubType.Commodity;
            }
            else
            {
                var baseAsset = assets.SingleOrDefault(x => x.ShortName == s.BaseAsset);
                if (baseAsset != null)
                {
                    if (baseAsset.FullName.Contains("xStock"))
                    {
                        result.BaseAssetType = SharedAssetType.TradFi;
                        result.BaseAssetSubType = SharedAssetSubType.Equity;
                    }
                    else
                    {
                        result.BaseAssetType = SharedAssetType.Crypto;
                        if (LibraryHelpers.IsStableCoin(result.BaseAsset))
                            result.BaseAssetSubType = SharedAssetSubType.StableCoin;
                    }
                }
            }

            result.QuoteAssetType = SharedAssetType.Crypto;
            if (LibraryHelpers.IsStableCoin(result.QuoteAsset))
                result.QuoteAssetSubType = SharedAssetSubType.StableCoin;

            return result;
        }

        async Task<ExchangeCallResult<SharedSymbol[]>> ISpotSymbolRestClient.GetSpotSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicId, EnvironmentName, null, baseAsset));
        }

        async Task<ExchangeCallResult<bool>> ISpotSymbolRestClient.SupportsSpotSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode != TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Only Spot symbols allowed");

            if (!ExchangeSymbolCache.HasCached(_topicId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicId, EnvironmentName, null, symbol));
        }

        async Task<ExchangeCallResult<bool>> ISpotSymbolRestClient.SupportsSpotSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicId, EnvironmentName, null))
            {
                var symbols = await ((ISpotSymbolRestClient)this).GetSpotSymbolsAsync(new GetSymbolsRequest()).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicId, EnvironmentName, null, symbolName));
        }
        #endregion

        #region Ticker client

        GetSpotTickerOptions ISpotTickerRestClient.GetSpotTickerOptions { get; } = new GetSpotTickerOptions(_exchangeName);
        async Task<HttpResult<SharedSpotTicker>> ISpotTickerRestClient.GetSpotTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(new[] { request.Symbol!.GetSymbol(FormatSymbol) }, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker>(result);

            var ticker = result.Data.Single();
            return HttpResult.Ok(result, new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, ticker.Symbol), ticker.Symbol, ticker.LastPrice, ticker.HighPrice, ticker.LowPrice, ticker.Volume, ticker.OpenPrice == 0 ? null : Math.Round(ticker.LastPrice / ticker.OpenPrice * 100 - 100, 2))
            {
                QuoteVolume = ticker.Value
            });
        }

        GetSpotTickersOptions ISpotTickerRestClient.GetSpotTickersOptions { get; } = new GetSpotTickersOptions(_exchangeName);
        async Task<HttpResult<SharedSpotTicker[]>> ISpotTickerRestClient.GetSpotTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTicker[]>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedSpotTicker[]>(result);

            return HttpResult.Ok(result, result.Data.Select(x => new SharedSpotTicker(ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, x.Symbol), x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice, x.Volume, x.OpenPrice == 0 ? null : Math.Round(x.LastPrice / x.OpenPrice * 100 - 100, 2))
            {
                QuoteVolume = x.Value
            }).ToArray());
        }

        #endregion

        #region Book Ticker client

        GetBookTickerOptions IBookTickerRestClient.GetBookTickerOptions { get; } = new GetBookTickerOptions(_exchangeName, false);
        async Task<HttpResult<SharedBookTicker>> IBookTickerRestClient.GetBookTickerAsync(GetBookTickerRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetBookTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBookTicker>(Exchange, validationError);

            var resultTicker = await ExchangeData.GetOrderBookAsync(request.Symbol!.GetSymbol(FormatSymbol), 5, ct: ct).ConfigureAwait(false);
            if (!resultTicker.Success)
                return HttpResult.Fail<SharedBookTicker>(resultTicker);

            return HttpResult.Ok(resultTicker, new SharedBookTicker(
                ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, resultTicker.Data.Symbol),
                resultTicker.Data.Symbol,
                resultTicker.Data.Data.Asks[0].Price,
                resultTicker.Data.Data.Asks[0].Quantity,
                resultTicker.Data.Data.Bids[0].Price,
                resultTicker.Data.Data.Bids[0].Quantity));
        }

        #endregion

        #region Recent Trades

        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(_exchangeName, 1000, false);

        async Task<HttpResult<SharedTrade[]>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetRecentTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedTrade[]>(Exchange, validationError);

            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var result = await ExchangeData.GetTradeHistoryAsync(
                symbol,
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedTrade[]>(result);

            return HttpResult.Ok(result, result.Data.Select(x => 
            new SharedTrade(request.Symbol, symbol, x.Quantity, x.Price, x.Timestamp)
            {
                Side = x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell
            }).ToArray());
        }

        #endregion

        #region Balance client
        GetBalancesOptions IBalanceRestClient.GetBalancesOptions { get; } = new GetBalancesOptions(_exchangeName, AccountTypeFilter.Spot, AccountTypeFilter.Margin);

        async Task<HttpResult<SharedBalance[]>> IBalanceRestClient.GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetBalancesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBalance[]>(Exchange, validationError);

            if (request.AccountType == SharedAccountType.Spot || request.AccountType == null)
            {
                var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
                if (!result.Success)
                return HttpResult.Fail<SharedBalance[]>(result);

                return HttpResult.Ok(result, result.Data.Select(x => 
                    new SharedBalance(
                        SupportedTradingModes, 
                        x.Asset,
                        x.Available, 
                        x.Available + x.Frozen)).ToArray());
            }
            else
            {
                var result = await Account.GetMarginBalancesAsync(ct: ct).ConfigureAwait(false);
                if (!result.Success)
                return HttpResult.Fail<SharedBalance[]>(result);

                var resultList = new List<SharedBalance>();
                foreach(var item in result.Data)
                {
                    resultList.Add(
                        new SharedBalance(
                            SupportedTradingModes,
                            item.BaseAsset, 
                            item.Available.BaseAsset,
                            item.Available.BaseAsset + item.Frozen.BaseAsset)
                        { IsolatedMarginSymbol = item.MarginAccount });
                    resultList.Add(
                        new SharedBalance(
                            SupportedTradingModes,
                            item.QuoteAsset,
                            item.Available.QuoteAsset,
                            item.Available.QuoteAsset + item.Frozen.QuoteAsset)
                        { IsolatedMarginSymbol = item.MarginAccount });
                }

                return HttpResult.Ok(result, resultList.ToArray());
            }
        }

        #endregion

        #region Spot Order client

        PlaceSpotOrderOptions ISpotOrderRestClient.PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(_exchangeName);

        SharedFeeDeductionType ISpotOrderRestClient.SpotFeeDeductionType => SharedFeeDeductionType.DeductFromOutput;
        SharedFeeAssetType ISpotOrderRestClient.SpotFeeAssetType => SharedFeeAssetType.OutputAsset;
        SharedOrderType[] ISpotOrderRestClient.SpotSupportedOrderTypes { get; } = new[] { SharedOrderType.Limit, SharedOrderType.Market, SharedOrderType.LimitMaker };
        SharedTimeInForce[] ISpotOrderRestClient.SpotSupportedTimeInForce { get; } = new[] { SharedTimeInForce.GoodTillCanceled, SharedTimeInForce.ImmediateOrCancel, SharedTimeInForce.FillOrKill };

        SharedQuantitySupport ISpotOrderRestClient.SpotSupportedOrderQuantity { get; } = new SharedQuantitySupport(
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAsset,
                SharedQuantityType.BaseAndQuoteAsset,
                SharedQuantityType.BaseAndQuoteAsset);

        string ISpotOrderRestClient.GenerateClientOrderId() => ExchangeHelpers.RandomString(20);

        async Task<HttpResult<SharedId>> ISpotOrderRestClient.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.PlaceSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var result = await Trading.PlaceOrderAsync(
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

        GetSpotOrderOptions ISpotOrderRestClient.GetSpotOrderOptions { get; } = new GetSpotOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotOrder>> ISpotOrderRestClient.GetSpotOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedSpotOrder>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await Trading.GetOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotOrder>(orders);

            return HttpResult.Ok(orders, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, orders.Data.Symbol),
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
                Fee = orders.Data.FeeBaseAsset > 0 ? orders.Data.FeeBaseAsset : orders.Data.FeeQuoteAsset,
                FeeAsset = orders.Data.FeeBaseAsset > 0 ? request.Symbol!.BaseAsset : orders.Data.FeeQuoteAsset > 0 ? request.Symbol!.QuoteAsset : null,
                TimeInForce = ParseTimeInForce(orders.Data.OrderType)
            });
        }

        GetOpenSpotOrdersOptions ISpotOrderRestClient.GetOpenSpotOrdersOptions { get; } = new GetOpenSpotOrdersOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetOpenSpotOrdersAsync(GetOpenOrdersRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOpenSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            string? symbol = request.Symbol?.GetSymbol(FormatSymbol);

            var orders = await Trading.GetOpenOrdersAsync(AccountType.Spot, symbol, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotOrder[]>(orders);

            return HttpResult.Ok(orders, orders.Data.Items.Select(x => new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, x.Symbol), 
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
                Fee = x.FeeBaseAsset > 0 ? x.FeeBaseAsset : x.FeeQuoteAsset,
                FeeAsset = x.FeeBaseAsset > 0 ? request.Symbol?.BaseAsset : x.FeeQuoteAsset > 0 ? request.Symbol?.QuoteAsset : null,
                TimeInForce = ParseTimeInForce(x.OrderType)
            }).ToArray());
        }

        GetSpotClosedOrdersOptions ISpotOrderRestClient.GetClosedSpotOrdersOptions { get; } = new GetSpotClosedOrdersOptions(_exchangeName, false, true, false, 500);
        async Task<HttpResult<SharedSpotOrder[]>> ISpotOrderRestClient.GetClosedSpotOrdersAsync(GetClosedOrdersRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetClosedSpotOrdersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder[]>(Exchange, validationError);

            // Determine page token
            var direction = DataDirection.Descending;
            var limit = request.Limit ?? 500;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await Trading.GetClosedOrdersAsync(
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
                        ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, x.Symbol), 
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
                            Fee = x.FeeBaseAsset > 0 ? x.FeeBaseAsset : x.FeeQuoteAsset,
                            FeeAsset = x.FeeBaseAsset > 0 ? request.Symbol?.BaseAsset : x.FeeQuoteAsset > 0 ? request.Symbol?.QuoteAsset : null,
                            TimeInForce = ParseTimeInForce(x.OrderType)
                        })
                    .ToArray(), nextPageRequest);
        }

        GetSpotOrderTradesOptions ISpotOrderRestClient.GetSpotOrderTradesOptions { get; } = new GetSpotOrderTradesOptions(_exchangeName, true);
        async Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotOrderTradesAsync(GetOrderTradesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotOrderTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, ArgumentError.Invalid(nameof(GetOrderTradesRequest.OrderId), "Invalid order id"));

            var orders = await Trading.GetOrderTradesAsync(request.Symbol!.GetSymbol(FormatSymbol), AccountType.Spot, orderId: orderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedUserTrade[]>(orders);

            return HttpResult.Ok(orders, orders.Data.Items.Select(x => new SharedUserTrade(
                ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, x.Symbol), 
                x.Symbol,
                x.OrderId.ToString(),
                x.Id.ToString(),
                x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                x.Quantity,
                x.Price,
                x.CreateTime)
            {
                ClientOrderId = x.ClientOrderId,
                Role = x.Role == TransactionRole.Maker ? SharedRole.Maker : SharedRole.Taker,
                Fee = x.Fee,
                FeeAsset = x.FeeAsset,
            }).ToArray());
        }

        GetSpotUserTradesOptions ISpotOrderRestClient.GetSpotUserTradesOptions { get; } = new GetSpotUserTradesOptions(_exchangeName, false, true, true, 500);
        async Task<HttpResult<SharedUserTrade[]>> ISpotOrderRestClient.GetSpotUserTradesAsync(GetUserTradesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotUserTradesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedUserTrade[]>(Exchange, validationError);

            // Determine page token
            int limit = request.Limit ?? 500;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await Trading.GetUserTradesAsync(
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
                        ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, x.Symbol), 
                        x.Symbol,
                        x.OrderId.ToString(),
                        x.Id.ToString(),
                        x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell,
                        x.Quantity,
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
                
        CancelSpotOrderOptions ISpotOrderRestClient.CancelSpotOrderOptions { get; } = new CancelSpotOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> ISpotOrderRestClient.CancelSpotOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            if (!long.TryParse(request.OrderId, out var orderId))
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid(nameof(CancelOrderRequest.OrderId), "Invalid order id"));

            var order = await Trading.CancelOrderAsync(request.Symbol!.GetSymbol(FormatSymbol), AccountType.Spot, orderId, ct: ct).ConfigureAwait(false);
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

        GetSpotOrderByClientOrderIdOptions ISpotOrderClientIdRestClient.GetSpotOrderByClientOrderIdOptions { get; } = new GetSpotOrderByClientOrderIdOptions(_exchangeName, true);
        async Task<HttpResult<SharedSpotOrder>> ISpotOrderClientIdRestClient.GetSpotOrderByClientOrderIdAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, validationError);

            var order = await Trading.GetOpenOrdersAsync(AccountType.Spot, clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedSpotOrder>(order);

            var orderData = order.Data.Items.FirstOrDefault();
            if (orderData == null)
            {
                order = await Trading.GetClosedOrdersAsync(AccountType.Spot, clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
                if (!order.Success)
                return HttpResult.Fail<SharedSpotOrder>(order);

                orderData = order.Data.Items.FirstOrDefault();                
            }

            if (orderData == null)
                return HttpResult.Fail<SharedSpotOrder>(Exchange, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

            return HttpResult.Ok(order, new SharedSpotOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, orderData.Symbol),
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
                Fee = orderData.FeeBaseAsset > 0 ? orderData.FeeBaseAsset : orderData.FeeQuoteAsset,
                FeeAsset = orderData.FeeBaseAsset > 0 ? request.Symbol!.BaseAsset : orderData.FeeQuoteAsset > 0 ? request.Symbol!.QuoteAsset : null,
                TimeInForce = ParseTimeInForce(orderData.OrderType)
            });
        }

        CancelSpotOrderByClientOrderIdOptions ISpotOrderClientIdRestClient.CancelSpotOrderByClientOrderIdOptions { get; } = new CancelSpotOrderByClientOrderIdOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> ISpotOrderClientIdRestClient.CancelSpotOrderByClientOrderIdAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelSpotOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var order = await Trading.CancelOrdersByClientOrderIdAsync(request.Symbol!.GetSymbol(FormatSymbol), AccountType.Spot, clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(order.Data.FirstOrDefault()?.Id.ToString() ?? request.OrderId));
        }
        #endregion

        #region Asset client

        GetAssetOptions IAssetsRestClient.GetAssetOptions { get; } = new GetAssetOptions(_exchangeName, false);
        async Task<HttpResult<SharedAsset>> IAssetsRestClient.GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetAssetOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset>(Exchange, validationError);

            var asset = await Account.GetDepositWithdrawalConfigAsync(request.Asset, ct: ct).ConfigureAwait(false);
            if (!asset.Success)
                return HttpResult.Fail<SharedAsset>(asset);

            return HttpResult.Ok(asset, new SharedAsset(asset.Data.Asset.Asset)
            {
                Networks = asset.Data.Networks.Select(x => new SharedAssetNetwork(x.Network)
                {
                    DepositEnabled = x.DepositEnabled,
                    WithdrawEnabled = x.WithdrawEnabled,
                    WithdrawFee = x.WithdrawalFee,
                    MinWithdrawQuantity = x.MinWithdrawQuantity,
                    MinConfirmations = x.SafeConfirmations
                }).ToArray()
            });
        }

        GetAssetsOptions IAssetsRestClient.GetAssetsOptions { get; } = new GetAssetsOptions(_exchangeName, false);
        async Task<HttpResult<SharedAsset[]>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetAssetsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset[]>(Exchange, validationError);

            var assets = ExchangeData.GetAssetsAsync(ct: ct);
            var assetConfigs = Account.GetAllDepositWithdrawalConfigsAsync(ct: ct);
            await Task.WhenAll(assets, assetConfigs).ConfigureAwait(false);
            if (!assets.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assets.Result);
            if (!assetConfigs.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assetConfigs.Result);

            return HttpResult.Ok(assets.Result, assets.Result.Data.Select(x =>
            {
                var config = assetConfigs.Result.Data.SingleOrDefault(y => y.Asset.Asset.Equals(x.ShortName));
                return new SharedAsset(x.ShortName)
                {
                    FullName = x.FullName,
                    Networks = config?.Networks.Select(x => new SharedAssetNetwork(x.Network)
                    {
                        DepositEnabled = x.DepositEnabled,
                        WithdrawEnabled = x.WithdrawEnabled,
                        WithdrawFee = x.WithdrawalFee,
                        MinWithdrawQuantity = x.MinWithdrawQuantity,
                        MinConfirmations = x.SafeConfirmations
                    }).ToArray() ?? []
                };
            }).ToArray());
        }

        #endregion

        #region Deposit client
        GetDepositAddressesOptions IDepositRestClient.GetDepositAddressesOptions { get; } = new GetDepositAddressesOptions(_exchangeName, true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(GetDepositAddressesRequest.Network), typeof(string), "The network for the deposit address", "ERC20")
            }
        };

        async Task<HttpResult<SharedDepositAddress[]>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetDepositAddressesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDepositAddress[]>(Exchange, validationError);

            var depositAddresses = await Account.GetDepositAddressAsync(request.Asset, request.Network!).ConfigureAwait(false);
            if (!depositAddresses.Success)
                return HttpResult.Fail<SharedDepositAddress[]>(depositAddresses);

            return HttpResult.Ok(depositAddresses, new[] { new SharedDepositAddress(request.Asset, depositAddresses.Data.Address)
            {
                Network = request.Network,
                TagOrMemo = depositAddresses.Data.Memo
            }
            });
        }

        GetDepositsOptions IDepositRestClient.GetDepositsOptions { get; } = new GetDepositsOptions(_exchangeName, false, true, false, 100)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(GetWithdrawalsRequest.Asset), typeof(string), "Asset the deposits should be retrieved for", "ETH")
            }
        };
        async Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetDepositsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDeposit[]>(Exchange, validationError);

            // Determine page token
            int limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await Account.GetDepositHistoryAsync(
                request.Asset!,
                pageSize: limit,
                page: pageParams.Page,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedDeposit[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromPage(pageParams),
                    result.Data.Items.Length,
                    result.Data.Items.Select(x => x.CreateTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Items, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                    .Select(x => 
                        new SharedDeposit(
                            x.Asset,
                            x.Quantity, 
                            x.Status == DepositStatus.Finished,
                            x.CreateTime,
                            ParseTransferStatus(x.Status))
                        {
                            Id = x.Id.ToString(),
                            Confirmations = x.Confirmations,
                            Network = x.Network,
                            TransactionId = x.TransactionId
                        })
                    .ToArray(), nextPageRequest);
        }

        private SharedTransferStatus ParseTransferStatus(DepositStatus status)
        {
            if (status == DepositStatus.Finished)
                return SharedTransferStatus.Completed;
            if (status == DepositStatus.Processing || status == DepositStatus.Confirming)
                return SharedTransferStatus.InProgress;
            if (status == DepositStatus.Exception || status == DepositStatus.TooSmall)
                return SharedTransferStatus.Failed;

            return SharedTransferStatus.Unknown;
        }

        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(_exchangeName, new [] { 5, 10, 20, 50 }, false);
        async Task<HttpResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetOrderBookOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                limit: request.Limit ?? 20,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOrderBook>(result);

            return HttpResult.Ok(result, new SharedOrderBook(result.Data.Data.Asks, result.Data.Data.Bids));
        }
        #endregion

        #region Withdrawal client

        GetWithdrawalsOptions IWithdrawalRestClient.GetWithdrawalsOptions { get; } = new GetWithdrawalsOptions(_exchangeName, false, true, false, 100);
        async Task<HttpResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = SharedClient.GetWithdrawalsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedWithdrawal[]>(Exchange, validationError);

            // Determine page token
            int limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await Account.GetWithdrawalHistoryAsync(
                request.Asset,
                pageSize: limit,
                page: pageParams.Page,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedWithdrawal[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromPage(pageParams),
                    result.Data.Items.Length,
                    result.Data.Items.Select(x => x.CreateTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Items, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                    .Select(x => 
                        new SharedWithdrawal(
                            x.Asset,
                            x.ToAddress,
                            x.Quantity,
                            x.Status == WithdrawStatusV2.Finished,
                            x.CreateTime,
                            GetWithdrawalStatus(x))
                        {
                            Id = x.Id.ToString(),
                            Confirmations = x.Confirmations,
                            Network = x.Network,
                            Tag = x.Memo,
                            TransactionId = x.TransactionId,
                            Fee = x.Fee
                        })
                    .ToArray(), nextPageRequest);
        }

        private SharedTransferStatus GetWithdrawalStatus(CoinExWithdrawal x)
        {
            if (x.Status == WithdrawStatusV2.Canceled || x.Status == WithdrawStatusV2.Failed)
                return SharedTransferStatus.Failed;

            if (x.Status == WithdrawStatusV2.Finished)
                return SharedTransferStatus.Completed;

            if (x.Status == WithdrawStatusV2.Audited
                || x.Status == WithdrawStatusV2.AuditRequired
                || x.Status == WithdrawStatusV2.Confirming
                || x.Status == WithdrawStatusV2.Processing
                || x.Status == WithdrawStatusV2.Created)
            {
                return SharedTransferStatus.InProgress;
            }

            return SharedTransferStatus.Unknown;
        }
        #endregion

        #region Withdraw client

        WithdrawOptions IWithdrawRestClient.WithdrawOptions { get; } = new WithdrawOptions(_exchangeName);

        async Task<HttpResult<SharedId>> IWithdrawRestClient.WithdrawAsync(WithdrawRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.WithdrawOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            // Get data
            var withdrawal = await Account.WithdrawAsync(
                request.Asset,
                toAddress: request.Address,
                quantity: request.Quantity,
                network: request.Network,
                method: MovementMethod.OnChain,
                memo: request.AddressTag,
                ct: ct).ConfigureAwait(false);
            if (!withdrawal.Success)
                return HttpResult.Fail<SharedId>(withdrawal);

            return HttpResult.Ok(withdrawal, new SharedId(withdrawal.Data.Id.ToString()));
        }

        #endregion

        #region Fee Client
        GetFeeOptions IFeeRestClient.GetFeeOptions { get; } = new GetFeeOptions(_exchangeName, true);

        async Task<HttpResult<SharedFee>> IFeeRestClient.GetFeesAsync(GetFeeRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetFeeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFee>(Exchange, validationError);

            // Get data
            var result = await Account.GetTradingFeesAsync(request.Symbol!.GetSymbol(FormatSymbol), AccountType.Spot, ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFee>(result);

            // Return
            return HttpResult.Ok(result, new SharedFee(result.Data.MakerFeeRate * 100, result.Data.TakerFeeRate * 100));
        }
        #endregion

        #region Spot Trigger Order Client
        PlaceSpotTriggerOrderOptions ISpotTriggerOrderRestClient.PlaceSpotTriggerOrderOptions { get; } = new PlaceSpotTriggerOrderOptions(_exchangeName, false);

        async Task<HttpResult<SharedId>> ISpotTriggerOrderRestClient.PlaceSpotTriggerOrderAsync(PlaceSpotTriggerOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.PlaceSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var clientOrderId = request.ClientOrderId ?? ExchangeHelpers.RandomString(32);
            var result = await Trading.PlaceStopOrderAsync(
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

        GetSpotTriggerOrderOptions ISpotTriggerOrderRestClient.GetSpotTriggerOrderOptions { get; } = new GetSpotTriggerOrderOptions(_exchangeName, true)
        {
            RequestNotes = "Only pending trigger orders can be requested, executed trigger orders are not available in the API"
        };
        async Task<HttpResult<SharedSpotTriggerOrder>> ISpotTriggerOrderRestClient.GetSpotTriggerOrderAsync(GetOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.GetSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotTriggerOrder>(Exchange, validationError);

            var status = SharedTriggerOrderStatus.Active;
            var orders = await Trading.GetOpenStopOrdersAsync(AccountType.Spot, clientOrderId: request.OrderId, ct: ct).ConfigureAwait(false);
            if (!orders.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(orders);

            CoinExStopOrder order;
            if (orders.Data.Items.Any())
            {
                order = orders.Data.Items.Single();
            }
            else
            {
                orders = await Trading.GetClosedStopOrdersAsync(AccountType.Spot, request.Symbol!.GetSymbol(FormatSymbol), pageSize: 1000, ct: ct).ConfigureAwait(false);
                if (!orders.Success)
                return HttpResult.Fail<SharedSpotTriggerOrder>(orders);

                order = orders.Data.Items.SingleOrDefault(x => x.ClientOrderId == request.OrderId)!;
                if (order == null)
                    return HttpResult.Fail<SharedSpotTriggerOrder>(orders, new ServerError(new ErrorInfo(ErrorType.UnknownOrder, "Order not found")));

                status = SharedTriggerOrderStatus.Filled;
            }

            return HttpResult.Ok(orders, new SharedSpotTriggerOrder(
                ExchangeSymbolCache.ParseSymbol(_topicId, EnvironmentName, null, order.Symbol),
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

        CancelSpotTriggerOrderOptions ISpotTriggerOrderRestClient.CancelSpotTriggerOrderOptions { get; } = new CancelSpotTriggerOrderOptions(_exchangeName, true);
        async Task<HttpResult<SharedId>> ISpotTriggerOrderRestClient.CancelSpotTriggerOrderAsync(CancelOrderRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.CancelSpotTriggerOrderOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var order = await Trading.CancelStopOrdersByClientOrderIdAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                AccountType.Spot,
                request.OrderId,
                ct: ct).ConfigureAwait(false);
            if (!order.Success)
                return HttpResult.Fail<SharedId>(order);

            return HttpResult.Ok(order, new SharedId(request.OrderId));
        }

        #endregion

        #region Transfer client

        TransferOptions ITransferRestClient.TransferOptions { get; } = new TransferOptions(_exchangeName, [
            SharedAccountType.Spot,
            SharedAccountType.PerpetualLinearFutures,
            SharedAccountType.PerpetualInverseFutures,
            SharedAccountType.DeliveryLinearFutures,
            SharedAccountType.DeliveryInverseFutures,
            SharedAccountType.CrossMargin,
            SharedAccountType.IsolatedMargin]);
        async Task<HttpResult<SharedId>> ITransferRestClient.TransferAsync(TransferRequest request, CancellationToken ct)
        {
            var validationError = SharedClient.TransferOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            var fromType = GetTransferType(request.FromAccountType);
            var toType = GetTransferType(request.ToAccountType);
            if (fromType == null || toType == null)
                return HttpResult.Fail<SharedId>(Exchange, ArgumentError.Invalid("To/From AccountType", "invalid to/from account combination"));

            // Get data
            var transfer = await Account.TransferAsync(
                request.Asset,
                fromType.Value,
                toType.Value,
                request.Quantity,
                fromType == AccountType.Margin ? request.FromSymbol : request.ToSymbol,
                ct: ct).ConfigureAwait(false);
            if (!transfer.Success)
                return HttpResult.Fail<SharedId>(transfer);

            return HttpResult.Ok(transfer, new SharedId(""));
        }

        private AccountType? GetTransferType(SharedAccountType type)
        {
            if (type == SharedAccountType.Spot) return AccountType.Spot;
            if (type.IsMarginAccount()) return AccountType.Margin;
            if (type.IsFuturesAccount()) return AccountType.Futures;
            return null;
        }

        #endregion
    }
}
