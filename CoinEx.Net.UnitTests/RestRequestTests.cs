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
        public async Task ValidateFuturesAccountCalls()
        {
            var client = new CoinExRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new ApiCredentials("1", "2");
            });
            var tester = new RestRequestValidator<CoinExRestClient>(client, "Endpoints/FuturesApi/Account", "https://api.coinex.com", IsAuthenticated, nestedPropertyForCompare: "data", stjCompare: true);
            await tester.ValidateAsync(client => client.FuturesApi.Account.GetBalancesAsync(), "GetBalances");
            await tester.ValidateAsync(client => client.FuturesApi.Account.SetLeverageAsync("ETHUSDT", Enums.MarginMode.Isolated, 1), "SetLeverage");
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

        [Test]
        public async Task ValidateFuturesTradingCalls()
        {
            var client = new CoinExRestClient(opts =>
            {
                opts.AutoTimestamp = false;
                opts.ApiCredentials = new ApiCredentials("1", "2");
            });
            var tester = new RestRequestValidator<CoinExRestClient>(client, "Endpoints/FuturesApi/Trading", "https://api.coinex.com", IsAuthenticated, nestedPropertyForCompare: "data", stjCompare: true);
            await tester.ValidateAsync(client => client.FuturesApi.Trading.PlaceOrderAsync("ETHUSDT", Enums.OrderSide.Buy, Enums.OrderTypeV2.Limit, 1), "PlaceOrder");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.PlaceStopOrderAsync("ETHUSDT", Enums.OrderSide.Buy, Enums.OrderTypeV2.Limit, 1, 1, Enums.TriggerPriceType.LastPrice), "PlaceStopOrder");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.PlaceMultipleOrdersAsync([new CoinExFuturesPlaceOrderRequest()]), "PlaceMultipleOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.PlaceMultipleStopOrdersAsync([new CoinExFuturesPlaceStopOrderRequest()]), "PlaceMultipleStopOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetOrderAsync("ETHUSDT", 1), "GetOrder");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetOpenOrdersAsync("ETHUSDT"), "GetOpenOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetClosedOrdersAsync("ETHUSDT"), "GetClosedOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetOpenStopOrdersAsync("ETHUSDT"), "GetOpenStopOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetClosedStopOrdersAsync("ETHUSDT"), "GetClosedStopOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.EditOrderAsync("ETHUSDT", 1, 1), "EditOrder");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.EditStopOrderAsync("ETHUSDT", 1, 1, 1), "EditStopOrder");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.CancelAllOrdersAsync("ETHUSDT"), "CancelAllOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.CancelOrderAsync("ETHUSDT", 1), "CancelOrder");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.CancelStopOrderAsync("ETHUSDT", 1), "CancelStopOrder");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.CancelOrderByClientOrderIdAsync("ETHUSDT", "1"), "CancelOrderByClientOrderId");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.CancelStopOrderByClientOrderIdAsync("ETHUSDT", "1"), "CancelStopOrderByClientOrderId");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.CancelOrdersAsync("ETHUSDT", [1]), "CancelOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.CancelStopOrdersAsync("ETHUSDT", [1]), "CancelStopOrders");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetUserTradesAsync("ETHUSDT"), "GetUserTrades");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetOrderTradesAsync("ETHUSDT", 1), "GetOrderTrades");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetPositionsAsync("ETHUSDT"), "GetPositions");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetPositionHistoryAsync("ETHUSDT"), "GetPositionHistory");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.ClosePositionAsync("ETHUSDT", Enums.OrderTypeV2.Market), "ClosePosition");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.AdjustPositionMarginAsync("ETHUSDT", 1), "AdjustPositionMargin");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.SetStopLossAsync("ETHUSDT", Enums.PriceType.LastPrice, 1), "SetStopLoss");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.SetTakeProfitAsync("ETHUSDT", Enums.PriceType.LastPrice, 1), "SetTakeProfit", ignoreProperties: ["base_fee", "quote_fee", "discount_fee", "maker_fee_rate", "taker_fee_rate"]);
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetMarginHistoryAsync("ETHUSDT", 1), "GetMarginHistory");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetFundingRateHistoryAsync("ETHUSDT", 1), "GetFundingRateHistory");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetAutoDeleverageHistoryAsync("ETHUSDT", 1), "GetAutoDeleverageHistory");
            await tester.ValidateAsync(client => client.FuturesApi.Trading.GetAutoSettlementHistoryAsync("ETHUSDT", 1), "GetAutoSettlementHistory");
        }

        private bool IsAuthenticated(WebCallResult result)
        {
            return result.RequestHeaders.Any(x => x.Key == "X-COINEX-KEY");
        }
    }
}
