using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;
using CoinEx.Net.Interfaces.Clients.FuturesApi;

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc />
    internal class CoinExRestClientFuturesApiAccount : ICoinExRestClientFuturesApiAccount
    {
        private static readonly RequestDefinitionCache _definitions = new RequestDefinitionCache();
        private readonly CoinExRestClientFuturesApi _baseClient;

        internal CoinExRestClientFuturesApiAccount(CoinExRestClientFuturesApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExTradeFee>> GetTradingFeesAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", AccountType.Futures);
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/account/trade-fee-rate", CoinExExchange.RateLimiter.CoinExRestSpotAccount, 1, true);
            return await _baseClient.SendAsync<CoinExTradeFee>(request, parameters, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExFuturesBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            var request = _definitions.GetOrCreate(HttpMethod.Get, "v2/assets/futures/balance", CoinExExchange.RateLimiter.CoinExRestFuturesAccount, 1, true);
            var result = await _baseClient.SendAsync< IEnumerable<CoinExFuturesBalance>>(request, null, ct).ConfigureAwait(false);
            if (result && result.Data == null)
                return result.As<IEnumerable<CoinExFuturesBalance>>(Array.Empty<CoinExFuturesBalance>());

            return result;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExLeverage>> SetLeverageAsync(string symbol, MarginMode mode, int leverage, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol },
                { "leverage", leverage }
            };
            parameters.AddEnum("market_Type", AccountType.Futures);
            parameters.AddEnum("margin_mode", mode);
            var request = _definitions.GetOrCreate(HttpMethod.Post, "v2/futures/adjust-position-leverage", CoinExExchange.RateLimiter.CoinExRestFuturesOrder, 1, true);
            return await _baseClient.SendAsync<CoinExLeverage>(request, parameters, ct).ConfigureAwait(false);
        }
    }
}
