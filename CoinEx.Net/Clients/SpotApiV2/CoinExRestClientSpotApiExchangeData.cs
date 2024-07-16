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
        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiExchangeData(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.ExecuteAsync<CoinExServerTime>(_baseClient.GetUri("v2/time"), HttpMethod.Get, ct).ConfigureAwait(false);
            return result.As(result.Data?.ServerTime ?? default);
        }

        // Doesn't seem to exist on the url specified in the docs
        ///// <inheritdoc />
        //public async Task<WebCallResult<IEnumerable<CoinExMaintenance>>> GetMaintenanceInfoAsync(CancellationToken ct = default)
        //{
        //    return await _baseClient.ExecuteAsync<IEnumerable<CoinExMaintenance>>(_baseClient.GetUri("v2/maintain-info"), HttpMethod.Get, ct).ConfigureAwait(false);
        //}

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExSymbol>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExSymbol>>(_baseClient.GetUri("v2/spot/market"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExAsset>>> GetAssetsAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExAsset>>(_baseClient.GetUri("v2/assets/info"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExTicker>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExTicker>>(_baseClient.GetUri("v2/spot/ticker"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<CoinExOrderBook>(_baseClient.GetUri("v2/spot/depth"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExTrade>>(_baseClient.GetUri("v2/spot/deals"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExKline>>(_baseClient.GetUri("v2/spot/kline"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExIndexPrice>>> GetIndexPricesAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExIndexPrice>>(_baseClient.GetUri("v2/spot/index"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
    }
}
