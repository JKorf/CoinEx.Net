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

        #region Get Klines

        async Task<ICallResult<SharedKline[]>> IGetKlines.GetKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetKlinesAsync(request, pageRequest, ct).ConfigureAwait(false);

        public GetKlinesOptions GetKlinesOptions { get; } = new GetKlinesOptions(_exchangeName, true, true, true, 1000, false,
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

        public async Task<HttpResult<SharedKline[]>> GetKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;

            var validationError = GetKlinesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedKline[]>(Exchange, validationError);

            var direction = request.Direction ?? DataDirection.Ascending;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 1000;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, false);

            var result = await _api.ExchangeData.GetKlinesAsync(
                symbol,
                interval,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: pageParams.Limit,
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
                            new SharedOrderQuantity(x.Volume, x.Value)))
                   .ToArray(), nextPageRequest);
        }

        #endregion



        #region Get Mark Price Klines

        async Task<ICallResult<SharedFuturesKline[]>> IGetMarkPriceKlines.GetMarkPriceKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetMarkPriceKlinesAsync(request, pageRequest, ct).ConfigureAwait(false);

        public GetMarkPriceKlinesOptions GetMarkPriceKlinesOptions { get; } = new GetMarkPriceKlinesOptions(_exchangeName, true, true, true, 1000, false);
        public async Task<HttpResult<SharedFuturesKline[]>> GetMarkPriceKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;

            var validationError = GetKlinesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesKline[]>(Exchange, validationError);

            var direction = request.Direction ?? DataDirection.Ascending;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 1000;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, false);

            var result = await _api.ExchangeData.GetKlinesAsync(
                symbol,
                interval,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: pageParams.Limit,
                priceType: PriceType.MarkPrice,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesKline[]>(result);

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
                        new SharedFuturesKline(request.Symbol, symbol, x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice))
                   .ToArray(), nextPageRequest);
        }

        #endregion



        #region Get Index Price Klines

        async Task<ICallResult<SharedFuturesKline[]>> IGetIndexPriceKlines.GetIndexPriceKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetIndexPriceKlinesAsync(request, pageRequest, ct).ConfigureAwait(false);

        public GetIndexPriceKlinesOptions GetIndexPriceKlinesOptions { get; } = new GetIndexPriceKlinesOptions(_exchangeName, true, true, true, 1000, false);
        public async Task<HttpResult<SharedFuturesKline[]>> GetIndexPriceKlinesAsync(GetKlinesRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var interval = (Enums.KlineInterval)request.Interval;

            var validationError = GetKlinesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesKline[]>(Exchange, validationError);

            var direction = request.Direction ?? DataDirection.Ascending;
            var symbol = request.Symbol!.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 1000;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, false);

            var result = await _api.ExchangeData.GetKlinesAsync(
                symbol,
                interval,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                limit: pageParams.Limit,
                priceType: PriceType.IndexPrice,
                ct: ct
                ).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedFuturesKline[]>(result);

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
                        new SharedFuturesKline(request.Symbol, symbol, x.OpenTime, x.ClosePrice, x.HighPrice, x.LowPrice, x.OpenPrice))
                   .ToArray(), nextPageRequest);
        }

        #endregion

    }
}
