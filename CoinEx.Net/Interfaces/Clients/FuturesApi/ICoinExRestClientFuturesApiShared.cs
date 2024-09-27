using CryptoExchange.Net.SharedApis;

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// Shared interface for Futures rest API usage
    /// </summary>
    public interface ICoinExRestClientFuturesApiShared :
        IBalanceRestClient,
        IFuturesTickerRestClient,
        IFuturesSymbolRestClient,
        IFuturesOrderRestClient,
        IKlineRestClient,
        IMarkPriceKlineRestClient,
        IIndexPriceKlineRestClient,
        IRecentTradeRestClient,
        ILeverageRestClient,
        IOrderBookRestClient,
        IOpenInterestRestClient,
        IFundingRateRestClient,
        IPositionHistoryRestClient
    {
    }
}
