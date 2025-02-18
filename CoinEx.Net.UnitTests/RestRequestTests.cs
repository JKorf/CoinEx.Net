using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoExchange.Net.Authentication;
using System.Linq;
using CoinEx.Net.Clients;
using CoinEx.Net.Objects.Models.V2;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class RestRequestTests
    {
        [Test]
        public async Task ValidateSpotAccountCalls()
        {
            var client = new CoinExRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new ApiCredentials("1", "2");
            });
            var tester = new RestRequestValidator<CoinExRestClient>(client, "Endpoints/SpotApi/Account", "https://api.coinex.com", IsAuthenticated, nestedPropertyForCompare: "data", stjCompare: true);
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetTradingFeesAsync("ETHUSDT", Enums.AccountType.Spot), "GetTradingFees");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.SetAccountConfigAsync(true), "SetAccountConfig");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetBalancesAsync(), "GetBalances");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetMarginBalancesAsync(), "GetMarginBalances");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetFinancialBalancesAsync(), "GetFinancialBalances");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetCreditAccountAsync(), "GetCreditAccount");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetAutoMarketMakerAccountLiquidityAsync(), "GetAutoMarketMakerAccountLiquidity");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.MarginBorrowAsync("ETHUSDT", "ETH", 1, true), "MarginBorrow");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.MarginRepayAsync("ETHUSDT", "ETH", 1), "MarginRepay");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetBorrowHistoryAsync("ETHUSDT"), "GetBorrowHistory");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetBorrowLimitAsync("ETHUSDT", "ETH"), "GetBorrowLimit");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetDepositAddressAsync("ETH", "ETH"), "GetDepositAddress");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.RenewDepositAddressAsync("ETH", "ETH"), "RenewDepositAddress");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetDepositHistoryAsync("ETHUSDT"), "GetDepositHistory");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.WithdrawAsync("ETH", 1, ""), "Withdraw");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.CancelWithdrawalAsync(1), "CancelWithdrawal");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetWithdrawalHistoryAsync(), "GetWithdrawalHistory");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetDepositWithdrawalConfigAsync("ETH"), "GetDepositWithdrawalConfig");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.TransferAsync("ETH", Enums.AccountType.Spot, Enums.AccountType.Futures, 1), "Transfer");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetTransfersAsync("ETH", Enums.AccountType.Spot), "GetTransfers");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.AddAutoMarketMakerLiquidityAsync("ETHUSDT", 1, 1), "AddAutoMarketMakerLiquidity");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.RemoveAutoMarketMakerLiquidityAsync("ETHUSDT"), "RemoveAutoMarketMakerLiquidity");
            await tester.ValidateAsync(client => client.SpotApiV2.Account.GetTransactionHistoryAsync(Enums.TransactionType.Withdrawal), "GetTransactionHistory");
        }

        [Test]
        public async Task ValidateSpotExchangeDataCalls()
        {
            var client = new CoinExRestClient(opts =>
            {
                opts.AutoTimestamp = false;
            });
            var tester = new RestRequestValidator<CoinExRestClient>(client, "Endpoints/SpotApi/ExchangeData", "https://api.coinex.com", IsAuthenticated, nestedPropertyForCompare: "data", stjCompare: true);
            await tester.ValidateAsync(client => client.SpotApiV2.ExchangeData.GetSymbolsAsync(), "GetSymbols");
            await tester.ValidateAsync(client => client.SpotApiV2.ExchangeData.GetAssetsAsync(), "GetAssets");
            await tester.ValidateAsync(client => client.SpotApiV2.ExchangeData.GetTickersAsync(), "GetTickers", ignoreProperties: ["period"]);
            await tester.ValidateAsync(client => client.SpotApiV2.ExchangeData.GetOrderBookAsync("ETHUSDT", 5), "GetOrderBook");
            await tester.ValidateAsync(client => client.SpotApiV2.ExchangeData.GetTradeHistoryAsync("ETHUSDT"), "GetTradeHistory");
            await tester.ValidateAsync(client => client.SpotApiV2.ExchangeData.GetKlinesAsync("ETHUSDT", Enums.KlineInterval.OneDay), "GetKlines");
            await tester.ValidateAsync(client => client.SpotApiV2.ExchangeData.GetIndexPricesAsync(), "GetIndexPrices");
        }

        [Test]
        public async Task ValidateSpotTradingCalls()
        {
            var client = new CoinExRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new ApiCredentials("1", "2");
            });
            var tester = new RestRequestValidator<CoinExRestClient>(client, "Endpoints/SpotApi/Trading", "https://api.coinex.com", IsAuthenticated, nestedPropertyForCompare: "data", stjCompare: true);
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.PlaceOrderAsync("ETHUSDT", Enums.AccountType.Spot, Enums.OrderSide.Buy, Enums.OrderTypeV2.Limit, 1), "PlaceOrder");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.PlaceStopOrderAsync("ETHUSDT", Enums.AccountType.Spot, Enums.OrderSide.Buy, Enums.OrderTypeV2.Limit, 1, 1), "PlaceStopOrder");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.PlaceMultipleOrdersAsync([new CoinExPlaceOrderRequest()]), "PlaceMultipleOrders", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.PlaceMultipleStopOrdersAsync([new CoinExPlaceStopOrderRequest()]), "PlaceMultipleStopOrders");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.GetOrderAsync("ETHUSDT", 1), "GetOrder");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.GetOpenOrdersAsync(Enums.AccountType.Spot), "GetOpenOrders");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.GetClosedOrdersAsync(Enums.AccountType.Spot), "GetClosedOrders");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.GetOpenStopOrdersAsync(Enums.AccountType.Spot), "GetOpenStopOrders");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.GetClosedStopOrdersAsync(Enums.AccountType.Spot), "GetClosedStopOrders");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.EditOrderAsync("ETHUSDT", Enums.AccountType.Spot, 1, 1), "EditOrder");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.EditStopOrderAsync("ETHUSDT", Enums.AccountType.Spot, 1, 1, 1), "EditStopOrder");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.CancelAllOrdersAsync("ETHUSDT", Enums.AccountType.Spot), "CancelAllOrders");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.CancelOrderAsync("ETHUSDT", Enums.AccountType.Spot, 1), "CancelOrder");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.CancelOrdersAsync("ETHUSDT", [1]), "CancelOrders");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.CancelStopOrderAsync("ETHUSDT", Enums.AccountType.Spot, 1), "CancelStopOrder");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.CancelOrdersByClientOrderIdAsync("ETHUSDT", Enums.AccountType.Spot, "1"), "CancelOrdersByClientOrderId", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.CancelStopOrdersByClientOrderIdAsync("ETHUSDT", Enums.AccountType.Spot, "1"), "CancelStopOrdersByClientOrderId", skipResponseValidation: true);
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.GetUserTradesAsync("ETHUSDT", Enums.AccountType.Spot), "GetUserTrades");
            await tester.ValidateAsync(client => client.SpotApiV2.Trading.GetOrderTradesAsync("ETHUSDT", Enums.AccountType.Spot, 1), "GetOrderTrades");
        }


        [Test]
        public async Task ValidateFuturesExchangeDataCalls()
        {
            var client = new CoinExRestClient(opts =>
            {
                opts.AutoTimestamp = false;
            });
            var tester = new RestRequestValidator<CoinExRestClient>(client, "Endpoints/FuturesApi/ExchangeData", "https://api.coinex.com", IsAuthenticated, nestedPropertyForCompare: "data", stjCompare: true);
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetSymbolsAsync(), "GetSymbols");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetTickersAsync(), "GetTickers", ignoreProperties: ["period"]);
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetOrderBookAsync("ETHUSDT", 5), "GetOrderBook");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetTradeHistoryAsync("ETHUSDT"), "GetTradeHistory");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetKlinesAsync("ETHUSDT", Enums.KlineInterval.OneDay), "GetKlines");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetIndexPricesAsync(), "GetIndexPrices");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetFundingRatesAsync(), "GetFundingRates");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetFundingRateHistoryAsync("ETHUSDT"), "GetFundingRateHistory");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetPremiumIndexPriceHistoryAsync("ETHUSDT"), "GetPremiumIndexPriceHistory");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetPositionLevelsAsync(), "GetPositionLevels");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetLiquidationHistoryAsync("ETHUSDT"), "GetLiquidationHistory");
            await tester.ValidateAsync(client => client.FuturesApi.ExchangeData.GetBasisHistoryAsync("ETHUSDT"), "GetBasisHistory");
        }

        private bool IsAuthenticated(WebCallResult result)
        {
            return result.RequestHeaders.Any(x => x.Key == "X-COINEX-KEY");
        }
    }
}
