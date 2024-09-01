using CryptoExchange.Net.SharedApis.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV2
{
    public interface ICoinExRestClientSpotApiShared :
        IAssetsRestClient,
        IBalanceRestClient,
        IDepositRestClient,
        IKlineRestClient,
        IOrderBookRestClient,
        IRecentTradeRestClient,
        ISpotOrderRestClient,
        ISpotSymbolRestClient,
        ITickerRestClient,
        //ITradeHistoryRestClient,
        IWithdrawalRestClient,
        IWithdrawRestClient
    {
    }
}
