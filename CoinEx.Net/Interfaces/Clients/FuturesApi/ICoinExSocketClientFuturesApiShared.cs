using CryptoExchange.Net.SharedApis;

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// Get the shared socket subscription client
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
}
