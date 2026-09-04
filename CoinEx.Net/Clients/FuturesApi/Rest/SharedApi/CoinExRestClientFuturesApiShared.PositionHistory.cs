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

        #region Get Position History

        async Task<ICallResult<SharedPositionHistory[]>> IGetPositionHistory.GetPositionHistoryAsync(GetPositionHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetPositionHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        public GetPositionHistoryOptions GetPositionHistoryOptions { get; } = new GetPositionHistoryOptions(_exchangeName, false, true, true, 100);
        public async Task<HttpResult<SharedPositionHistory[]>> GetPositionHistoryAsync(GetPositionHistoryRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetPositionHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedPositionHistory[]>(Exchange, validationError);

            var direction = request.Direction ?? DataDirection.Ascending;
            var symbol = request.Symbol?.GetSymbol(FormatSymbol);
            var limit = request.Limit ?? 100;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest, false);

            // Get data
            var result = await _api.Trading.GetPositionHistoryAsync(
                symbol: symbol,
                startTime: pageParams.StartTime,
                endTime: pageParams.EndTime,
                page: pageParams.Page,
                pageSize: limit,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedPositionHistory[]>(result);

            var nextPageRequest = Pagination.GetNextPageRequest(
                    () => Pagination.NextPageFromPage(pageParams),
                    result.Data.Items.Length,
                    result.Data.Items.Select(x => x.CreateTime),
                    request.StartTime,
                    request.EndTime ?? DateTime.UtcNow,
                    pageParams);

            return HttpResult.Ok(result, ExchangeHelpers.ApplyFilter(result.Data.Items, x => x.CreateTime, request.StartTime, request.EndTime, direction)
                   .Select(x => 
                        new SharedPositionHistory(
                            ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, x.Symbol),
                            x.Symbol,
                            x.Side == PositionSide.Long ? SharedPositionSide.Long : SharedPositionSide.Short,
                            x.AverageEntryPrice,
                            x.SettlePrice,
                            new SharedOrderQuantity(x.AthPositionQuantity),
                            x.RealizedPnl,
                            x.UpdateTime ?? x.CreateTime)
                        {
                            Leverage = x.Leverage,
                            PositionId = x.Id.ToString()
                        }).ToArray(), nextPageRequest);
        }

        #endregion
    }
}
