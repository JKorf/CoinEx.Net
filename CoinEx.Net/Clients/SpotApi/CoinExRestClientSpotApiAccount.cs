using CoinEx.Net.Converters;
using CoinEx.Net.Enums;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.ExtensionMethods;

namespace CoinEx.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class CoinExRestClientSpotApiAccount : ICoinExClientSpotApiAccount
    {
        private readonly CoinExRestClientSpotApi _baseClient;

        internal CoinExRestClientSpotApiAccount(CoinExRestClientSpotApi baseClient)
        {
            _baseClient = baseClient;
        }


    }
}
