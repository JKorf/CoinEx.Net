using CryptoExchange.Net.SharedApis;
using System;

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
        /// Format a base and quote asset to a CoinEx recognized symbol 
        /// </summary>
        /// <param name="baseAsset">Base asset</param>
        /// <param name="quoteAsset">Quote asset</param>
        /// <param name="tradingMode">Trading mode</param>
        /// <param name="deliverTime">Delivery time for delivery futures</param>
        /// <returns></returns>
        public static string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
        {
            return $"{baseAsset.ToUpperInvariant()}{quoteAsset.ToUpperInvariant()}";
        }
    }
}
