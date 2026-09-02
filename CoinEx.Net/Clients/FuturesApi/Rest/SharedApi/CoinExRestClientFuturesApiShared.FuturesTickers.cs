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
        #region Ticker client

        public GetFuturesTickerOptions GetFuturesTickerOptions { get; } = new GetFuturesTickerOptions(_exchangeName);
        public async Task<HttpResult<SharedFuturesTicker>> GetFuturesTickerAsync(GetTickerRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesTickerOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker>(Exchange, validationError);

            var resultTicker = _api.ExchangeData.GetTickersAsync(new[] { request.Symbol!.GetSymbol(FormatSymbol) }, ct: ct);
            var resultFunding = _api.ExchangeData.GetFundingRatesAsync(new[] { request.Symbol!.GetSymbol(FormatSymbol) }, ct: ct);
            await Task.WhenAll(resultTicker, resultFunding).ConfigureAwait(false);
            if (!resultTicker.Result.Success)
                return HttpResult.Fail<SharedFuturesTicker>(resultTicker.Result);
            if (!resultFunding.Result.Success)
                return HttpResult.Fail<SharedFuturesTicker>(resultFunding.Result);

            var ticker = resultTicker.Result.Data.SingleOrDefault();
            var funding = resultFunding.Result.Data.SingleOrDefault();

            if (ticker == null || funding == null)
                return HttpResult.Fail<SharedFuturesTicker>(resultTicker.Result, new ServerError(new ErrorInfo(ErrorType.Unknown, "Not found")));

            return HttpResult.Ok(resultTicker.Result, 
                new SharedFuturesTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, ticker.Symbol),
                    ticker.Symbol, 
                    ticker.LastPrice,
                    ticker.HighPrice, 
                    ticker.LowPrice,
                    new SharedOrderQuantity(ticker.Volume, ticker.Value), 
                    ticker.OpenPrice == 0 ? null : Math.Round(ticker.LastPrice / ticker.OpenPrice * 100 - 100, 2))
            {
                IndexPrice = ticker.IndexPrice,
                MarkPrice = ticker.MarkPrice,
                FundingRate = funding.NextFundingRate,
                NextFundingTime = funding.NextFundingTime
            });
        }

        Task<HttpResult<SharedFuturesTicker[]>> IFuturesTickerRestClient.GetFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
           => GetAllFuturesTickersAsync(request, ct);
        GetAllFuturesTickersOptions IFuturesTickerRestClient.GetFuturesTickersOptions => GetAllFuturesTickersOptions;

        public GetAllFuturesTickersOptions GetAllFuturesTickersOptions { get; } = new GetAllFuturesTickersOptions(_exchangeName);
        public async Task<HttpResult<SharedFuturesTicker[]>> GetAllFuturesTickersAsync(GetTickersRequest request, CancellationToken ct)
        {
            var validationError = GetAllFuturesTickersOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesTicker[]>(Exchange, validationError);

            var resultTickers = _api.ExchangeData.GetTickersAsync(ct: ct);
            var resultFunding = _api.ExchangeData.GetFundingRatesAsync(ct: ct);
            await Task.WhenAll(resultTickers, resultFunding).ConfigureAwait(false);
            if (!resultTickers.Result.Success)
                return HttpResult.Fail<SharedFuturesTicker[]>(resultTickers.Result);
            if (!resultFunding.Result.Success)
                return HttpResult.Fail<SharedFuturesTicker[]>(resultFunding.Result);

            return HttpResult.Ok(resultTickers.Result, resultTickers.Result.Data.Select(x =>
            {
                var funding = resultFunding.Result.Data.Single(p => p.Symbol == x.Symbol);
                return new SharedFuturesTicker(
                    ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, x.Symbol),
                    x.Symbol, 
                    x.LastPrice,
                    x.HighPrice, 
                    x.LowPrice,
                    new SharedOrderQuantity(x.Volume, x.Value), 
                    x.OpenPrice == 0 ? null : Math.Round(x.LastPrice / x.OpenPrice * 100 - 100, 2))
                {
                    IndexPrice = x.IndexPrice,
                    MarkPrice = x.MarkPrice,
                    FundingRate = funding.NextFundingRate,
                    NextFundingTime = funding.NextFundingTime
                };
            }).ToArray());
        }

        #endregion
    }
}
