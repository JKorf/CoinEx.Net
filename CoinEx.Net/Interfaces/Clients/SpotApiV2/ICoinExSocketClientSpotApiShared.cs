using CryptoExchange.Net.SharedApis;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV2
{
    /// <summary>
    /// Get the shared socket subscription client. This interface is shared with other exhanges to allow for a common implementation for different exchanges.
    /// </summary>
    public interface ICoinExSocketClientSpotApiShared :
        ITickerSocketClient,
        ITickersSocketClient,
        ITradeSocketClient,
        IBookTickerSocketClient,
        IOrderBookSocketClient,
        IBalanceSocketClient,
        ISpotOrderSocketClient,
        IUserTradeSocketClient
    {
    }
}
