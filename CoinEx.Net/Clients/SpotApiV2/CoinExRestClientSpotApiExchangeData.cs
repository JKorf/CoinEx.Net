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
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/time", CoinExExchange.RateLimiter.CoinExRestPublic);
            var result = await _baseClient.SendAsync<CoinExServerTime>(request, null, ct).ConfigureAwait(false);
            return result.As(result.Data?.ServerTime ?? default);
        }

        // Doesn't seem to exist on the url specified in the docs
        ///// <inheritdoc />
        //public async Task<WebCallResult<CoinExMaintenance[]>> GetMaintenanceInfoAsync(CancellationToken ct = default)
        //{
        //    return await _baseClient.ExecuteAsync<IEnumerable<CoinExMaintenance>>(_baseClient.GetUri("v2/maintain-info"), HttpMethod.Get, ct).ConfigureAwait(false);
        //}

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExSymbol[]>> GetSymbolsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/market", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExSymbol[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExAsset[]>> GetAssetsAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/info", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExAsset[]>(request, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExTicker[]>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/ticker", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExTicker[]>(request, parameters, ct).ConfigureAwait(false);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/depth", CoinExExchange.RateLimiter.CoinExRestPublic);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/deals", CoinExExchange.RateLimiter.CoinExRestPublic);
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
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/kline", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExKline[]>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExIndexPrice[]>> GetIndexPricesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/spot/index", CoinExExchange.RateLimiter.CoinExRestPublic);
            return await _baseClient.SendAsync<CoinExIndexPrice[]>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
