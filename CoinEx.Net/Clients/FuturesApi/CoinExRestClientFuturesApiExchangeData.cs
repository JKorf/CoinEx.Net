using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using System;

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc />
    internal class CoinExRestClientFuturesApiExchangeData : ICoinExRestClientFuturesApiExchangeData
    {
        private readonly CoinExRestClientFuturesApi _baseClient;

        internal CoinExRestClientFuturesApiExchangeData(CoinExRestClientFuturesApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.ExecuteAsync<CoinExServerTime>(_baseClient.GetUri("v2/time"), HttpMethod.Get, ct).ConfigureAwait(false);
            return result.As(result.Data?.ServerTime ?? default);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExFuturesSymbol>>> GetSymbolsAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null :string.Join(",", symbols));
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExFuturesSymbol>>(_baseClient.GetUri("v2/futures/market"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExFuturesTicker>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExFuturesTicker>>(_baseClient.GetUri("v2/futures/ticker"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int limit, string? mergeLevel = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "limit", limit },
                { "interval", mergeLevel ?? "0" }
            };
            return await _baseClient.ExecuteAsync<CoinExOrderBook>(_baseClient.GetUri("v2/futures/depth"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExTrade>>> GetTradeHistoryAsync(string symbol, int? limit = null, long? lastId = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("last_id", lastId);
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExTrade>>(_baseClient.GetUri("v2/futures/deals"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, PriceType? priceType = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("period", interval);
            parameters.AddOptionalEnum("price_type", priceType);
            parameters.AddOptional("limit", limit);
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExKline>>(_baseClient.GetUri("v2/futures/kline"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExIndexPrice>>> GetIndexPricesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExIndexPrice>>(_baseClient.GetUri("v2/futures/index"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExFundingRate>>> GetFundingRatesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExFundingRate>>(_baseClient.GetUri("v2/futures/funding-rate"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExFundingRateHistory>>> GetFundingRateHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExFundingRateHistory>(_baseClient.GetUri("v2/futures/funding-rate-history"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExPremiumIndexHistory>>> GetPremiumIndexPriceHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExPremiumIndexHistory>>(_baseClient.GetUri("v2/futures/premium-index-history"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExPositionLevels>>> GetPositionLevelsAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExPositionLevels>>(_baseClient.GetUri("v2/futures/position-level"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPaginated<CoinExLiquidation>>> GetLiquidationHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            return await _baseClient.ExecutePaginatedAsync<CoinExLiquidation>(_baseClient.GetUri("v2/futures/liquidation-history"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExBasis>>> GetBasisHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExBasis>>(_baseClient.GetUri("v2/futures/basis-history"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
    }
}
