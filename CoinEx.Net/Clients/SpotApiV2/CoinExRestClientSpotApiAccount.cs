using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net.Objects;
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
        public async Task<WebCallResult<CoinExTradeFee>> GetTradingFeesAsync(string symbol, AccountType accountType, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", accountType);

            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/account/trade-fee-rate", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExTradeFee>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> SetAccountConfigAsync(bool cetDiscountEnabled, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "cet_discount_enabled", cetDiscountEnabled }
            };

            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/account/settings", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/spot/balance", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<IEnumerable<CoinExBalance>>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExMarginBalance>>> GetMarginBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/margin/balance", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<IEnumerable<CoinExMarginBalance>>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBalance>>> GetFinancialBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/financial/balance", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<IEnumerable<CoinExBalance>>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExCreditBalance>> GetCreditAccountAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/credit/info", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExCreditBalance>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExAmmBalance>>> GetAutoMarketMakerAccountLiquidityAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/amm/liquidity", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<IEnumerable<CoinExAmmBalance>>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBorrow>> MarginBorrowAsync(string symbol, string asset, decimal quantity, bool autoRenew, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "ccy", asset },
                { "is_auto_renew", autoRenew }
            };
            parameters.AddString("borrow_amount", quantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/assets/margin/borrow", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExBorrow> (request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> MarginRepayAsync(string symbol, string asset, decimal quantity, long? borrowId = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "ccy", asset },
            };
            parameters.AddString("borrow_amount", quantity);
            parameters.AddOptional("borrow_id", borrowId);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/assets/margin/repay", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExBorrow>>> GetBorrowHistoryAsync(string? symbol = null, BorrowStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddOptionalEnum("status", status);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/margin/borrow-history", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExBorrow>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBorrowLimit>> GetBorrowLimitAsync(string symbol, string asset, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "ccy", asset },
            };
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/margin/interest-limit", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExBorrowLimit>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string network, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "chain", network },
                { "ccy", asset },
            };
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/deposit-address", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, true);
            return await _baseClient.SendAsync<CoinExDepositAddress>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExDepositAddress>> RenewDepositAddressAsync(string asset, string network, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "chain", network },
                { "ccy", asset },
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/assets/renewal-deposit-address", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExDepositAddress>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExDeposit>>> GetDepositHistoryAsync(string asset, string? transactionId = null, DepositStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "ccy", asset },
            };
            parameters.AddOptional("tx_id", transactionId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            parameters.AddOptionalEnum("status", status);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/deposit-history", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExDeposit>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string asset, decimal quanity, string toAddress, MovementMethod? method = null, string? network = null, string? remark = null, string? memo = null, Dictionary<string, object>? extraParameters = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "ccy", asset },
                { "to_address", toAddress },
            };
            parameters.AddOptionalEnum("withdraw_method", method);
            parameters.AddString("amount", quanity);
            parameters.AddOptional("chain", network);
            parameters.AddOptional("remark", remark);
            parameters.AddOptional("memo", memo);
            parameters.AddOptional("extra", extraParameters);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/assets/withdraw", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExWithdrawal>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> CancelWithdrawalAsync(long withdrawalId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "withdraw_id", withdrawalId }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/assets/cancel-withdraw", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? asset = null, long? withdrawId = null, WithdrawStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("ccy", asset);
            parameters.AddOptional("withdraw_id", withdrawId);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            parameters.AddOptionalEnum("status", status);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/withdraw", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExWithdrawal>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExDepositWithdrawalConfig>> GetDepositWithdrawalConfigAsync(string asset, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("ccy", asset);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/deposit-withdraw-config", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, false);
            return await _baseClient.SendAsync<CoinExDepositWithdrawalConfig>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExDepositWithdrawalConfig>>> GetAllDepositWithdrawalConfigsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/all-deposit-withdraw-config", CoinExExchange.RateLimiter.CoinExRestSpotAccountQuery, 1, false);
            return await _baseClient.SendAsync<IEnumerable<CoinExDepositWithdrawalConfig>>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> TransferAsync(string asset, AccountType fromAccount, AccountType toAccount, decimal quantity, string? marginSymbol = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "ccy", asset }
            };
            parameters.AddEnum("from_account_type", fromAccount);
            parameters.AddEnum("to_account_type", toAccount);
            parameters.AddString("amount", quantity);
            parameters.AddOptional("market", marginSymbol);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/assets/transfer", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExTransfer>>> GetTransfersAsync(string asset, AccountType transferType, string? marginSymbol = null, TransferStatus? status = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "ccy", asset }
            };
            parameters.AddEnum("transfer_type", transferType);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            parameters.AddOptionalEnum("status", status);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/transfer-history", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExTransfer>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExAamLiquidity>> AddAutoMarketMakerLiquidityAsync(string symbol, decimal baseAssetQuantity, decimal quoteAssetQuantity, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddString("base_ccy_amount", baseAssetQuantity);
            parameters.AddString("quote_ccy_amount", quoteAssetQuantity);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/assets/amm/add-liquidity", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExAamLiquidity>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExAamLiquidity>> RemoveAutoMarketMakerLiquidityAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/assets/amm/remove-liquidity", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExAamLiquidity>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExTransaction>>> GetTransactionHistoryAsync(TransactionType transactionType, string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("ccy", asset);
            parameters.AddEnum("type", transactionType);
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/spot/transcation-history", CoinExExchange.RateLimiter.CoinExRestSpotAccountHistory, 1, true);
            return await _baseClient.SendPaginatedAsync<CoinExTransaction>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
