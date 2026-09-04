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
            SharedKlineInterval.OneWeek)
        {
        };

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

            // Get data
            var result = await _api.ExchangeData.GetKlinesAsync(
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
                           new SharedOrderQuantity(x.Volume, x.Value)))
                   .ToArray(), nextPageRequest);
        }

        #endregion

    }
}
