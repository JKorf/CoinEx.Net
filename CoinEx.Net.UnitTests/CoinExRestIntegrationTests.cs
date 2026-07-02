using CoinEx.Net.Clients;
using CoinEx.Net.Objects;
using CoinEx.Net.SymbolOrderBooks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinEx.Net.UnitTests
{
    [NonParallelizable]
    internal class CoinExRestIntegrationTests : RestIntegrationTest<CoinExRestClient>
    {
        public override bool Run { get; set; } = false;

        public CoinExRestIntegrationTests()
        {
        }

        public override CoinExRestClient GetClient(ILoggerFactory loggerFactory)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new CoinExRestClient(null, loggerFactory, Options.Create(new Objects.Options.CoinExRestOptions
            {
                OutputOriginalData = true,
                ApiCredentials = Authenticated ? new CoinExCredentials(key, sec) : null
            }));
        }

        [Test]
        public async Task TestErrorResponseParsing()
        {
            if (!ShouldRun())
                return;

            var result = await CreateClient().SpotApiV2.ExchangeData.GetTickersAsync(new[] { "TSTTST" }, default);

            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task TestSpotAccount()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetTradingFeesAsync("ETHUSDT", Enums.AccountType.Spot, default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetBalancesAsync(default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetMarginBalancesAsync(default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetFinancialBalancesAsync(default), true, "data");
            //await RunAndCheckResult(client => client.SpotApiV2.Account.GetCreditAccountAsync(default), true);
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetAutoMarketMakerAccountLiquidityAsync(default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetBorrowHistoryAsync(default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetBorrowLimitAsync("ETHUSDT", "USDT", default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetDepositHistoryAsync("USDT", default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetWithdrawalHistoryAsync(default, default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetDepositWithdrawalConfigAsync("ETH", default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Account.GetTransfersAsync("ETH", Enums.AccountType.Margin, default, default, default, default, default, default), true, "data");
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestSpotExchangeData()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetServerTimeAsync(default), false);
            await RunAndCheckResult(warnings, client => client.SpotApiV2.ExchangeData.GetSymbolsAsync(default), false, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.ExchangeData.GetAssetsAsync(default), false, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.ExchangeData.GetTickersAsync(default, default), false, "data", ignoreProperties: ["period"]);
            await RunAndCheckResult(warnings, client => client.SpotApiV2.ExchangeData.GetOrderBookAsync("ETHUSDT", 5, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.ExchangeData.GetTradeHistoryAsync("ETHUSDT", default, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.ExchangeData.GetKlinesAsync("ETHUSDT", Enums.KlineInterval.OneDay, default, default, default, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.ExchangeData.GetIndexPricesAsync(default, default), false, "data");
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestSpotTrading()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Trading.GetOpenOrdersAsync(Enums.AccountType.Spot, default, default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Trading.GetClosedOrdersAsync(Enums.AccountType.Spot, default, default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Trading.GetOpenStopOrdersAsync(Enums.AccountType.Spot, default, default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Trading.GetClosedStopOrdersAsync(Enums.AccountType.Spot, default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.SpotApiV2.Trading.GetUserTradesAsync("ETHUSDT", Enums.AccountType.Spot, default, default, default, default, default, default), true, "data");
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestFuturesAccount()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.FuturesApi.Account.GetTradingFeesAsync("ETHUSDT", default), true, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.Account.GetBalancesAsync(default), true, "data");
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestFuturesExchangeData()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetServerTimeAsync(default), false);
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetSymbolsAsync(default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetTickersAsync(default, default), false, "data", ignoreProperties: ["period"]);
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetOrderBookAsync("ETHUSDT", 5, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetTradeHistoryAsync("ETHUSDT", default, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetKlinesAsync("ETHUSDT", Enums.KlineInterval.OneDay, default, default, default, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetIndexPricesAsync(default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetFundingRatesAsync(default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetFundingRateHistoryAsync("ETHUSDT", default, default, default, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetPremiumIndexPriceHistoryAsync("ETHUSDT", default, default, default, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetPositionLevelsAsync(default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetLiquidationHistoryAsync("ETHUSDT", default, default, default, default, default), false, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.ExchangeData.GetBasisHistoryAsync("ETHUSDT", default, default, default), false, "data");
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestFuturesTrading()
        {
            var warnings = new List<Exception>();
            await RunAndCheckResult(warnings, client => client.FuturesApi.Trading.GetOpenOrdersAsync(default, default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.Trading.GetClosedOrdersAsync(default, default, default, default, default, default), true, "data");  
            await RunAndCheckResult(warnings, client => client.FuturesApi.Trading.GetOpenStopOrdersAsync(default, default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.Trading.GetClosedStopOrdersAsync(default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.Trading.GetUserTradesAsync("ETHUSDT", default, default, default, default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.Trading.GetPositionsAsync("ETHUSDT", default, default, default), true, "data");
            await RunAndCheckResult(warnings, client => client.FuturesApi.Trading.GetPositionHistoryAsync("ETHUSDT", default, default, default, default, default), true, "data");
            foreach (var warning in warnings)
                Assert.Warn(warning.Message);
        }

        [Test]
        public async Task TestOrderBooks()
        {
            await TestOrderBook(new CoinExSpotSymbolOrderBook("ETHUSDT"));
            await TestOrderBook(new CoinExFuturesSymbolOrderBook("ETHUSDT"));
        }
    }
}
