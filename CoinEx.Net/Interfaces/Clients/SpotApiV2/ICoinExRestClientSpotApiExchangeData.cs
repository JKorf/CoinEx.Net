using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV2
{
    /// <summary>
    /// CoinEx exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.
    /// </summary>
    public interface ICoinExRestClientSpotApiExchangeData
    {
        /// <summary>
        /// Get server time
        /// </summary>
        /// <para><a href="https://docs.coinex.com/api/v2/common/http/time" /></para>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);

        // Doesn't seem to exist on the url specified in the docs
        ///// <summary>
        ///// Get maintenance info
        ///// </summary>
        ///// <para><a href="https://docs.coinex.com/api/v2/common/http/maintain" /></para>
        ///// <param name="ct">Cancelation token</param>
        ///// <returns></returns>
        //Task<WebCallResult<IEnumerable<CoinExMaintenance>>> GetMaintenanceInfoAsync(CancellationToken ct = default);

        /// <summary>
        /// Get symbol information
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/http/list-market" /></para>
        /// </summary>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExSymbol>>> GetSymbolsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get asset information
        /// <para><a href="https://docs.coinex.com/api/v2/assets/deposit-withdrawal/http/list-assets-info" /></para>
        /// </summary>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExAsset>>> GetAssetsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get symbol tickers
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/http/list-market-ticker" /></para>
        /// </summary>
        /// <param name="symbols">Fitler by symbol names, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExTicker>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get the orderbook for a symbol
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/http/list-market-depth" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="limit">Amount of rows, 5, 10, 20 or 50</param>
        /// <param name="mergeLevel">The merge level, 0.00000000001 up to 1000, 0 for no merging</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int limit, string? mergeLevel = null, CancellationToken ct = default);

        /// <summary>
        /// Get the trade history for a symbol
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/http/list-market-deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="lastId">The starting point of the query, 0 means to acquire from the latest record</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExTrade>>> GetTradeHistoryAsync(string symbol, int? limit = null, long? lastId = null, CancellationToken ct = default);

        /// <summary>
        /// Get klines/candlesticks
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/http/list-market-kline" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="interval">Kline interval</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="priceType">Price type, either LastPrice(default) or IndexPrice</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, PriceType? priceType = null, CancellationToken ct = default);

        /// <summary>
        /// Get index prices
        /// <para><a href="https://docs.coinex.com/api/v2/spot/market/http/list-market-index" /></para>
        /// </summary>
        /// <param name="symbols">Filter by symbols, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExIndexPrice>>> GetIndexPricesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);
    }
}
