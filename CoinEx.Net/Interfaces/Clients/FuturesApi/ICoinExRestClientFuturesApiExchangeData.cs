using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// CoinEx exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.
    /// </summary>
    public interface ICoinExRestClientFuturesApiExchangeData
    {
        /// <summary>
        /// Get server time
        /// <para><a href="https://docs.coinex.com/api/v2/common/http/time" /></para>
        /// </summary>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);

        /// <summary>
        /// Get list of support symbols
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market" /></para>
        /// </summary>
        /// <param name="symbols">Filter by symbol name; max 10, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExFuturesSymbol>>> GetSymbolsAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get symbol tickers
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-ticker" /></para>
        /// </summary>
        /// <param name="symbols">Fitler by symbol names; max 10, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExFuturesTicker>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get the orderbook for a symbol
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-depth" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="limit">Amount of rows, 5, 10, 20 or 50</param>
        /// <param name="mergeLevel">The merge level, 0.00000000001 up to 1000, 0 for no merging</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int limit, string? mergeLevel = null, CancellationToken ct = default);

        /// <summary>
        /// Get the trade history for a symbol
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="limit">Max amount of results</param>
        /// <param name="lastId">The starting point of the query, 0 means to acquire from the latest record</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExTrade>>> GetTradeHistoryAsync(string symbol, int? limit = null, long? lastId = null, CancellationToken ct = default);

        /// <summary>
        /// Get klines/candlesticks
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-kline" /></para>
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
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-index" /></para>
        /// </summary>
        /// <param name="symbols">Filter by symbols, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExIndexPrice>>> GetIndexPricesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get funding rates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-funding-rate" /></para>
        /// </summary>
        /// <param name="symbols">Filter by symbols, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExFundingRate>>> GetFundingRatesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get historical funding rates
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-funding-rate-history" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExFundingRateHistory>>> GetFundingRateHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get historical premium index prices
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-premium-history" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExPremiumIndexHistory>>> GetPremiumIndexPriceHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get position levels
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-position-level" /></para>
        /// </summary>
        /// <param name="symbols">Filter by symbols; max 10, for example `ETHUSDT`</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExPositionLevels>>> GetPositionLevelsAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default);

        /// <summary>
        /// Get liquidation history
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-liquidation-history" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExLiquidation>>> GetLiquidationHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get basis rate history
        /// <para><a href="https://docs.coinex.com/api/v2/futures/market/http/list-market-basis-history" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExBasis>>> GetBasisHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default);
    }
}
