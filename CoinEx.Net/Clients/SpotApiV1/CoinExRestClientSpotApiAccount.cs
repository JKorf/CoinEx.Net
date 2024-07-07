using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Interfaces.Clients.SpotApiV1;

namespace CoinEx.Net.Clients.SpotApiV1
{
    /// <inheritdoc />
    internal class CoinExRestClientSpotApiAccount : ICoinExRestClientSpotApiAccount
    {
        private const string AccountInfoEndpoint = "balance/info";
        private const string WithdrawalHistoryEndpoint = "balance/coin/withdraw";
        private const string DepositHistoryEndpoint = "balance/coin/deposit";
        private const string WithdrawEndpoint = "balance/coin/withdraw";
        private const string CancelWithdrawalEndpoint = "balance/coin/withdraw";
        private const string DepositAddressEndpoint = "balance/deposit/address/";


        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiAccount(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.Execute<Dictionary<string, CoinExBalance>>(_baseClient.GetUrl(AccountInfoEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
            if (result)
            {
                foreach (var b in result.Data)
                    b.Value.Asset = b.Key;
            }

            return result;
        }

        /// <inheritdoc />
		public async Task<WebCallResult<CoinExPagedResult<CoinExDeposit>>> GetDepositHistoryAsync(string? asset = null, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", asset);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await _baseClient.ExecutePaged<CoinExDeposit>(_baseClient.GetUrl(DepositHistoryEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPagedResult<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? asset = null, long? withdrawId = null, int? page = null, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 100);
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", asset);
            parameters.AddOptionalParameter("coin_withdraw_id", withdrawId);
            parameters.AddOptionalParameter("page", page);
            parameters.AddOptionalParameter("limit", limit);

            return await _baseClient.Execute<CoinExPagedResult<CoinExWithdrawal>>(_baseClient.GetUrl(WithdrawalHistoryEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string asset, string address, bool localTransfer, decimal quantity, string? network = null, CancellationToken ct = default)
        {
            asset.ValidateNotNull(nameof(asset));
            address.ValidateNotNull(nameof(address));
            var parameters = new Dictionary<string, object>
            {
                { "coin_type", asset },
                { "coin_address", address },
                { "transfer_method", localTransfer ? "local": "onchain" },
                { "actual_amount", quantity.ToString(CultureInfo.InvariantCulture) }
            };
            parameters.AddOptionalParameter("smart_contract_name", network);

            return await _baseClient.Execute<CoinExWithdrawal>(_baseClient.GetUrl(WithdrawEndpoint), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<bool>> CancelWithdrawalAsync(long withdrawId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "coin_withdraw_id", withdrawId }
            };

            var result = await _baseClient.Execute<object>(_baseClient.GetUrl(CancelWithdrawalEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
            return result.As(result.Success);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string? smartContractName = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("smart_contract_name", smartContractName);

            return await _baseClient.Execute<CoinExDepositAddress>(_baseClient.GetUrl(DepositAddressEndpoint + asset), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
    }
}
