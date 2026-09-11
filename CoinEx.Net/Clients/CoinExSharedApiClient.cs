using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Options;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc />
    public class CoinExSharedApiClient : SharedApiClientBase, ICoinExSharedApiClient
    {
        /// <inheritdoc />
        public ICoinExRestClientSpotSharedApi SpotRest { get; }
        /// <inheritdoc />
        public ICoinExRestClientFuturesSharedApi FuturesRest { get; }
        /// <inheritdoc />
        public ICoinExSocketClientSpotSharedApi SpotSocket { get; }
        /// <inheritdoc />
        public ICoinExSocketClientFuturesSharedApi FuturesSocket { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExSharedApiClient(
            ICoinExRestClient restClient,
            ICoinExSocketClient socketClient,
            IOptions<CoinExOptions> options)
            : base(options.Value.SharedApi.PreferredTransport,
                  restClient.SpotApiV2.SharedApi,
                  restClient.FuturesApi.SharedApi,
                  socketClient.SpotApiV2.SharedApi,
                   socketClient.FuturesApi.SharedApi
                  )
        {
            SpotRest = restClient.SpotApiV2.SharedApi;
            FuturesRest = restClient.FuturesApi.SharedApi;
            SpotSocket = socketClient.SpotApiV2.SharedApi;
            FuturesSocket = socketClient.FuturesApi.SharedApi;
        }
    }
}
