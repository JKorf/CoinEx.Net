using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc />
    public class CoinExSharedApiClient : ICoinExSharedApiClient
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
            ICoinExSocketClient socketClient)
        {
            SpotRest = restClient.SpotApiV2.SharedApi;
            FuturesRest = restClient.FuturesApi.SharedApi;
            SpotSocket = socketClient.SpotApiV2.SharedApi;
            FuturesSocket = socketClient.FuturesApi.SharedApi;
        }
    }
}
