using CoinEx.Net.Converters;
using CryptoExchange.Net;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiting;
using CryptoExchange.Net.RateLimiting.Filters;
using CryptoExchange.Net.RateLimiting.Guards;
using CryptoExchange.Net.RateLimiting.Interfaces;
using CryptoExchange.Net.SharedApis;
using System;
using System.Text.Json.Serialization;

namespace CoinEx.Net
{
    /// <summary>
    /// CoinEx exchange information and configuration
    /// </summary>
    public static class CoinExExchange
    {
        /// <summary>
        /// Exchange name
        /// </summary>
        public static string ExchangeName => "CoinEx";

        /// <summary>
        /// Exchange name
        /// </summary>
        public static string DisplayName => "CoinEx";

        /// <summary>
        /// Url to exchange image
        /// </summary>
        public static string ImageUrl { get; } = "https://raw.githubusercontent.com/JKorf/CoinEx.Net/master/CoinEx.Net/Icon/icon.png";

        /// <summary>
        /// Url to the main website
        /// </summary>
        public static string Url { get; } = "https://www.coinex.com";

        /// <summary>
        /// Urls to the API documentation
        /// </summary>
        public static string[] ApiDocsUrl { get; } = new[] {
            "https://viabtc.github.io/coinex_api_en_doc/",
            "https://docs.coinex.com/api/v2/"
            };

        /// <summary>
        /// Type of exchange
        /// </summary>
        public static ExchangeType Type { get; } = ExchangeType.CEX;

        internal static JsonSerializerContext _serializerContext = JsonSerializerContextCache.GetOrCreate<CoinExSourceGenerationContext>();

        /// <summary>
        /// Aliases for CoinEx assets
        /// </summary>
        public static AssetAliasConfiguration AssetAliases { get; } = new AssetAliasConfiguration
        {
            Aliases = [
                new AssetAlias("USDT", SharedSymbol.UsdOrStable.ToUpperInvariant(), AliasType.OnlyToExchange)
            ]
        };

        /// <summary>
        /// Format a base and quote asset to a CoinEx recognized symbol 
        /// </summary>
        /// <param name="baseAsset">Base asset</param>
        /// <param name="quoteAsset">Quote asset</param>
        /// <param name="tradingMode">Trading mode</param>
        /// <param name="deliverTime">Delivery time for delivery futures</param>
        /// <returns></returns>
        public static string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
        {
            baseAsset = AssetAliases.CommonToExchangeName(baseAsset.ToUpperInvariant());
            quoteAsset = AssetAliases.CommonToExchangeName(quoteAsset.ToUpperInvariant());

            return $"{baseAsset}{quoteAsset}";
        }

        /// <summary>
        /// Rate limiter configuration for the CoinEx API
        /// </summary>
        public static CoinExRateLimiters RateLimiter { get; } = new CoinExRateLimiters();
    }

    /// <summary>
    /// Rate limiter configuration for the CoinEx API
    /// </summary>
    public class CoinExRateLimiters
    {
        /// <summary>
        /// Event for when a rate limit is triggered
        /// </summary>
        public event Action<RateLimitEvent> RateLimitTriggered;

        /// <summary>
        /// Event when the rate limit is updated. Note that it's only updated when a request is send, so there are no specific updates when the current usage is decaying.
        /// </summary>
        public event Action<RateLimitUpdateEvent> RateLimitUpdated;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal CoinExRateLimiters()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Initialize();
        }

        private void Initialize()
        {
            var overallGuard = new RateLimitGuard(RateLimitGuard.PerHost, Array.Empty<IGuardFilter>(), 400, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding);// 400 requests per IP
            CoinExRestPublic = new RateLimitGate("CoinEx Public").AddGuard(overallGuard);
            CoinExRestSpotOrder = new RateLimitGate("CoinEx Spot Order")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 30, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 30 requests per second
            CoinExRestSpotCancel = new RateLimitGate("CoinEx Spot Cancel")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 60, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 60 requests per second
            CoinExRestSpotBatchCancel = new RateLimitGate("CoinEx Spot Batch Cancel")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 40, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 40 requests per second
            CoinExRestSpotQuery = new RateLimitGate("CoinEx Spot Order Query")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 50, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 50 requests per second
            CoinExRestSpotHistory = new RateLimitGate("CoinEx Spot Order History")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 10, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 10 requests per second
            CoinExRestSpotAccount = new RateLimitGate("CoinEx Spot Order Account")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 10, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 10 requests per second
            CoinExRestSpotAccountQuery = new RateLimitGate("CoinEx Spot Order Account")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 10, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 10 requests per second
            CoinExRestSpotAccountHistory = new RateLimitGate("CoinEx Spot Order Account")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 10, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 10 requests per second

            CoinExRestFuturesOrder = new RateLimitGate("CoinEx Futures Order")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 20, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 20 requests per second
            CoinExRestFuturesCancel = new RateLimitGate("CoinEx Futures Cancel")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 40, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 40 requests per second
            CoinExRestFuturesBatchCancel = new RateLimitGate("CoinEx Futures Batch Cancel")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 20, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 20 requests per second
            CoinExRestFuturesQuery = new RateLimitGate("CoinEx Futures Order Query")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 50, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 50 requests per second
            CoinExRestFuturesHistory = new RateLimitGate("CoinEx Futures Order History")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 10, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 10 requests per second
            CoinExRestFuturesAccount = new RateLimitGate("CoinEx Futures Order Account")
                .AddGuard(overallGuard)
                .AddGuard(new RateLimitGuard(RateLimitGuard.PerApiKey, [], 10, TimeSpan.FromSeconds(1), RateLimitWindowType.Sliding)); // 10 requests per second

            CoinExRestPublic.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestPublic.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestSpotOrder.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestSpotOrder.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestSpotCancel.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestSpotCancel.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestSpotBatchCancel.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestSpotBatchCancel.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestSpotQuery.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestSpotQuery.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestSpotHistory.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestSpotHistory.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestSpotAccount.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestSpotAccount.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestSpotAccountQuery.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestSpotAccountQuery.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestSpotAccountHistory.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestSpotAccountHistory.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);

            CoinExRestFuturesOrder.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestFuturesOrder.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestFuturesCancel.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestFuturesCancel.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestFuturesBatchCancel.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestFuturesBatchCancel.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestFuturesQuery.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestFuturesQuery.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestFuturesHistory.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestFuturesHistory.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
            CoinExRestFuturesAccount.RateLimitTriggered += (x) => RateLimitTriggered?.Invoke(x);
            CoinExRestFuturesAccount.RateLimitUpdated += (x) => RateLimitUpdated?.Invoke(x);
        }


        internal IRateLimitGate CoinExRestPublic { get; private set; }
        internal IRateLimitGate CoinExRestSpotOrder { get; private set; }
        internal IRateLimitGate CoinExRestSpotCancel { get; private set; }
        internal IRateLimitGate CoinExRestSpotBatchCancel { get; private set; }
        internal IRateLimitGate CoinExRestSpotQuery { get; private set; }
        internal IRateLimitGate CoinExRestSpotHistory { get; private set; }
        internal IRateLimitGate CoinExRestSpotAccount { get; private set; }
        internal IRateLimitGate CoinExRestSpotAccountQuery { get; private set; }
        internal IRateLimitGate CoinExRestSpotAccountHistory { get; private set; }

        internal IRateLimitGate CoinExRestFuturesOrder { get; private set; }
        internal IRateLimitGate CoinExRestFuturesCancel { get; private set; }
        internal IRateLimitGate CoinExRestFuturesBatchCancel { get; private set; }
        internal IRateLimitGate CoinExRestFuturesQuery { get; private set; }
        internal IRateLimitGate CoinExRestFuturesHistory { get; private set; }
        internal IRateLimitGate CoinExRestFuturesAccount { get; private set; }

    }
}
