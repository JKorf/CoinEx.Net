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
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly CoinExRestClientFuturesApi _baseClient;

        internal CoinExRestClientFuturesApiExchangeData(CoinExRestClientFuturesApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/time", CoinExExchange.RateLimiter.CoinExRestPublic);
            var result = await _baseClient.SendAsync<CoinExServerTime>(request, null, ct).ConfigureAwait(false);
            return result.As(result.Data?.ServerTime ?? default);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesSymbol[]>> GetSymbolsAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null :string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/market", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExFuturesSymbol[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFuturesTicker[]>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/ticker", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExFuturesTicker[]>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/depth", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExOrderBook>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExTrade[]>> GetTradeHistoryAsync(string symbol, int? limit = null, long? lastId = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddOptional("limit", limit);
            parameters.AddOptional("last_id", lastId);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/deals", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExTrade[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExKline[]>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, PriceType? priceType = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("period", interval);
            parameters.AddOptionalEnum("price_type", priceType);
            parameters.AddOptional("limit", limit);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/kline", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExKline[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExIndexPrice[]>> GetIndexPricesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/index", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExIndexPrice[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExFundingRate[]>> GetFundingRatesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/funding-rate", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExFundingRate[]>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/funding-rate-history", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendPaginatedAsync<CoinExFundingRateHistory>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPremiumIndexHistory[]>> GetPremiumIndexPriceHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            parameters.AddOptional("page", page);
            parameters.AddOptional("limit", pageSize);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/premium-index-history", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExPremiumIndexHistory[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExPositionLevels[]>> GetPositionLevelsAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/position-level", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExPositionLevels[]>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/liquidation-history", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendPaginatedAsync<CoinExLiquidation>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExBasis[]>> GetBasisHistoryAsync(string symbol, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddOptionalMilliseconds("start_time", startTime);
            parameters.AddOptionalMilliseconds("end_time", endTime);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/futures/basis-history", CoinExExchange.RateLimiter.CoinExRestPublic, 1, false);
            return await _baseClient.SendAsync<CoinExBasis[]>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
