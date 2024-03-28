using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Interfaces.Clients.SpotApi;

namespace CoinEx.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class CoinExRestClientSpotApiExchangeData : ICoinExClientSpotApiExchangeData
    {
        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiExchangeData(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExSymbol>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExSymbol>>(_baseClient.GetUri("v2/spot/market"), HttpMethod.Get, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<CoinExTicker>>> GetTickersAsync(IEnumerable<string>? symbols = null, CancellationToken ct = default)
        {
            var parameters = new ParameterCollection();
            parameters.AddOptional("market", symbols == null ? null : string.Join(",", symbols));
            return await _baseClient.ExecuteAsync<IEnumerable<CoinExTicker>>(_baseClient.GetUri("v2/spot/ticker"), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
        }
    }
}
