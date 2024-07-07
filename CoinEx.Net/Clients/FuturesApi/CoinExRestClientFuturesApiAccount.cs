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
            return await _baseClient.ExecuteAsync<CoinExTradeFee>(_baseClient.GetUri("v2/account/trade-fee-rate"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExFuturesBalance>>> GetBalancesAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.ExecuteAsync<IEnumerable<CoinExFuturesBalance>>(_baseClient.GetUri("v2/assets/futures/balance"), HttpMethod.Get, ct, null, true).ConfigureAwait(false);
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
            return await _baseClient.ExecuteAsync<CoinExLeverage>(_baseClient.GetUri("v2/futures/adjust-position-leverage"), HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }
    }
}
