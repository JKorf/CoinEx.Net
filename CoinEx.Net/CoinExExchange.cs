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
    }
}
