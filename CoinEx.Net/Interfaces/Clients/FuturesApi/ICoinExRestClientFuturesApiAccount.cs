using CryptoExchange.Net.Objects;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Enums;
using System.Collections.Generic;

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// CoinEx account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings
    /// </summary>
    public interface ICoinExRestClientFuturesApiAccount
    {
        /// <summary>
        /// Get trading fees for a symbol
        /// <para><a href="https://docs.coinex.com/api/v2/account/fees/http/get-account-trade-fees" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExTradeFee>> GetTradingFeesAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Get balances
        /// <para><a href="https://docs.coinex.com/api/v2/assets/balance/http/get-futures-balance" /></para>
        /// </summary>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExFuturesBalance>>> GetBalancesAsync(CancellationToken ct = default);

        /// <summary>
        /// Set leverage for a symbol
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/adjust-position-leverage" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="mode">Margin mode</param>
        /// <param name="leverage">Leverage</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExLeverage>> SetLeverageAsync(string symbol, MarginMode mode, int leverage, CancellationToken ct = default);
    }
}
