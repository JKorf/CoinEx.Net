using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using System;

namespace CoinEx.Net.Clients.SpotApiV2
{
    /// <inheritdoc />
    internal class CoinExRestClientSpotApiExchangeData : ICoinExRestClientSpotApiExchangeData
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiExchangeData(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<HttpResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/time", CoinExExchange.RateLimiter.CoinExRestPublic);
            var result = await _baseClient.SendAsync<CoinExServerTime>(request, null, ct).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<DateTime>(result);

            return HttpResult.Ok(result, result.Data.ServerTime);
        }

        // Doesn't seem to exist on the url specified in the docs
        ///// <inheritdoc />
        //public async Task<HttpResult<CoinExMaintenance[]>> GetMaintenanceInfoAsync(CancellationToken ct = default)
        //{
        //    return await _baseClient.ExecuteAsync<IEnumerable<CoinExMaintenance>>(_baseClient.GetUri("v2/maintain-info"), HttpMethod.Get, ct).ConfigureAwait(false);
        //}

        /// <inheritdoc />
        public async Task<HttpResult<CoinExSymbol[]>> GetSymbolsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/market", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExSymbol[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExAsset[]>> GetAssetsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/assets/info", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExAsset[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExTicker[]>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market", symbols == null ? null : string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/ticker", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExTicker[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int limit, string? mergeLevel = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol },
                { "limit", limit },
                { "interval", mergeLevel ?? "0" }
            };
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/depth", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExOrderBook>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExTrade[]>> GetTradeHistoryAsync(string symbol, int? limit = null, long? lastId = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("limit", limit);
            parameters.Add("last_id", lastId);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/deals", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExTrade[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExKline[]>> GetKlinesAsync(
            string symbol,
            KlineInterval interval, 
            int? limit = null,
            PriceType? priceType = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings)
            {
                { "market", symbol }
            };
            parameters.Add("period", interval);
            parameters.Add("price_type", priceType);
            parameters.Add("limit", limit);
            parameters.Add("start_time", startTime);
            parameters.Add("end_time", endTime);
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/kline", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExKline[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<HttpResult<CoinExIndexPrice[]>> GetIndexPricesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new Parameters(CoinExExchange._parameterSerializationSettings);
            parameters.Add("market", symbols == null ? null : string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, _baseClient.BaseAddress, "v2/spot/index", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExIndexPrice[]>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
