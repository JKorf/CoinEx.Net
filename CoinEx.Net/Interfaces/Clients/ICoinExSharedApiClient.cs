using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;

namespace CoinEx.Net.Interfaces.Clients
{
    /// <summary>
    /// Client for the shared REST and WebSocket API implementations of CoinEx
    /// </summary>
    public interface ICoinExSharedApiClient
    {
        /// <summary>
        /// Spot REST shared API implementations
        /// </summary>
        ICoinExRestClientSpotSharedApi SpotRest { get; }

        /// <summary>
        /// Futures REST shared API implementations
        /// </summary>
        ICoinExRestClientFuturesSharedApi FuturesRest { get; }

        /// <summary>
        /// Spot WebSocket shared API implementations
        /// </summary>
        ICoinExSocketClientSpotSharedApi SpotSocket { get; }

        /// <summary>
        /// Futures WebSocket shared API implementations
        /// </summary>
        ICoinExSocketClientFuturesSharedApi FuturesSocket { get; }
    }
}
