﻿using CoinEx.Net.Clients;
using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Interfaces.CommonClients;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoExchange.Net.Clients
{
    /// <summary>
    /// Extensions for the ICryptoRestClient and ICryptoSocketClient interfaces
    /// </summary>
    public static class CryptoClientExtensions
    {
        /// <summary>
        /// Get the CoinEx REST Api client
        /// </summary>
        /// <param name="baseClient"></param>
        /// <returns></returns>
        public static ICoinExRestClient CoinEx(this ICryptoRestClient baseClient) => baseClient.TryGet<ICoinExRestClient>(() => new CoinExRestClient());

        /// <summary>
        /// Get the CoinEx Websocket Api client
        /// </summary>
        /// <param name="baseClient"></param>
        /// <returns></returns>
        public static ICoinExSocketClient CoinEx(this ICryptoSocketClient baseClient) => baseClient.TryGet<ICoinExSocketClient>(() => new CoinExSocketClient());
    }
}
