using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Interfaces.Clients.SpotApiV1;

namespace CoinEx.Net.Clients.SpotApiV1
{
    /// <inheritdoc />
    internal class CoinExRestClientSpotApiExchangeData : ICoinExRestClientSpotApiExchangeData
    {
        private const string AssetConfigEndpoint = "common/asset/config";
        private const string CurrencyRateEndpoint = "common/currency/rate";

        private const string MarketListEndpoint = "market/list";
        private const string MarketStatisticsEndpoint = "market/ticker";
        private const string MarketStatisticsListEndpoint = "market/ticker/all";
        private const string MarketDepthEndpoint = "market/depth";
        private const string MarketDealsEndpoint = "market/deals";
        private const string MarketKlinesEndpoint = "market/kline";
        private const string MarketInfoEndpoint = "market/info";
        private const string MiningDifficultyEndpoint = "order/mining/difficulty";

        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiExchangeData(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await _baseClient.Execute<IEnumerable<string>>(_baseClient.GetUrl(MarketListEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, decimal>>> GetCurrencyRateAsync(CancellationToken ct = default)
        {
            return await _baseClient.Execute<Dictionary<string, decimal>>(_baseClient.GetUrl(CurrencyRateEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, CoinExAssetConfig>>> GetAssetsAsync(string? assetType = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("coin_type", assetType);

            return await _baseClient.Execute<Dictionary<string, CoinExAssetConfig>>(_baseClient.GetUrl(AssetConfigEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExSymbolState>> GetTickerAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };

            return await _baseClient.Execute<CoinExSymbolState>(_baseClient.GetUrl(MarketStatisticsEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExSymbolStatesList>> GetTickersAsync(CancellationToken ct = default)
        {
            var data = await _baseClient.Execute<CoinExSymbolStatesList>(_baseClient.GetUrl(MarketStatisticsListEndpoint), HttpMethod.Get, ct)
                .ConfigureAwait(false);
            if (!data)
                return data;

            foreach (var item in data.Data.Tickers)
                item.Value.Symbol = item.Key;
            return data;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int mergeDepth, int? limit = null, CancellationToken ct = default)
        {
            mergeDepth.ValidateIntBetween(nameof(mergeDepth), 0, 8);
            limit?.ValidateIntValues(nameof(limit), 5, 10, 20);

            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "merge", CoinExHelpers.MergeDepthIntToString(mergeDepth) }
            };
            parameters.AddOptionalParameter("limit", limit);

            return await _baseClient.Execute<CoinExOrderBook>(_baseClient.GetUrl(MarketDepthEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExSymbolTrade>>> GetTradeHistoryAsync(string symbol, long? fromId = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };
            parameters.AddOptionalParameter("last_id", fromId);

            return await _baseClient.Execute<IEnumerable<CoinExSymbolTrade>>(_baseClient.GetUrl(MarketDealsEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol }
            };
            return await _baseClient.Execute<Dictionary<string, CoinExSymbol>>(_baseClient.GetUrl(MarketInfoEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(CancellationToken ct = default)
        {
            return await _baseClient.Execute<Dictionary<string, CoinExSymbol>>(_baseClient.GetUrl(MarketInfoEndpoint), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = null, CancellationToken ct = default)
        {
            limit?.ValidateIntBetween(nameof(limit), 1, 1000);
            var parameters = new Dictionary<string, object>
            {
                { "market", symbol },
                { "type", JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)) }
            };
            parameters.AddOptionalParameter("limit", limit);

            return await _baseClient.Execute<IEnumerable<CoinExKline>>(_baseClient.GetUrl(MarketKlinesEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }


        /// <inheritdoc />
        public async Task<WebCallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync(CancellationToken ct = default)
        {
            return await _baseClient.Execute<CoinExMiningDifficulty>(_baseClient.GetUrl(MiningDifficultyEndpoint), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
        }
    }
}
