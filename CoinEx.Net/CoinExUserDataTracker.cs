using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Trackers.UserData;
using CryptoExchange.Net.Trackers.UserData.Objects;
using Microsoft.Extensions.Logging;

namespace CoinEx.Net
{
    /// <inheritdoc/>
    public class CoinExUserSpotDataTracker : UserSpotDataTracker
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CoinExUserSpotDataTracker(
            ILogger<CoinExUserSpotDataTracker> logger,
            ICoinExRestClient restClient,
            ICoinExSocketClient socketClient,
            string? userIdentifier,
            SpotUserDataTrackerConfig? config) : base(
                logger,
                restClient.SpotApiV2.SharedApi,

                restClient.SpotApiV2.SharedApi,
                socketClient.SpotApiV2.SharedApi,

                restClient.SpotApiV2.SharedApi,
                restClient.SpotApiV2.SharedApi,
                socketClient.SpotApiV2.SharedApi,

                restClient.SpotApiV2.SharedApi,
                socketClient.SpotApiV2.SharedApi,
                userIdentifier,
                config ?? new SpotUserDataTrackerConfig())
        {
        }
    }

    /// <inheritdoc/>
    public class CoinExUserFuturesDataTracker : UserFuturesDataTracker
    {
        /// <inheritdoc/>
        protected override bool WebsocketPositionUpdatesAreFullSnapshots => false;

        /// <summary>
        /// ctor
        /// </summary>
        public CoinExUserFuturesDataTracker(
            ILogger<CoinExUserFuturesDataTracker> logger,
            ICoinExRestClient restClient,
            ICoinExSocketClient socketClient,
            string? userIdentifier,
            FuturesUserDataTrackerConfig? config) : base(logger,
                restClient.FuturesApi.SharedApi,

                restClient.FuturesApi.SharedApi,
                socketClient.FuturesApi.SharedApi,

                restClient.FuturesApi.SharedApi,
                restClient.FuturesApi.SharedApi,
                socketClient.FuturesApi.SharedApi,

                restClient.FuturesApi.SharedApi,
                socketClient.FuturesApi.SharedApi,

                restClient.FuturesApi.SharedApi,
                socketClient.FuturesApi.SharedApi,
                userIdentifier,
                config ?? new FuturesUserDataTrackerConfig())
        {
        }
    }
}
