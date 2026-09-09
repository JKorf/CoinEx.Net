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
        #region Get Deposit Addresses

        async Task<ICallResult<SharedDepositAddress[]>> IGetDepositAddresses.GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
            => await GetDepositAddressesAsync(request, ct).ConfigureAwait(false);

        public GetDepositAddressesOptions GetDepositAddressesOptions { get; } = new GetDepositAddressesOptions(_exchangeName, true)
        {
            ParameterRuleOverwrites = [
                RequestParameterRuleOverride<GetDepositAddressesRequest>.Required(x => x.Network)
            ]
        };

        public async Task<HttpResult<SharedDepositAddress[]>> GetDepositAddressesAsync(GetDepositAddressesRequest request, CancellationToken ct)
        {
            var validationError = GetDepositAddressesOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDepositAddress[]>(Exchange, validationError);

            var depositAddresses = await _api.Account.GetDepositAddressAsync(request.Asset, request.Network!).ConfigureAwait(false);
            if (!depositAddresses.Success)
                return HttpResult.Fail<SharedDepositAddress[]>(depositAddresses);

            return HttpResult.Ok(depositAddresses, new[] { new SharedDepositAddress(request.Asset, depositAddresses.Data.Address)
            {
                Network = request.Network,
                TagOrMemo = depositAddresses.Data.Memo
            }
            });
        }

        #endregion

        #region Get Deposit History

        async Task<ICallResult<SharedDeposit[]>> IGetDepositHistory.GetDepositHistoryAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetDepositHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        Task<HttpResult<SharedDeposit[]>> IDepositRestClient.GetDepositsAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetDepositHistoryAsync(request, pageRequest, ct);
        GetDepositHistoryOptions IDepositRestClient.GetDepositsOptions => GetDepositHistoryOptions;

        public GetDepositHistoryOptions GetDepositHistoryOptions { get; } = new GetDepositHistoryOptions(_exchangeName, false, true, false, 100)
        {
            ParameterRuleOverwrites = [
                RequestParameterRuleOverride<GetDepositsRequest>.Required(x => x.Asset),
                RequestParameterRuleOverride<GetDepositsRequest>.NotSupported(x => x.StartTime),
                RequestParameterRuleOverride<GetDepositsRequest>.NotSupported(x => x.EndTime)
            ]
        };
        public async Task<HttpResult<SharedDeposit[]>> GetDepositHistoryAsync(GetDepositsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetDepositHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedDeposit[]>(Exchange, validationError);

            // Determine page token
            int limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Account.GetDepositHistoryAsync(
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

        #endregion

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

    }
}
