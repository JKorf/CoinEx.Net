using CryptoExchange.Net.Objects;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Enums;
using System.Collections.Generic;
using System;

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
        /// <param name="symbol">Symbol name, for example `ETHUSDT`</param>
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
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
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
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
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
        /// <param name="symbol">Filter by symbol, for example `ETHUSDT`</param>
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
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="asset">Asset</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExBorrowLimit>> GetBorrowLimitAsync(string symbol, string asset, CancellationToken ct = default);

        /// <summary>
        /// Get the deposit address for an asset
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/get-deposit-address" /></para>
        /// </summary>
        /// <param name="asset">The asset to deposit, for example `ETH`</param>
        /// <param name="network">Network</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string network, CancellationToken ct = default);

        /// <summary>
        /// Renew deposit address
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/update-deposit-address" /></para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="network">Network</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExDepositAddress>> RenewDepositAddressAsync(string asset, string network, CancellationToken ct = default);

        /// <summary>
        /// Get deposit history
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/list-deposit-history" /></para>
        /// </summary>
        /// <param name="asset">Asset, for example `ETH`</param>
        /// <param name="transactionId">Filter by transaction id</param>
        /// <param name="status">Filter by status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExDeposit>>> GetDepositHistoryAsync(string asset, string? transactionId = null, DepositStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Withdraw funds to an external address or another CoinEx user
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/withdrawal" /></para>
        /// </summary>
        /// <param name="asset">Asset to withdraw, for example `ETH`</param>
        /// <param name="quantity">Withdrawal quantity</param>
        /// <param name="toAddress">Withdrawal address.The withdrawal address needs to be added to the IP whitelist via Developer Console</param>
        /// <param name="method">Withdrawal methods (On-chain or inter-user transfer). Default as on-chain withdrawal</param>
        /// <param name="network">Network name. Required for On-chain, not required for inter-user transfer</param>
        /// <param name="remark">Withdrawal note</param>
        /// <param name="memo">Memo for the withdrawal, required for some deposits</param>
        /// <param name="extraParameters">If it is a withdrawal from the KDA chain, you need to append the chain_id field to the extra field</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string asset, decimal quantity, string toAddress, MovementMethod? method = null, string? network = null, string? remark = null, string? memo = null, Dictionary<string, object>? extraParameters = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel a pending withdrawal
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/cancel-withdrawal" /></para>
        /// </summary>
        /// <param name="withdrawalId">The withdrawal id</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult> CancelWithdrawalAsync(long withdrawalId, CancellationToken ct = default);

        /// <summary>
        /// Get withdrawal history
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/list-withdrawal-history" /></para>
        /// </summary>
        /// <param name="asset">Filter by asset, for example `ETH`</param>
        /// <param name="withdrawId">Filter by withdrawal id</param>
        /// <param name="status">Filter by status</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? asset = null, long? withdrawId = null, WithdrawStatus? status = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get withdraw and deposit information for an asset
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/get-deposit-withdrawal-config" /></para>
        /// </summary>
        /// <param name="asset">The asset, for example `ETH`</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExDepositWithdrawalConfig>> GetDepositWithdrawalConfigAsync(string asset, CancellationToken ct = default);

        /// <summary>
        /// Transfer between accounts
        /// <para><a href="https://docs.coinex.com/api/v2/assets/transfer/http/transfer" /></para>
        /// </summary>
        /// <param name="asset">Asset to transfer, for example `ETH`</param>
        /// <param name="fromAccount">From account</param>
        /// <param name="toAccount">To account</param>
        /// <param name="quantity">Quantity to transfer</param>
        /// <param name="marginSymbol">Margin symbol, only needed when from or to account type is Margin</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult> TransferAsync(string asset, AccountType fromAccount, AccountType toAccount, decimal quantity, string? marginSymbol = null, CancellationToken ct = default);

        /// <summary>
        /// Get transfer history
        /// <para><a href="https://docs.coinex.com/api/v2/assets/transfer/http/list-transfer-history" /></para>
        /// </summary>
        /// <param name="asset">Asset, for example `ETH`</param>
        /// <param name="transferType">Transfer type. Must be either Margin or Futures</param>
        /// <param name="marginSymbol">Filter by margin symbol</param>
        /// <param name="status">Filter by status</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExTransfer>>> GetTransfersAsync(string asset, AccountType transferType, string? marginSymbol = null, TransferStatus? status = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Add AAM liquidity
        /// <para><a href="https://docs.coinex.com/api/v2/assets/amm/http/add-liquidtiy" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETH`</param>
        /// <param name="baseAssetQuantity">Base asset quantity to add</param>
        /// <param name="quoteAssetQuantity">Quote asset quantity to add</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExAamLiquidity>> AddAutoMarketMakerLiquidityAsync(string symbol, decimal baseAssetQuantity, decimal quoteAssetQuantity, CancellationToken ct = default);

        /// <summary>
        /// Remove AAM liquidity. Currently only support withdrawing all liquidity
        /// <para><a href="https://docs.coinex.com/api/v2/assets/amm/http/remove-liquidtiy" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExAamLiquidity>> RemoveAutoMarketMakerLiquidityAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get account transaction history
        /// <para><a href="https://docs.coinex.com/api/v2/assets/balance/http/get-spot-transcation-history" /></para>
        /// </summary>
        /// <param name="transactionType"></param>
        /// <param name="asset">Filter by asset, for example `ETH`</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExTransaction>>> GetTransactionHistoryAsync(TransactionType transactionType, string? asset = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);
    }
}
