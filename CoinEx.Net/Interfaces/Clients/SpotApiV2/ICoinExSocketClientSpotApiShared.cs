using CryptoExchange.Net.SharedApis.Interfaces.Socket;
using CryptoExchange.Net.SharedApis.Interfaces.Socket.Spot;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV2
{
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
