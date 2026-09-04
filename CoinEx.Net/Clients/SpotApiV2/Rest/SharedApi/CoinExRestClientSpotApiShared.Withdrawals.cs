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

        #region Get Withdrawal History

        async Task<ICallResult<SharedWithdrawal[]>> IGetWithdrawalHistory.GetWithdrawalHistoryAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => await GetWithdrawalHistoryAsync(request, pageRequest, ct).ConfigureAwait(false);

        Task<HttpResult<SharedWithdrawal[]>> IWithdrawalRestClient.GetWithdrawalsAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
            => GetWithdrawalHistoryAsync(request, pageRequest, ct);
        GetWithdrawalHistoryOptions IWithdrawalRestClient.GetWithdrawalsOptions => GetWithdrawalHistoryOptions;

        public GetWithdrawalHistoryOptions GetWithdrawalHistoryOptions { get; } = new GetWithdrawalHistoryOptions(_exchangeName, false, true, false, 100);
        public async Task<HttpResult<SharedWithdrawal[]>> GetWithdrawalHistoryAsync(GetWithdrawalsRequest request, PageRequest? pageRequest, CancellationToken ct)
        {
            var validationError = GetWithdrawalHistoryOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedWithdrawal[]>(Exchange, validationError);

            // Determine page token
            int limit = request.Limit ?? 100;
            var direction = DataDirection.Descending;
            var pageParams = Pagination.GetPaginationParameters(direction, limit, request.StartTime, request.EndTime ?? DateTime.UtcNow, pageRequest);

            // Get data
            var result = await _api.Account.GetWithdrawalHistoryAsync(
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

        #endregion

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


        #region Withdraw

        async Task<ICallResult<SharedId>> IWithdraw.WithdrawAsync(WithdrawRequest request, CancellationToken ct)
            => await WithdrawAsync(request, ct).ConfigureAwait(false);

        public WithdrawOptions WithdrawOptions { get; } = new WithdrawOptions(_exchangeName);

        public async Task<HttpResult<SharedId>> WithdrawAsync(WithdrawRequest request, CancellationToken ct)
        {
            var validationError = WithdrawOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedId>(Exchange, validationError);

            // Get data
            var withdrawal = await _api.Account.WithdrawAsync(
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

    }
}
