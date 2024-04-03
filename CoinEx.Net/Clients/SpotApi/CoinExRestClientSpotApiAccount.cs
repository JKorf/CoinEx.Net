using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.ExtensionMethods;
using CoinEx.Net.Objects.Models.V2;

namespace CoinEx.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class CoinExRestClientSpotApiAccount : ICoinExClientSpotApiAccount
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

    }
}
