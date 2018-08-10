namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketMarketDepth
    {
        /// <summary>
        /// The asks on the market
        /// </summary>
        public CoinExDepthEntry[] Asks { get; set; }
        /// <summary>
        /// The bids on the market
        /// </summary>
        public CoinExDepthEntry[] Bids { get; set; }
    }
}
