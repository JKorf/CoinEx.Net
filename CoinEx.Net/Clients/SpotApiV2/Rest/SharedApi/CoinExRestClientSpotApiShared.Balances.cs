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
        #region Balance client
        public GetBalancesOptions GetBalancesOptions { get; } = new GetBalancesOptions(_exchangeName, AccountTypeFilter.Spot, AccountTypeFilter.Margin);

        public async Task<HttpResult<SharedBalance[]>> GetBalancesAsync(GetBalancesRequest request, CancellationToken ct)
        {
            var validationError = GetBalancesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedBalance[]>(Exchange, validationError);

            if (request.AccountType == SharedAccountType.Spot || request.AccountType == null)
            {
                var result = await _api.Account.GetBalancesAsync(ct: ct).ConfigureAwait(false);
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
                var result = await _api.Account.GetMarginBalancesAsync(ct: ct).ConfigureAwait(false);
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
    }
}
