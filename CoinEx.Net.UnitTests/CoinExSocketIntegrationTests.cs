using CoinEx.Net.Clients;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CoinEx.Net.UnitTests
{
    [NonParallelizable]
    internal class CoinExSocketIntegrationTests : SocketIntegrationTest<CoinExSocketClient>
    {
        public override bool Run { get; set; } = false;

        public CoinExSocketIntegrationTests()
        {
        }

        public override CoinExSocketClient GetClient(ILoggerFactory loggerFactory, bool useUpdatedDeserialization)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new CoinExSocketClient(Options.Create(new CoinExSocketOptions
            {
                OutputOriginalData = true,
                UseUpdatedDeserialization = useUpdatedDeserialization,
                ApiCredentials = Authenticated ? new CryptoExchange.Net.Authentication.ApiCredentials(key, sec) : null
            }), loggerFactory);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task TestSubscriptions(bool useUpdatedDeserialization)
        {
            await RunAndCheckUpdate<CoinExTicker>(useUpdatedDeserialization , (client, updateHandler) => client.SpotApiV2.SubscribeToBalanceUpdatesAsync(default , default), false, true);
            await RunAndCheckUpdate<CoinExTicker[]>(useUpdatedDeserialization, (client, updateHandler) => client.SpotApiV2.SubscribeToTickerUpdatesAsync(new[] { "ETHUSDT" }, updateHandler, default), true, false);

            await RunAndCheckUpdate<CoinExTicker>(useUpdatedDeserialization, (client, updateHandler) => client.FuturesApi.SubscribeToBalanceUpdatesAsync(default, default), false, true);
            await RunAndCheckUpdate<CoinExFuturesTickerUpdate[]>(useUpdatedDeserialization, (client, updateHandler) => client.FuturesApi.SubscribeToTickerUpdatesAsync(new[] { "ETHUSDT" }, updateHandler, default), true, false);
        } 
    }
}
