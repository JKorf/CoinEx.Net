using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using System;

namespace CoinEx.Net.Clients.SpotApiV2
{
    /// <inheritdoc />
    internal class CoinExRestClientSpotApiAccount : ICoinExRestClientSpotApiAccount
    {
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
            return await _baseClient.ExecuteAsync<CoinExTradeFee>(_baseClient.GetUri("v2/account/trade-fee-rate"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> SetAccountConfigAsync(bool cetDiscountEnabled, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "cet_discount_enabled", cetDiscountEnabled }
            };
            return await _baseClient.ExecuteAsync(_baseClient.GetUri("v2/account/settings"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExBalance>>(_baseClient.GetUri("v2/assets/spot/balance"), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExMarginBalance>>> GetMarginBalancesAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExMarginBalance>>(_baseClient.GetUri("v2/assets/margin/balance"), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBalance>>> GetFinancialBalancesAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExBalance>>(_baseClient.GetUri("v2/assets/financial/balance"), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExCreditBalance>>> GetCreditAccountAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExCreditBalance>>(_baseClient.GetUri("v2/assets/credit/info"), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExAmmBalance>>> GetAutoMarketMakerAccountLiquidityAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExAmmBalance>>(_baseClient.GetUri("v2/assets/amm/liquidity"), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<CoinExBorrow>(_baseClient.GetUri("v2/assets/margin/borrow"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync(_baseClient.GetUri("v2/assets/margin/repay"), HttpMethod.Post, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExBorrow>>> GetBorrowHistoryAsync(string? symbol = null, BorrowStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbol);
            parameters.AddOptionalEnum("status", status);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExBorrow>(_baseClient.GetUri("v2/assets/margin/borrow-history"), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBorrowLimit>> GetBorrowLimitAsync(string symbol, string asset, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "ccy", asset },
            };
            return await _baseClient.ExecuteAsync<CoinExBorrowLimit>(_baseClient.GetUri("v2/assets/margin/interest-limit"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string network, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "chain", network },
                { "ccy", asset },
            };
            return await _baseClient.ExecuteAsync<CoinExDepositAddress>(_baseClient.GetUri("v2/assets/deposit-address"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExDepositAddress>> RenewDepositAddressAsync(string asset, string network, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "chain", network },
                { "ccy", asset },
            };
            return await _baseClient.ExecuteAsync<CoinExDepositAddress>(_baseClient.GetUri("v2/assets/renewal-deposit-address"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExDeposit>(_baseClient.GetUri("v2/assets/deposit-history"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<CoinExWithdrawal>(_baseClient.GetUri("v2/assets/withdraw"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult> CancelWithdrawalAsync(long withdrawalId, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "withdraw_id", withdrawalId }
            };
            return await _baseClient.ExecuteAsync(_baseClient.GetUri("v2/assets/cancel-withdraw"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExWithdrawal>(_baseClient.GetUri("v2/assets/withdraw"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExDepositWithdrawalConfig>> GetDepositWithdrawalConfigAsync(string asset, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("ccy", asset);
            return await _baseClient.ExecuteAsync<CoinExDepositWithdrawalConfig>(_baseClient.GetUri("v2/assets/deposit-withdraw-config"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync(_baseClient.GetUri("v2/assets/transfer"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExTransfer>(_baseClient.GetUri("v2/assets/transfer-history"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<CoinExAamLiquidity>(_baseClient.GetUri("v2/amm/add-liquidity"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExAamLiquidity>> RemoveAutoMarketMakerLiquidityAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            return await _baseClient.ExecuteAsync<CoinExAamLiquidity>(_baseClient.GetUri("v2/amm/remove-liquidity"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
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
            return await _baseClient.ExecutePaginatedAsync<CoinExTransaction>(_baseClient.GetUri("v2/assets/spot/transcation-history"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
    }
}
