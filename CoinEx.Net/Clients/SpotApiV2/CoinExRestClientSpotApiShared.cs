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
using CryptoExchange.Net.SharedApis.Models.FilterOptions;

namespace CoinEx.Net.Clients.SpotApiV2
{
    internal partial class CoinExRestClientSpotApi : ICoinExRestClientSpotApiShared
    {
        public string Exchange => CoinExExchange.ExchangeName;

        #region Kline client

        GetKlinesOptions IKlineRestClient.GetKlinesOptions { get; } = new GetKlinesOptions(false, false)
        {
            MaxRequestDataPoints = 1000
        };

        async Task<ExchangeWebResult<IEnumerable<SharedKline>>> IKlineRestClient.GetKlinesAsync(GetKlinesRequest request, INextPageToken? pageToken, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;
            if (!Enum.IsDefined(typeof(Enums.KlineInterval), interval))
                return new ExchangeWebResult<IEnumerable<SharedKline>>(Exchange, new ArgumentError("Interval not supported"));

            var validationError = ((IKlineRestClient)this).GetKlinesOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedKline>>(Exchange, validationError);

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

        #endregion

        #region Spot Symbol client

        EndpointOptions ISpotSymbolRestClient.GetSpotSymbolsOptions { get; } = new EndpointOptions(false);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotSymbol>>> ISpotSymbolRestClient.GetSpotSymbolsAsync(ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ISpotSymbolRestClient)this).GetSpotSymbolsOptions.ValidateRequest(Exchange, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotSymbol>>(Exchange, validationError);

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

        #endregion

        #region Ticker client

        EndpointOptions<GetTickerRequest> ITickerRestClient.GetTickerOptions { get; } = new EndpointOptions<GetTickerRequest>(false);
        async Task<ExchangeWebResult<SharedTicker>> ITickerRestClient.GetTickerAsync(GetTickerRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ITickerRestClient)this).GetTickerOptions.ValidateRequest(Exchange, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<SharedTicker>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(new[] { request.GetSymbol(FormatSymbol) }, ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedTicker>(Exchange, default);

            var ticker = result.Data.Single();
            return result.AsExchangeResult(Exchange, new SharedTicker(ticker.Symbol, ticker.LastPrice, ticker.HighPrice, ticker.LowPrice, ticker.Volume));
        }

        EndpointOptions ITickerRestClient.GetTickersOptions { get; } = new EndpointOptions(false);
        async Task<ExchangeWebResult<IEnumerable<SharedTicker>>> ITickerRestClient.GetTickersAsync(ApiType? apiType, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ITickerRestClient)this).GetTickerOptions.ValidateRequest(Exchange, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedTicker>>(Exchange, validationError);

            var result = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTicker>>(Exchange, default);

            return result.AsExchangeResult<IEnumerable<SharedTicker>>(Exchange, result.Data.Select(x => new SharedTicker(x.Symbol, x.LastPrice, x.HighPrice, x.LowPrice, x.Volume)));
        }

        #endregion

        #region Recent Trades

        GetRecentTradesOptions IRecentTradeRestClient.GetRecentTradesOptions { get; } = new GetRecentTradesOptions(1000, false);

        async Task<ExchangeWebResult<IEnumerable<SharedTrade>>> IRecentTradeRestClient.GetRecentTradesAsync(GetRecentTradesRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((IRecentTradeRestClient)this).GetRecentTradesOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedTrade>>(Exchange, validationError);

            var result = await ExchangeData.GetTradeHistoryAsync(
                request.GetSymbol(FormatSymbol),
                limit: request.Limit,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedTrade>>(Exchange, default);

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedTrade(x.Quantity, x.Price, x.Timestamp)));
        }

        #endregion

        #region Balance client
        EndpointOptions IBalanceRestClient.GetBalancesOptions { get; } = new EndpointOptions(true);

        async Task<ExchangeWebResult<IEnumerable<SharedBalance>>> IBalanceRestClient.GetBalancesAsync(ApiType? apiType, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((IBalanceRestClient)this).GetBalancesOptions.ValidateRequest(Exchange, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedBalance>>(Exchange, validationError);

            var result = await Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<IEnumerable<SharedBalance>>(Exchange, default);

            return result.AsExchangeResult(Exchange, result.Data.Select(x => new SharedBalance(x.Asset, x.Available, x.Available + x.Frozen)));
        }

        #endregion

        #region Spot Order client

        PlaceSpotOrderOptions ISpotOrderRestClient.PlaceSpotOrderOptions { get; } = new PlaceSpotOrderOptions(
            new[]
            {
                SharedOrderType.Limit,
                SharedOrderType.Market,
                SharedOrderType.LimitMaker
            },
            new[]
            {
                SharedTimeInForce.GoodTillCanceled,
                SharedTimeInForce.ImmediateOrCancel,
                SharedTimeInForce.FillOrKill
            },
            new SharedQuantitySupport(
                SharedQuantityType.BaseAssetQuantity,
                SharedQuantityType.BaseAssetQuantity,
                SharedQuantityType.Both,
                SharedQuantityType.Both));

        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.PlaceSpotOrderAsync(PlaceSpotOrderRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).PlaceSpotOrderOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

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

        EndpointOptions<GetOrderRequest> ISpotOrderRestClient.GetOrderOptions { get; } = new EndpointOptions<GetOrderRequest>(true);
        async Task<ExchangeWebResult<SharedSpotOrder>> ISpotOrderRestClient.GetOrderAsync(GetOrderRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetOrderOptions.ValidateRequest(Exchange, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<SharedSpotOrder>(Exchange, validationError);

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

        EndpointOptions<GetSpotOpenOrdersRequest> ISpotOrderRestClient.GetOpenOrdersOptions { get; } = new EndpointOptions<GetSpotOpenOrdersRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetOpenOrdersAsync(GetSpotOpenOrdersRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetOpenOrdersOptions.ValidateRequest(Exchange, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotOrder>>(Exchange, validationError);

            string? symbol = null;
            if (request.BaseAsset != null && request.QuoteAsset != null)
                symbol = FormatSymbol(request.BaseAsset, request.QuoteAsset, ApiType.Spot);

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

        PaginatedEndpointOptions<GetSpotClosedOrdersRequest> ISpotOrderRestClient.GetClosedOrdersOptions { get; } = new PaginatedEndpointOptions<GetSpotClosedOrdersRequest>(true, true);
        async Task<ExchangeWebResult<IEnumerable<SharedSpotOrder>>> ISpotOrderRestClient.GetClosedOrdersAsync(GetSpotClosedOrdersRequest request, INextPageToken? pageToken, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetClosedOrdersOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedSpotOrder>>(Exchange, validationError);

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

        EndpointOptions<GetOrderTradesRequest> ISpotOrderRestClient.GetOrderTradesOptions { get; } = new EndpointOptions<GetOrderTradesRequest>(true);
        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetOrderTradesAsync(GetOrderTradesRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetOrderTradesOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, validationError);

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

        PaginatedEndpointOptions<GetUserTradesRequest> ISpotOrderRestClient.GetUserTradesOptions { get; } = new PaginatedEndpointOptions<GetUserTradesRequest>(true, true);
        async Task<ExchangeWebResult<IEnumerable<SharedUserTrade>>> ISpotOrderRestClient.GetUserTradesAsync(GetUserTradesRequest request, INextPageToken? pageToken, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).GetUserTradesOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedUserTrade>>(Exchange, validationError);

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
                
        EndpointOptions<CancelOrderRequest> ISpotOrderRestClient.CancelOrderOptions { get; } = new EndpointOptions<CancelOrderRequest>(true);
        async Task<ExchangeWebResult<SharedId>> ISpotOrderRestClient.CancelOrderAsync(CancelOrderRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((ISpotOrderRestClient)this).CancelOrderOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

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

        #endregion

        #region Asset client
        EndpointOptions IAssetRestClient.GetAssetsOptions { get; } = new EndpointOptions(false);

        async Task<ExchangeWebResult<IEnumerable<SharedAsset>>> IAssetRestClient.GetAssetsAsync(ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((IAssetRestClient)this).GetAssetsOptions.ValidateRequest(Exchange, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedAsset>>(Exchange, validationError);

            var assets = await ExchangeData.GetAssetsAsync(ct: ct).ConfigureAwait(false);
            if (!assets)
                return assets.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, default);

            return assets.AsExchangeResult<IEnumerable<SharedAsset>>(Exchange, assets.Data.Select(x => new SharedAsset(x.ShortName)
            {
                FullName = x.FullName,
                Networks = x.Networks.Select(x => new SharedAssetNetwork(x.Name))
            }).ToList());
        }

        #endregion

        #region Deposit client
        EndpointOptions<GetDepositAddressesRequest> IDepositRestClient.GetDepositAddressesOptions { get; } = new EndpointOptions<GetDepositAddressesRequest>(true)
        {
            RequiredOptionalParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(GetDepositAddressesRequest.Network), typeof(string), "The network for the deposit address", "ETH")
            }
        };

        async Task<ExchangeWebResult<IEnumerable<SharedDepositAddress>>> IDepositRestClient.GetDepositAddressesAsync(GetDepositAddressesRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((IDepositRestClient)this).GetDepositAddressesOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedDepositAddress>>(Exchange, validationError);

            var depositAddresses = await Account.GetDepositAddressAsync(request.Asset, request.Network!).ConfigureAwait(false);
            if (!depositAddresses)
                return depositAddresses.AsExchangeResult<IEnumerable<SharedDepositAddress>>(Exchange, default);

            return depositAddresses.AsExchangeResult<IEnumerable<SharedDepositAddress>>(Exchange, new[] { new SharedDepositAddress(request.Asset, depositAddresses.Data.Address)
            {
                Network = request.Network,
                Tag = depositAddresses.Data.Memo
            }
            });
        }

        PaginatedEndpointOptions<GetDepositsRequest> IDepositRestClient.GetDepositsOptions { get; } = new PaginatedEndpointOptions<GetDepositsRequest>(true, true);
        async Task<ExchangeWebResult<IEnumerable<SharedDeposit>>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, INextPageToken? pageToken, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((IDepositRestClient)this).GetDepositsOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedDeposit>>(Exchange, validationError);

            // Determine page token
            int page = 1;
            int pageSize = request.Filter?.Limit ?? 500;
            if (pageToken is PageToken token)
            {
                page = token.Page;
                pageSize = token.PageSize;
            }

#warning time filter not available, asset required
            // Get data
            var deposits = await Account.GetDepositHistoryAsync(
                request.Asset,
                pageSize: request.Filter?.Limit ?? 100,
                page: page,
                ct: ct).ConfigureAwait(false);
            if (!deposits)
                return deposits.AsExchangeResult<IEnumerable<SharedDeposit>>(Exchange, default);

            // Get next token
            PageToken? nextToken = null;
            if (deposits.Data.HasNext == true)
                nextToken = new PageToken(page + 1, pageSize);

            return deposits.AsExchangeResult(Exchange, deposits.Data.Items.Select(x => new SharedDeposit(x.Asset, x.Quantity, x.Status == DepositStatus.Finished, x.CreateTime)
            {
                Confirmations = x.Confirmations,
                Network = x.Network,
                TransactionId = x.TransactionId
            }), nextToken);
        }

        #endregion

        #region Order Book client
        GetOrderBookOptions IOrderBookRestClient.GetOrderBookOptions { get; } = new GetOrderBookOptions(new [] { 5, 10, 20, 50 }, false);
        async Task<ExchangeWebResult<SharedOrderBook>> IOrderBookRestClient.GetOrderBookAsync(GetOrderBookRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((IOrderBookRestClient)this).GetOrderBookOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<SharedOrderBook>(Exchange, validationError);

            var result = await ExchangeData.GetOrderBookAsync(
                request.GetSymbol(FormatSymbol),
                limit: request.Limit ?? 20,
                ct: ct).ConfigureAwait(false);
            if (!result)
                return result.AsExchangeResult<SharedOrderBook>(Exchange, default);

            return result.AsExchangeResult(Exchange, new SharedOrderBook(result.Data.Data.Asks, result.Data.Data.Bids));
        }
        #endregion

        #region Withdrawal client

        PaginatedEndpointOptions<GetWithdrawalsRequest> IWithdrawalRestClient.GetWithdrawalsOptions { get; } = new PaginatedEndpointOptions<GetWithdrawalsRequest>(true, true);
        async Task<ExchangeWebResult<IEnumerable<SharedWithdrawal>>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, INextPageToken? pageToken, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((IWithdrawalRestClient)this).GetWithdrawalsOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<IEnumerable<SharedWithdrawal>>(Exchange, validationError);

            // Determine page token
            int page = 1;
            int pageSize = request.Filter?.Limit ?? 500;
            if (pageToken is PageToken token)
            {
                page = token.Page;
                pageSize = token.PageSize;
            }

#warning time filter not available, asset required
            // Get data
            var withdrawals = await Account.GetWithdrawalHistoryAsync(
                request.Asset,
                pageSize: request.Filter?.Limit ?? 100,
                page: page,
                ct: ct).ConfigureAwait(false);
            if (!withdrawals)
                return withdrawals.AsExchangeResult<IEnumerable<SharedWithdrawal>>(Exchange, default);

            // Get next token
            PageToken? nextToken = null;
            if (withdrawals.Data.HasNext == true)
                nextToken = new PageToken(page + 1, pageSize);

            return withdrawals.AsExchangeResult(Exchange, withdrawals.Data.Items.Select(x => new SharedWithdrawal(x.Asset, x.ToAddress, x.Quantity, x.Status == WithdrawStatusV2.Finished, x.CreateTime)
            {
                Confirmations = x.Confirmations,
                Network = x.Network,
                Tag = x.Memo,
                TransactionId = x.TransactionId,
                Fee = x.Fee
            }));
        }

        #endregion

        #region Withdraw client

        WithdrawOptions IWithdrawRestClient.WithdrawOptions { get; } = new WithdrawOptions();

        async Task<ExchangeWebResult<SharedId>> IWithdrawRestClient.WithdrawAsync(WithdrawRequest request, ExchangeParameters? exchangeParameters, CancellationToken ct)
        {
            var validationError = ((IWithdrawRestClient)this).WithdrawOptions.ValidateRequest(Exchange, request, exchangeParameters);
            if (validationError != null)
                return new ExchangeWebResult<SharedId>(Exchange, validationError);

            // Get data
            var withdrawal = await Account.WithdrawAsync(
                request.Asset,
                toAddress: request.Address,
                quantity: request.Quantity,
                network: request.Network,
                method: MovementMethod.OnChain,
                memo: request.AddressTag,
                ct: ct).ConfigureAwait(false);
            if (!withdrawal)
                return withdrawal.AsExchangeResult<SharedId>(Exchange, default);

            return withdrawal.AsExchangeResult(Exchange, new SharedId(withdrawal.Data.Id.ToString()));
        }

        #endregion
    }
}
