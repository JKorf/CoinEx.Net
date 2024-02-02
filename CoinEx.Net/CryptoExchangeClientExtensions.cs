using CoinEx.Net.Clients;
using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Interfaces.CommonClients;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoExchange.Net.Clients
{
    public static class CryptoExchangeClientExtensions
    {
        public static ICoinExRestClient CoinEx(this ICryptoExchangeClient baseClient) => baseClient.TryGet<ICoinExRestClient>() ?? new CoinExRestClient();
    }
}
