using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV1
{
    /// <summary>
    /// CoinEx exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.
    /// </summary>
    public interface ICoinExRestClientSpotApiExchangeData
    {
        /// <summary>
        /// Gets the exchange rates of currencies
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/070currency_rate" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, decimal>>> GetCurrencyRateAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the asset configs
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/071asset_config" /></para>
        /// </summary>
        /// <param name="assetType">Optionally only return a certain type of asset, for example BCH</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, CoinExAssetConfig>>> GetAssetsAsync(string? assetType = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of symbols active on CoinEx
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/020market" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of symbol names</returns>
        Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the state of a specific symbol
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/021ticker" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the symbol</returns>
        Task<WebCallResult<CoinExSymbolState>> GetTickerAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Gets the states of all symbols
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/021ticker" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all symbols</returns>
        Task<WebCallResult<CoinExSymbolStatesList>> GetTickersAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for a symbol
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/022depth" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order book for a symbol</returns>
        Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int mergeDepth, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the latest trades for a symbol
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/023deals" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="fromId">The id from which on to return trades</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        Task<WebCallResult<IEnumerable<CoinExSymbolTrade>>> GetTradeHistoryAsync(string symbol, long? fromId = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves kline data for a specific symbol
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/024kline" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a symbol</returns>
        Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves market data for the exchange
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/026market_single_info" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Retrieves market data for the exchange
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/025marketinfo" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/0310mining_difficulty" /></para>
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Mining difficulty</returns>
        Task<WebCallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync(CancellationToken ct = default);
    }
}
