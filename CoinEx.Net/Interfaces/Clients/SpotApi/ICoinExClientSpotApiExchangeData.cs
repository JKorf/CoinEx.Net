using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;

namespace CoinEx.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// CoinEx exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.
    /// </summary>
    public interface ICoinExClientSpotApiExchangeData
    {
        /// <summary>
        /// Get symbol information
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/http/list-market" /></para>
        /// </summary>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExSymbol>>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get symbol tickers
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/http/list-market-ticker" /></para>
        /// </summary>
        /// <param name="symbols">Fitler by symbol names</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExTicker>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);
    }
}
