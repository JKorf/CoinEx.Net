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
        #region Get Order Book

        async Task<ICallResult<SharedOrderBook>> IGetOrderBook.GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
            => await GetOrderBookAsync(request, ct).ConfigureAwait(false);

        public GetOrderBookOptions GetOrderBookOptions { get; } = new GetOrderBookOptions(_exchangeName, new[] { 5, 10, 20, 50 }, false);
        public async Task<HttpResult<SharedOrderBook>> GetOrderBookAsync(GetOrderBookRequest request, CancellationToken ct)
        {
            var validationError = GetOrderBookOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedOrderBook>(Exchange, validationError);

            var result = await _api.ExchangeData.GetOrderBookAsync(
                request.Symbol!.GetSymbol(FormatSymbol),
                limit: request.Limit ?? 20,
                ct: ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<SharedOrderBook>(result);

            return HttpResult.Ok(result, new SharedOrderBook(SharedQuantityType.BaseAsset, null, result.Data.Data.Asks, result.Data.Data.Bids));
        }

        #endregion

    }
}
