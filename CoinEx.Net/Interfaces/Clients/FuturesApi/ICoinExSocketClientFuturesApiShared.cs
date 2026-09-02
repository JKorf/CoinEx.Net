using CryptoExchange.Net.SharedApis;

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// Get the shared socket subscription client. This interface is shared with other exhanges to allow for a common implementation for different exchanges.
    /// </summary>
    public interface ICoinExSocketClientFuturesApiShared :
        ITickerSocketClient,
        ITickersSocketClient,
        ITradeSocketClient,
        IBookTickerSocketClient,
        IOrderBookSocketClient,
        IBalanceSocketClient,
        IFuturesOrderSocketClient,
        IUserTradeSocketClient,
        IPositionSocketClient
    {
    }

    /// <summary>
    /// Shared API interface. Shared APIs provide a common,
    /// exchange-independent contract for accessing functionality across different
    /// exchange client libraries.
    /// </summary>
    public interface ICoinExSocketClientFuturesSharedApi :
        ISubscribeAllTickersSocket,
        ISubscribeTickerSocket,
        ISubscribeTradesSocket,
        ISubscribeBookTickerSocket,
        ISubscribeOrderBookSocket,
        ISubscribeBalancesSocket,
        ISubscribeFuturesOrdersSocket,
        ISubscribeUserTradesSocket,
        ISubscribePositionsSocket
    { }
}
