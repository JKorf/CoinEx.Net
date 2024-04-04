using CryptoExchange.Net.Objects;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Enums;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV2
{
    /// <summary>
    /// CoinEx account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
    /// </summary>
    public interface ICoinExRestClientSpotApiAccount
    {
        /// <summary>
        /// Get trading fees
        /// <para><a href="https://docs.coinex.com/api/v2/account/fees/http/get-account-trade-fees" /></para>
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="accountType">Account type</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExTradeFee>> GetTradingFeesAsync(string symbol, AccountType accountType, CancellationToken ct = default);

        /// <summary>
        /// Update account settings
        /// <para><a href="https://docs.coinex.com/api/v2/account/settings/http/modify-account-settings" /></para>
        /// </summary>
        /// <param name="cetDiscountEnabled">Global switch for CET Deduction</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult> SetAccountConfigAsync(bool cetDiscountEnabled, CancellationToken ct = default);

        /// <summary>
        /// Get balances
        /// <para><a href="https://docs.coinex.com/api/v2/assets/balance/http/get-spot-balance" /></para>
        /// </summary>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get margin balances
        /// <para><a href="https://docs.coinex.com/api/v2/assets/balance/http/get-marigin-balance" /></para>
        /// </summary>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExMarginBalance>>> GetMarginBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get balances in the financial account
        /// <para><a href="https://docs.coinex.com/api/v2/assets/balance/http/get-financial-balance" /></para>
        /// </summary>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExBalance>>> GetFinancialBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Get credit account info
        /// <para><a href="https://docs.coinex.com/api/v2/assets/balance/http/get-credit-info" /></para>
        /// </summary>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExCreditBalance>>> GetCreditAccountAsync(CancellationToken ct = default);

        /// <summary>
        /// Get automated market maker account liquidity
        /// <para><a href="https://docs.coinex.com/api/v2/assets/balance/http/get-amm-liquidity" /></para>
        /// </summary>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExAmmBalance>>> GetAutoMarketMakerAccountLiquidityAsync(CancellationToken ct = default);

        /// <summary>
        /// Apply for margin borrowing
        /// <para><a href="https://docs.coinex.com/api/v2/assets/loan-flat/http/margin-borrow" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="asset">Asset</param>
        /// <param name="quantity">Quantity to borrow</param>
        /// <param name="autoRenew">Whether to renew automatically. Automatic renewal means that after the loan expires, the system will renew the loan based on the latest borrowing interest rate and cycle.</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExBorrow>> MarginBorrowAsync(string symbol, string asset, decimal quantity, bool autoRenew, CancellationToken ct = default);

        /// <summary>
        /// Repay a margin loan
        /// <para><a href="https://docs.coinex.com/api/v2/assets/loan-flat/http/margin-repay" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="asset">Asset</param>
        /// <param name="quantity">Quantity to repay</param>
        /// <param name="borrowId">Loan record ID</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult> MarginRepayAsync(string symbol, string asset, decimal quantity, long? borrowId = null, CancellationToken ct = default);

        /// <summary>
        /// Get borrow history
        /// <para><a href="https://docs.coinex.com/api/v2/assets/loan-flat/http/list-margin-borrow-history" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol</param>
        /// <param name="status">Filter by status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExBorrow>>> GetBorrowHistoryAsync(string? symbol = null, BorrowStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get borrow limits
        /// <para><a href="https://docs.coinex.com/api/v2/assets/loan-flat/http/list-margin-interest-limit" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="asset">Asset</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExBorrowLimit>> GetBorrowLimitAsync(string symbol, string asset, CancellationToken ct = default);

        /// <summary>
        /// Get the deposit address for an asset
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/get-deposit-address" /></para>
        /// </summary>
        /// <param name="asset">The asset to deposit</param>
        /// <param name="network">Network</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string network, CancellationToken ct = default);

        /// <summary>
        /// Renew deposit address
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/update-deposit-address" /></para>
        /// </summary>
        /// <param name="asset">The asset</param>
        /// <param name="network">Network</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExDepositAddress>> RenewDepositAddressAsync(string asset, string network, CancellationToken ct = default);

        /// <summary>
        /// Get deposit history
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/list-deposit-history" /></para>
        /// </summary>
        /// <param name="asset">Asset</param>
        /// <param name="transactionId">Filter by transaction id</param>
        /// <param name="status">Filter by status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExDeposit>>> GetDepositHistoryAsync(string asset, string? transactionId = null, DepositStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default);
    }
}
