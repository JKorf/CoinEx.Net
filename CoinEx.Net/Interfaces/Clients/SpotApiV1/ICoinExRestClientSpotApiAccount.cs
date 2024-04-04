using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV1
{
    /// <summary>
    /// CoinEx account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
    /// </summary>
    public interface ICoinExRestClientSpotApiAccount
    {
        /// <summary>
        /// Retrieves a list of balances. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/060balance" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of balances</returns>
        Task<WebCallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of deposits. Requires API credentials and withdrawal permission on the API key
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/065get_deposit_list" /></para>
        /// </summary>
        /// <param name="asset">The asset to get history for</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPagedResult<CoinExDeposit>>> GetDepositHistoryAsync(string? asset = null, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Get the deposit address of an asset
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/072get_deposit_address" /></para>
        /// </summary>
        /// <param name="asset">The asset to deposit</param>
        /// <param name="smartContractName">Name of the network to use</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string? smartContractName = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/061get_withdraw_list" /></para>
        /// </summary>
        /// <param name="asset">The asset to get history for</param>
        /// <param name="withdrawId">Retrieve a withdrawal with a specific id</param>
        /// <param name="page">The page in the results to retrieve</param>
        /// <param name="limit">The number of results to return per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPagedResult<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? asset = null, long? withdrawId = null, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Withdraw assets from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/062submit_withdraw" /></para>
        /// </summary>
        /// <param name="asset">The asset to withdraw</param>
        /// <param name="localTransfer">Is it a local transfer between users or onchain</param>
        /// <param name="address">The address to withdraw to</param>
        /// <param name="quantity">The quantity to withdraw. This is the quantity AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees </param>
        /// <param name="network">The network to use for the withdrawal</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The withdrawal object</returns>
        Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string asset, string address, bool localTransfer, decimal quantity, string? network = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/064cancel_withdraw" /></para>
        /// </summary>
        /// <param name="withdrawId">The id of the withdrawal to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<WebCallResult<bool>> CancelWithdrawalAsync(long withdrawId, CancellationToken ct = default);
    }
}
