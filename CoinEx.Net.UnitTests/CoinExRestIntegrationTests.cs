using CoinEx.Net.Clients;
using CoinEx.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinEx.Net.UnitTests
{
    [NonParallelizable]
    internal class CoinExRestIntegrationTests : RestIntergrationTest<CoinExRestClient>
    {
        public override bool Run { get; set; }

        public CoinExRestIntegrationTests()
        {
        }

        public override CoinExRestClient GetClient(ILoggerFactory loggerFactory)
        {
            var key = Environment.GetEnvironmentVariable("APIKEY");
            var sec = Environment.GetEnvironmentVariable("APISECRET");

            Authenticated = key != null && sec != null;
            return new CoinExRestClient(null, loggerFactory, opts =>
            {
                opts.OutputOriginalData = true;
                opts.ApiCredentials = Authenticated ? new ApiCredentials(key, sec) : null;
            });
        }

        [Test]
        public async Task TestErrorResponseParsing()
        {
            if (!ShouldRun())
                return;

            var result = await CreateClient().SpotApiV2.ExchangeData.GetTickersAsync(new[] { "TSTTST" }, default);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo(3639));
        }

        [Test]
        public async Task TestSpotAccount()
        {
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetTradingFeesAsync("ETHUSDT", Enums.AccountType.Spot, default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetBalancesAsync(default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetMarginBalancesAsync(default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetFinancialBalancesAsync(default), true);
            //await RunAndCheckResult(client => client.SpotApiV2.Account.GetCreditAccountAsync(default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetAutoMarketMakerAccountLiquidityAsync(default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetBorrowHistoryAsync(default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetBorrowLimitAsync("ETHUSDT", "USDT", default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetDepositHistoryAsync("USDT", default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetWithdrawalHistoryAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetDepositWithdrawalConfigAsync("ETH", default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Account.GetTransfersAsync("ETH", Enums.AccountType.Margin, default, default, default, default, default, default, default), true);
        }

        [Test]
        public async Task TestSpotExchangeData()
        {
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetServerTimeAsync(default), false);
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetSymbolsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetAssetsAsync(default), false);
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetTickersAsync(default, default), false);
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetOrderBookAsync("ETHUSDT", 5, default, default), false);
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetTradeHistoryAsync("ETHUSDT", default, default, default), false);
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetKlinesAsync("ETHUSDT", Enums.KlineInterval.OneDay, default, default, default), false);
            await RunAndCheckResult(client => client.SpotApiV2.ExchangeData.GetIndexPricesAsync(default, default), false);
        }

        [Test]
        public async Task TestSpotTrading()
        {
            await RunAndCheckResult(client => client.SpotApiV2.Trading.GetOpenOrdersAsync(Enums.AccountType.Spot, default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Trading.GetClosedOrdersAsync(Enums.AccountType.Spot, default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Trading.GetOpenStopOrdersAsync(Enums.AccountType.Spot, default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Trading.GetClosedStopOrdersAsync(Enums.AccountType.Spot, default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.SpotApiV2.Trading.GetUserTradesAsync("ETHUSDT", Enums.AccountType.Spot, default, default, default, default, default, default), true);
        }

        [Test]
        public async Task TestFuturesAccount()
        {
            await RunAndCheckResult(client => client.FuturesApi.Account.GetTradingFeesAsync("ETHUSDT", default), true);
            await RunAndCheckResult(client => client.FuturesApi.Account.GetBalancesAsync(default), true);
        }

        [Test]
        public async Task TestFuturesExchangeData()
        {
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetServerTimeAsync(default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetSymbolsAsync(default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetTickersAsync(default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetOrderBookAsync("ETHUSDT", 5, default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetTradeHistoryAsync("ETHUSDT", default, default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetKlinesAsync("ETHUSDT", Enums.KlineInterval.OneDay, default, default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetIndexPricesAsync(default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetFundingRatesAsync(default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetFundingRateHistoryAsync("ETHUSDT", default, default, default, default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetPremiumIndexPriceHistoryAsync("ETHUSDT", default, default, default, default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetPositionLevelsAsync(default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetLiquidationHistoryAsync("ETHUSDT", default, default, default, default, default), false);
            await RunAndCheckResult(client => client.FuturesApi.ExchangeData.GetBasisHistoryAsync("ETHUSDT", default, default, default), false);
        }

        [Test]
        public async Task TestFuturesTrading()
        {
            await RunAndCheckResult(client => client.FuturesApi.Trading.GetOpenOrdersAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.FuturesApi.Trading.GetClosedOrdersAsync(default, default, default, default, default, default), true);  
            await RunAndCheckResult(client => client.FuturesApi.Trading.GetOpenStopOrdersAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.FuturesApi.Trading.GetClosedStopOrdersAsync(default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.FuturesApi.Trading.GetUserTradesAsync("ETHUSDT", default, default, default, default, default, default), true);
            await RunAndCheckResult(client => client.FuturesApi.Trading.GetPositionsAsync("ETHUSDT", default, default, default), true);
            await RunAndCheckResult(client => client.FuturesApi.Trading.GetPositionHistoryAsync("ETHUSDT", default, default, default, default, default), true);
        }
    }
}
