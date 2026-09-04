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
        #region Set Futures Tp Sl

        async Task<ICallResult<SharedId>> ISetFuturesTpSl.SetFuturesTpSlAsync(SetTpSlRequest request, CancellationToken ct)
            => await SetFuturesTpSlAsync(request, ct).ConfigureAwait(false);

        public SetFuturesTpSlOptions SetFuturesTpSlOptions { get; } = new SetFuturesTpSlOptions(_exchangeName, true)
        {
            RequiredRequestParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(PlaceFuturesTriggerOrderRequest.PositionMode), typeof(SharedPositionMode), "PositionMode the account is in", SharedPositionMode.OneWay)
            }
        };

        public async Task<HttpResult<SharedId>> SetFuturesTpSlAsync(SetTpSlRequest request, CancellationToken ct)
        {
            var validationError = SetFuturesTpSlOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            HttpResult<CoinExPosition> result;
            if (request.TpSlSide == SharedTpSlSide.TakeProfit)
            {
                result = await _api.Trading.SetTakeProfitAsync(
                    request.Symbol!.GetSymbol(FormatSymbol),
                    PriceType.LastPrice,
                    request.TriggerPrice,
                    ct: ct).ConfigureAwait(false);
            }
            else
            {
                result = await _api.Trading.SetTakeProfitAsync(
                                request.Symbol!.GetSymbol(FormatSymbol),
                                PriceType.LastPrice,
                                request.TriggerPrice,
                                ct: ct).ConfigureAwait(false);
            }

            if (!result.Success)
                return HttpResult.Fail<SharedId>(result);

            // Return
            return HttpResult.Ok(result, new SharedId(""));
        }

        #endregion

        #region Cancel Futures Tp Sl

        async Task<ICallResult<bool>> ICancelFuturesTpSl.CancelFuturesTpSlAsync(CancelTpSlRequest request, CancellationToken ct)
            => await CancelFuturesTpSlAsync(request, ct).ConfigureAwait(false);

        public CancelFuturesTpSlOptions CancelFuturesTpSlOptions { get; } = new CancelFuturesTpSlOptions(_exchangeName, true)
        {
            RequiredRequestParameters = new List<ParameterDescription>
            {
                new ParameterDescription(nameof(CancelTpSlRequest.TpSlSide), typeof(SharedTpSlSide), "Take profit / stop loss side to cancel", SharedTpSlSide.TakeProfit)
            }
        };

        public async Task<HttpResult<bool>> CancelFuturesTpSlAsync(CancelTpSlRequest request, CancellationToken ct)
        {
            var validationError = CancelFuturesTpSlOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<bool>(Exchange, validationError);

            HttpResult<CoinExPosition> result;
            if (request.TpSlSide == SharedTpSlSide.TakeProfit)
            {
                result = await _api.Trading.SetTakeProfitAsync(
                    request.Symbol!.GetSymbol(FormatSymbol),
                    PriceType.LastPrice,
                    0,
                    ct: ct).ConfigureAwait(false);
            }
            else
            {
                result = await _api.Trading.SetStopLossAsync(
                                request.Symbol!.GetSymbol(FormatSymbol),
                                PriceType.LastPrice,
                                0,
                                ct: ct).ConfigureAwait(false);
            }
            if (!result.Success)
                return HttpResult.Fail<bool>(result);

            // Return
            return HttpResult.Ok(result, true);
        }

        #endregion

    }
}
