using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.ExtensionMethods;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using System;
using CoinEx.Net.Interfaces.Clients.FuturesApi;

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc />
    public class CoinExRestClientFuturesApiAccount : ICoinExRestClientFuturesApiAccount
    {
        private readonly CoinExRestClientFuturesApi _baseClient;

        internal CoinExRestClientFuturesApiAccount(CoinExRestClientFuturesApi baseClient)
        {
            _baseClient = baseClient;
        }

        /// <inheritdoc />
        public async Task<WebCallResult<CoinExTradeFee>> GetTradingFeesAsync(string symbol, AccountType accountType, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection()
            {
                { "market", symbol }
            };
            parameters.AddEnum("market_type", accountType);
            return await _baseClient.ExecuteAsync<CoinExTradeFee>(_baseClient.GetUri("v2/account/trade-fee-rate"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
    }
}
