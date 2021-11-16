using CoinEx.Net.Enums;
using CoinEx.Net.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models;

namespace CoinEx.Net.Interfaces.Clients.Rest.Spot
{
    public interface ICoinExClientSpotExchangeData
    {
        /// <summary>
        /// Gets the exchange rates of currencies
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, decimal>>> GetCurrencyRateAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the asset configs
        /// </summary>
        /// <param name="assetType">Optionally only return a certain type of asset, for example BCH</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<WebCallResult<Dictionary<string, CoinExAssetConfig>>> GetAssetConfigAsync(string? assetType = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of symbols active on CoinEx
        /// </summary>
        /// <returns>List of symbol names</returns>
        Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the state of a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve state for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The state of the symbol</returns>
        Task<WebCallResult<CoinExSymbolState>> GetTickerAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Gets the states of all symbols
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of states for all symbols</returns>
        Task<WebCallResult<CoinExSymbolStatesList>> GetTickersAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets the order book for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve depth data for</param>
        /// <param name="mergeDepth">The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together</param>
        /// <param name="limit">The limit of results returned</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Order book for a symbol</returns>
        Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int mergeDepth, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets the latest trades for a symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="fromId">The id from which on to return trades</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        Task<WebCallResult<IEnumerable<CoinExSymbolTrade>>> GetTradeHistoryAsync(string symbol, long? fromId = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves kline data for a specific symbol
        /// </summary>
        /// <param name="symbol">The symbol to retrieve klines for</param>
        /// <param name="interval">The interval of the candles</param>
        /// <param name="limit">Limit of the number of results</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of klines for a symbol</returns>
        Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves market data for the exchange
        /// </summary>
        /// <param name="symbol">The symbol to retrieve data for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Retrieves market data for the exchange
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of market data for the exchange</returns>
        Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieve the mining difficulty. Requires API credentials
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Mining difficulty</returns>
        Task<WebCallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync(CancellationToken ct = default);
    }
}
