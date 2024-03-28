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
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.ExtensionMethods;

namespace CoinEx.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class CoinExRestClientSpotApiTrading : ICoinExClientSpotApiTrading
    {
        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiTrading(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


        /// <inheritdoc />
        public async Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            return await _baseClient.ExecuteAsync<IEnumerable<string>>(_baseClient.GetUri("spot/market"), HttpMethod.Get, ct).ConfigureAwait(false);
        }
    }
}
