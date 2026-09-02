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
        IPositionHistoryRestClient,
        IFeeRestClient,
        IFuturesOrderClientIdRestClient,
        IFuturesTriggerOrderRestClient,
        IFuturesTpSlRestClient,
        IBookTickerRestClient
    {
    }

    /// <summary>
    /// Shared API interface. Shared APIs provide a common,
    /// exchange-independent contract for accessing functionality across different
    /// exchange client libraries.
    /// </summary>
    public interface ICoinExRestClientFuturesSharedApi :
        IGetBalancesRest,
        IGetFuturesTickerRest,
        IGetAllFuturesTickersRest,
        IGetFuturesSymbolsRest,
        IPlaceFuturesOrderRest,
        IGetFuturesOrderRest,
        IGetOpenFuturesOrdersRest,
        IGetClosedFuturesOrdersRest,
        IGetFuturesOrderTradesRest,
        IGetFuturesUserTradeHistoryRest,
        ICancelFuturesOrderRest,
        IGetPositionsRest,
        IClosePositionRest,
        IGetKlinesRest,
        IGetMarkPriceKlinesRest,
        IGetIndexPriceKlinesRest,
        IGetRecentTradesRest,
        IGetLeverageRest,
        ISetLeverageRest,
        IGetOrderBookRest,
        IGetOpenInterestRest,
        IGetFundingRateHistoryRest,
        IGetPositionHistoryRest,
        IGetFeesRest,
        IGetFuturesOrderByClientOrderIdRest,
        ICancelFuturesOrderByClientOrderIdRest,
        IPlaceFuturesTriggerOrderRest,
        IGetFuturesTriggerOrderRest,
        ICancelFuturesTriggerOrderRest,
        ISetFuturesTpSlRest,
        ICancelFuturesTpSlRest,
        IGetBookTickerRest
    { }
}
