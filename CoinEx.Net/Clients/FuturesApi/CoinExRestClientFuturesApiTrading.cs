using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CryptoExchange.Net;
using CoinEx.Net.Interfaces.Clients.FuturesApi;

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc />
    public class CoinExRestClientFuturesApiTrading : ICoinExRestClientFuturesApiTrading
    {
        private readonly CoinExRestClientFuturesApi _baseClient;

        internal CoinExRestClientFuturesApiTrading(CoinExRestClientFuturesApi baseClient)
        {
            _baseClient = baseClient;
        }

    }
}
