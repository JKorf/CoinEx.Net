using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiting.Guards;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.SpotApiV2
{
    /// <inheritdoc />
    internal class CoinExRestClientSpotApiAccount : ICoinExRestClientSpotApiAccount
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiAccount(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExTradeFee>> GetTradingFeesAsync(string symbol, AccountType accountType, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("market_type", accountType);

            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/account/trade-fee-rate", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExTradeFee>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult> SetAccountConfigAsync(bool cetDiscountEnabled, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "cet_discount_enabled", cetDiscountEnabled }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/account/settings", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBalance[]>> GetBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/spot/balance", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExBalance[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExMarginBalance[]>> GetMarginBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/margin/balance", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExMarginBalance[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBalance[]>> GetFinancialBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/financial/balance", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExBalance[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExCreditBalance>> GetCreditAccountAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/credit/info", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExCreditBalance>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExAmmBalance[]>> GetAutoMarketMakerAccountLiquidityAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/amm/liquidity", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExAmmBalance[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBorrow>> MarginBorrowAsync(string symbol, string asset, decimal quantity, bool autoRenew, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "ccy", asset },
                { "is_auto_renew", autoRenew }
            };
            parameters.Add("borrow_amount", quantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/assets/margin/borrow", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExBorrow> (request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult> MarginRepayAsync(string symbol, string asset, decimal quantity, long? borrowId = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "ccy", asset },
            };
            parameters.Add("borrow_amount", quantity);
            parameters.Add("borrow_id", borrowId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/assets/margin/repay", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExBorrow>>> GetBorrowHistoryAsync(string? symbol = null, BorrowStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market", symbol);
            parameters.Add("status", status);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/margin/borrow-history", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExBorrow>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExBorrowLimit>> GetBorrowLimitAsync(string symbol, string asset, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "ccy", asset },
            };
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/margin/interest-limit", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExBorrowLimit>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string network, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "chain", network },
                { "ccy", asset },
            };
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/deposit-address", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExDepositAddress>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExDepositAddress>> RenewDepositAddressAsync(string asset, string network, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "chain", network },
                { "ccy", asset },
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/assets/renewal-deposit-address", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExDepositAddress>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExDeposit>>> GetDepositHistoryAsync(string? asset = null, string? transactionId = null, DepositStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("ccy", asset);
            parameters.Add("tx_id", transactionId);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            parameters.Add("status", status);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/deposit-history", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExDeposit>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExWithdrawal>> WithdrawAsync(string asset, decimal quantity, string toAddress, MovementMethod? method = null, string? network = null, string? remark = null, string? memo = null, Dictionary<string, object>? extraParameters = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "ccy", asset },
                { "to_address", toAddress },
            };
            parameters.Add("withdraw_method", method);
            parameters.Add("amount", quantity);
            parameters.Add("chain", network);
            parameters.Add("remark", remark);
            parameters.Add("memo", memo);
            parameters.AddRaw("extra", extraParameters);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/assets/withdraw", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExWithdrawal>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult> CancelWithdrawalAsync(long withdrawalId, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "withdraw_id", withdrawalId }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/assets/cancel-withdraw", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? asset = null, long? withdrawId = null, WithdrawStatusV2? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("ccy", asset);
            parameters.Add("withdraw_id", withdrawId);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            parameters.Add("status", status);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/withdraw", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExWithdrawal>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExDepositWithdrawalConfig>> GetDepositWithdrawalConfigAsync(string asset, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("ccy", asset);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/deposit-withdraw-config", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false,
                new SingleLimitGuard(10, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<CoinExDepositWithdrawalConfig>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExDepositWithdrawalConfig[]>> GetAllDepositWithdrawalConfigsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/all-deposit-withdraw-config", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false,
                new SingleLimitGuard(10, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding));
            return await _baseClient.SendAsync<CoinExDepositWithdrawalConfig[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult> TransferAsync(string asset, AccountType fromAccount, AccountType toAccount, decimal quantity, string? marginSymbol = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "ccy", asset }
            };
            parameters.Add("from_account_type", fromAccount);
            parameters.Add("to_account_type", toAccount);
            parameters.Add("amount", quantity);
            parameters.Add("market", marginSymbol);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/assets/transfer", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExTransfer>>> GetTransfersAsync(string asset, AccountType transferType, TransferStatus? status = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "ccy", asset }
            };
            parameters.Add("transfer_type", transferType);
            parameters.Add("start_time", startTime);
            parameters.Add("end_time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            parameters.Add("status", status);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/transfer-history", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExTransfer>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExAamLiquidity>> AddAutoMarketMakerLiquidityAsync(string symbol, decimal baseAssetQuantity, decimal quoteAssetQuantity, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("base_ccy_amount", baseAssetQuantity);
            parameters.Add("quote_ccy_amount", quoteAssetQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/assets/amm/add-liquidity", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExAamLiquidity>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExAamLiquidity>> RemoveAutoMarketMakerLiquidityAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, _baseClient.BaseAddress, "v2/assets/amm/remove-liquidity", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExAamLiquidity>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExPaginated<CoinExTransaction>>> GetTransactionHistoryAsync(TransactionType transactionType, string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("ccy", asset);
            parameters.Add("type", transactionType);
            parameters.Add("start_time", startTime);
            parameters.Add("end_time", endTime);
            parameters.Add("page", page);
            parameters.Add("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/spot/transcation-history", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExTransaction>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
