# CoinEx.Net AI API Quick Map

Use this file to route common user intents to the correct CoinEx.Net client member. If a method name or parameter is not listed here, inspect `CoinEx.Net/Interfaces/Clients/**` before generating code.

## Client Roots

| Intent | Use |
|---|---|
| REST calls | `new CoinExRestClient()` |
| WebSocket streams | `new CoinExSocketClient()` |
| API key authentication | `options.ApiCredentials = new CoinExCredentials("key", "secret")` |
| Live environment | `CoinExEnvironment.Live` |
| Custom environment | `CoinExEnvironment.CreateCustom(name, restAddress, socketAddress)` |
| Dependency injection | `services.AddCoinEx(options => { ... })` |
| Current spot REST API | `client.SpotApiV2` |
| Futures REST API | `client.FuturesApi` |

## Spot V2 REST

| User intent | CoinEx.Net member |
|---|---|
| Get server time | `client.SpotApiV2.ExchangeData.GetServerTimeAsync()` |
| Get spot symbols | `client.SpotApiV2.ExchangeData.GetSymbolsAsync()` |
| Get listed assets | `client.SpotApiV2.ExchangeData.GetAssetsAsync()` |
| Get one or more spot tickers | `client.SpotApiV2.ExchangeData.GetTickersAsync(new[] { "BTCUSDT" })` |
| Get all spot tickers | `client.SpotApiV2.ExchangeData.GetTickersAsync()` |
| Get spot order book | `client.SpotApiV2.ExchangeData.GetOrderBookAsync("BTCUSDT", 20, mergeLevel: "0")` |
| Get recent spot trades | `client.SpotApiV2.ExchangeData.GetTradeHistoryAsync("BTCUSDT")` |
| Get spot klines/candles | `client.SpotApiV2.ExchangeData.GetKlinesAsync("BTCUSDT", KlineInterval.OneMinute)` |
| Get spot index prices | `client.SpotApiV2.ExchangeData.GetIndexPricesAsync(new[] { "BTCUSDT" })` |
| Get spot balances | `client.SpotApiV2.Account.GetBalancesAsync()` |
| Get margin balances | `client.SpotApiV2.Account.GetMarginBalancesAsync()` |
| Get financial balances | `client.SpotApiV2.Account.GetFinancialBalancesAsync()` |
| Get credit account | `client.SpotApiV2.Account.GetCreditAccountAsync()` |
| Get trade fees | `client.SpotApiV2.Account.GetTradingFeesAsync("BTCUSDT", AccountType.Spot)` |
| Update CET discount setting | `client.SpotApiV2.Account.SetAccountConfigAsync(cetDiscountEnabled)` |
| Borrow margin asset | `client.SpotApiV2.Account.MarginBorrowAsync("BTCUSDT", "USDT", quantity, autoRenew)` |
| Repay margin loan | `client.SpotApiV2.Account.MarginRepayAsync("BTCUSDT", "USDT", quantity, borrowId)` |
| Get borrow history | `client.SpotApiV2.Account.GetBorrowHistoryAsync(...)` |
| Get borrow limit | `client.SpotApiV2.Account.GetBorrowLimitAsync("BTCUSDT", "USDT")` |
| Get deposit address | `client.SpotApiV2.Account.GetDepositAddressAsync("USDT", "TRC20")` |
| Renew deposit address | `client.SpotApiV2.Account.RenewDepositAddressAsync("USDT", "TRC20")` |
| Get deposit history | `client.SpotApiV2.Account.GetDepositHistoryAsync(...)` |
| Withdraw asset | `client.SpotApiV2.Account.WithdrawAsync(...)` |
| Cancel withdrawal | `client.SpotApiV2.Account.CancelWithdrawalAsync(withdrawalId)` |
| Get withdrawal history | `client.SpotApiV2.Account.GetWithdrawalHistoryAsync(...)` |
| Get deposit/withdraw config | `client.SpotApiV2.Account.GetDepositWithdrawalConfigAsync("USDT")` |
| Get all deposit/withdraw configs | `client.SpotApiV2.Account.GetAllDepositWithdrawalConfigsAsync()` |
| Transfer between accounts | `client.SpotApiV2.Account.TransferAsync("USDT", AccountType.Spot, AccountType.Futures, quantity)` |
| Get transfer history | `client.SpotApiV2.Account.GetTransfersAsync("USDT", AccountType.Futures)` |
| Add AMM liquidity | `client.SpotApiV2.Account.AddAutoMarketMakerLiquidityAsync(...)` |
| Remove AMM liquidity | `client.SpotApiV2.Account.RemoveAutoMarketMakerLiquidityAsync(symbol)` |
| Get transaction history | `client.SpotApiV2.Account.GetTransactionHistoryAsync(transactionType, ...)` |
| Place spot order | `client.SpotApiV2.Trading.PlaceOrderAsync("BTCUSDT", AccountType.Spot, OrderSide.Buy, OrderTypeV2.Limit, quantity, price)` |
| Place margin order | `client.SpotApiV2.Trading.PlaceOrderAsync("BTCUSDT", AccountType.Margin, ...)` |
| Place spot stop order | `client.SpotApiV2.Trading.PlaceStopOrderAsync(...)` |
| Place multiple spot orders | `client.SpotApiV2.Trading.PlaceMultipleOrdersAsync(requests)` |
| Query spot order | `client.SpotApiV2.Trading.GetOrderAsync("BTCUSDT", orderId)` |
| Get open spot orders | `client.SpotApiV2.Trading.GetOpenOrdersAsync(AccountType.Spot, symbol: "BTCUSDT")` |
| Get closed spot orders | `client.SpotApiV2.Trading.GetClosedOrdersAsync(AccountType.Spot, symbol: "BTCUSDT")` |
| Get open stop orders | `client.SpotApiV2.Trading.GetOpenStopOrdersAsync(AccountType.Spot, symbol: "BTCUSDT")` |
| Get closed stop orders | `client.SpotApiV2.Trading.GetClosedStopOrdersAsync(AccountType.Spot, symbol: "BTCUSDT")` |
| Edit spot order | `client.SpotApiV2.Trading.EditOrderAsync("BTCUSDT", AccountType.Spot, orderId, quantity, price)` |
| Edit spot stop order | `client.SpotApiV2.Trading.EditStopOrderAsync(...)` |
| Cancel all spot orders | `client.SpotApiV2.Trading.CancelAllOrdersAsync("BTCUSDT", AccountType.Spot)` |
| Cancel spot order | `client.SpotApiV2.Trading.CancelOrderAsync("BTCUSDT", AccountType.Spot, orderId)` |
| Cancel multiple spot orders | `client.SpotApiV2.Trading.CancelOrdersAsync("BTCUSDT", orderIds)` |
| Cancel spot order by client id | `client.SpotApiV2.Trading.CancelOrdersByClientOrderIdAsync("BTCUSDT", AccountType.Spot, clientOrderId)` |
| Get user trades | `client.SpotApiV2.Trading.GetUserTradesAsync("BTCUSDT", AccountType.Spot)` |
| Get order trades | `client.SpotApiV2.Trading.GetOrderTradesAsync("BTCUSDT", AccountType.Spot, orderId)` |

## Futures REST

| User intent | CoinEx.Net member |
|---|---|
| Get futures server time | `client.FuturesApi.ExchangeData.GetServerTimeAsync()` |
| Get futures symbols | `client.FuturesApi.ExchangeData.GetSymbolsAsync(new[] { "ETHUSDT" })` |
| Get futures tickers | `client.FuturesApi.ExchangeData.GetTickersAsync(new[] { "ETHUSDT" })` |
| Get all futures tickers | `client.FuturesApi.ExchangeData.GetTickersAsync()` |
| Get futures order book | `client.FuturesApi.ExchangeData.GetOrderBookAsync("ETHUSDT", 20, mergeLevel: "0")` |
| Get futures trades | `client.FuturesApi.ExchangeData.GetTradeHistoryAsync("ETHUSDT")` |
| Get futures klines | `client.FuturesApi.ExchangeData.GetKlinesAsync("ETHUSDT", KlineInterval.OneMinute)` |
| Get futures index prices | `client.FuturesApi.ExchangeData.GetIndexPricesAsync(new[] { "ETHUSDT" })` |
| Get futures funding rates | `client.FuturesApi.ExchangeData.GetFundingRatesAsync(new[] { "ETHUSDT" })` |
| Get futures funding history | `client.FuturesApi.ExchangeData.GetFundingRateHistoryAsync("ETHUSDT")` |
| Get premium index history | `client.FuturesApi.ExchangeData.GetPremiumIndexPriceHistoryAsync("ETHUSDT")` |
| Get futures position levels | `client.FuturesApi.ExchangeData.GetPositionLevelsAsync(new[] { "ETHUSDT" })` |
| Get liquidation history | `client.FuturesApi.ExchangeData.GetLiquidationHistoryAsync("ETHUSDT")` |
| Get basis history | `client.FuturesApi.ExchangeData.GetBasisHistoryAsync("ETHUSDT")` |
| Get futures balances | `client.FuturesApi.Account.GetBalancesAsync()` |
| Get futures trade fees | `client.FuturesApi.Account.GetTradingFeesAsync("ETHUSDT")` |
| Set futures leverage | `client.FuturesApi.Account.SetLeverageAsync("ETHUSDT", MarginMode.Cross, 5)` |
| Place futures order | `client.FuturesApi.Trading.PlaceOrderAsync("ETHUSDT", OrderSide.Buy, OrderTypeV2.Market, quantity)` |
| Place futures stop order | `client.FuturesApi.Trading.PlaceStopOrderAsync(...)` |
| Place multiple futures orders | `client.FuturesApi.Trading.PlaceMultipleOrdersAsync(requests)` |
| Query futures order | `client.FuturesApi.Trading.GetOrderAsync("ETHUSDT", orderId)` |
| Get open futures orders | `client.FuturesApi.Trading.GetOpenOrdersAsync(symbol: "ETHUSDT")` |
| Get closed futures orders | `client.FuturesApi.Trading.GetClosedOrdersAsync(symbol: "ETHUSDT")` |
| Get open futures stop orders | `client.FuturesApi.Trading.GetOpenStopOrdersAsync(symbol: "ETHUSDT")` |
| Get closed futures stop orders | `client.FuturesApi.Trading.GetClosedStopOrdersAsync(symbol: "ETHUSDT")` |
| Edit futures order | `client.FuturesApi.Trading.EditOrderAsync("ETHUSDT", orderId, quantity, price)` |
| Cancel all futures orders | `client.FuturesApi.Trading.CancelAllOrdersAsync("ETHUSDT")` |
| Cancel futures order | `client.FuturesApi.Trading.CancelOrderAsync("ETHUSDT", orderId)` |
| Cancel futures order by client id | `client.FuturesApi.Trading.CancelOrderByClientOrderIdAsync("ETHUSDT", clientOrderId)` |
| Get futures user trades | `client.FuturesApi.Trading.GetUserTradesAsync("ETHUSDT")` |
| Get futures order trades | `client.FuturesApi.Trading.GetOrderTradesAsync("ETHUSDT", orderId)` |
| Get open positions | `client.FuturesApi.Trading.GetPositionsAsync("ETHUSDT")` |
| Get position history | `client.FuturesApi.Trading.GetPositionHistoryAsync("ETHUSDT")` |
| Close position | `client.FuturesApi.Trading.ClosePositionAsync("ETHUSDT", OrderTypeV2.Market, quantity: quantity)` |
| Adjust position margin | `client.FuturesApi.Trading.AdjustPositionMarginAsync("ETHUSDT", quantity)` |
| Set stop loss | `client.FuturesApi.Trading.SetStopLossAsync("ETHUSDT", PriceType.LastPrice, stopLossPrice)` |
| Edit stop loss | `client.FuturesApi.Trading.EditStopLossAsync("ETHUSDT", stopLossOrderId, ...)` |
| Cancel stop loss | `client.FuturesApi.Trading.CancelStopLossAsync("ETHUSDT", stopLossOrderId)` |
| Set take profit | `client.FuturesApi.Trading.SetTakeProfitAsync("ETHUSDT", PriceType.LastPrice, takeProfitPrice)` |
| Edit take profit | `client.FuturesApi.Trading.EditTakeProfitAsync("ETHUSDT", takeProfitOrderId, ...)` |
| Cancel take profit | `client.FuturesApi.Trading.CancelTakeProfitAsync("ETHUSDT", takeProfitOrderId)` |
| Get position margin history | `client.FuturesApi.Trading.GetMarginHistoryAsync("ETHUSDT", positionId)` |
| Get position funding history | `client.FuturesApi.Trading.GetFundingRateHistoryAsync("ETHUSDT", positionId)` |
| Get ADL history | `client.FuturesApi.Trading.GetAutoDeleverageHistoryAsync("ETHUSDT", positionId)` |
| Get settlement history | `client.FuturesApi.Trading.GetAutoSettlementHistoryAsync("ETHUSDT", positionId)` |

## Spot WebSocket

| User intent | CoinEx.Net member |
|---|---|
| Subscribe system notices | `socketClient.SpotApiV2.SubscribeToSystemNoticeUpdatesAsync(handler)` |
| Subscribe spot ticker updates | `socketClient.SpotApiV2.SubscribeToTickerUpdatesAsync(new[] { "BTCUSDT" }, handler)` |
| Subscribe all spot ticker updates | `socketClient.SpotApiV2.SubscribeToTickerUpdatesAsync(handler)` |
| Subscribe spot order book | `socketClient.SpotApiV2.SubscribeToOrderBookUpdatesAsync("BTCUSDT", 20, "0", fullBookUpdates: true, handler)` |
| Subscribe spot trades | `socketClient.SpotApiV2.SubscribeToTradeUpdatesAsync("BTCUSDT", handler)` |
| Subscribe all spot trades | `socketClient.SpotApiV2.SubscribeToTradeUpdatesAsync(handler)` |
| Subscribe spot index price | `socketClient.SpotApiV2.SubscribeToIndexPriceUpdatesAsync("BTCUSDT", handler)` |
| Subscribe spot book price / BBO | `socketClient.SpotApiV2.SubscribeToBookPriceUpdatesAsync("BTCUSDT", handler)` |
| Subscribe spot order updates | `socketClient.SpotApiV2.SubscribeToOrderUpdatesAsync(handler)` |
| Subscribe spot stop order updates | `socketClient.SpotApiV2.SubscribeToStopOrderUpdatesAsync(handler)` |
| Subscribe spot user trades | `socketClient.SpotApiV2.SubscribeToUserTradeUpdatesAsync(handler)` |
| Subscribe spot balance updates | `socketClient.SpotApiV2.SubscribeToBalanceUpdatesAsync(handler)` |

## Futures WebSocket

| User intent | CoinEx.Net member |
|---|---|
| Subscribe futures ticker updates | `socketClient.FuturesApi.SubscribeToTickerUpdatesAsync(new[] { "ETHUSDT" }, handler)` |
| Subscribe all futures ticker updates | `socketClient.FuturesApi.SubscribeToTickerUpdatesAsync(handler)` |
| Subscribe futures order book | `socketClient.FuturesApi.SubscribeToOrderBookUpdatesAsync("ETHUSDT", 20, "0", fullBookUpdates: true, handler)` |
| Subscribe futures trades | `socketClient.FuturesApi.SubscribeToTradeUpdatesAsync("ETHUSDT", handler)` |
| Subscribe futures index price | `socketClient.FuturesApi.SubscribeToIndexPriceUpdatesAsync("ETHUSDT", handler)` |
| Subscribe futures book price / BBO | `socketClient.FuturesApi.SubscribeToBookPriceUpdatesAsync("ETHUSDT", handler)` |
| Subscribe futures premium index | `socketClient.FuturesApi.SubscribeToPremiumIndexUpdatesAsync("ETHUSDT", handler)` |
| Subscribe futures order updates | `socketClient.FuturesApi.SubscribeToOrderUpdatesAsync(handler)` |
| Subscribe futures stop order updates | `socketClient.FuturesApi.SubscribeToStopOrderUpdatesAsync(handler)` |
| Subscribe futures user trades | `socketClient.FuturesApi.SubscribeToUserTradeUpdatesAsync(handler)` |
| Subscribe futures balance updates | `socketClient.FuturesApi.SubscribeToBalanceUpdatesAsync(handler)` |
| Subscribe futures position updates | `socketClient.FuturesApi.SubscribeToPositionUpdatesAsync(handler)` |

## SharedApis

| User intent | CoinEx.Net member or interface |
|---|---|
| Shared spot REST client | `new CoinExRestClient().SpotApiV2.SharedClient` |
| Shared futures REST client | `new CoinExRestClient().FuturesApi.SharedClient` |
| Shared spot socket client | `new CoinExSocketClient().SpotApiV2.SharedClient` |
| Shared futures socket client | `new CoinExSocketClient().FuturesApi.SharedClient` |
| Shared spot ticker REST | `ISpotTickerRestClient.GetSpotTickerAsync(new GetTickerRequest(symbol))` |
| Shared futures ticker REST | `IFuturesTickerRestClient.GetFuturesTickerAsync(new GetTickerRequest(symbol))` |
| Shared spot order REST | `ISpotOrderRestClient.PlaceSpotOrderAsync(...)` |
| Shared futures order REST | `IFuturesOrderRestClient.PlaceFuturesOrderAsync(...)` |
| Shared balance REST | `IBalanceRestClient.GetBalancesAsync(...)` |
| Shared position REST | `IPositionRestClient.GetPositionsAsync(...)` |
| Shared ticker socket | `ITickerSocketClient.SubscribeToTickerUpdatesAsync(...)` |
| Shared order book socket | `IOrderBookSocketClient.SubscribeToOrderBookUpdatesAsync(...)` |

For shared socket subscriptions, keep the concrete socket client and unsubscribe with `await socketClient.UnsubscribeAsync(subscription.Data)`.

## Result Handling

| Situation | Pattern |
|---|---|
| REST success check | `if (!result.Success) { Console.WriteLine(result.Error); return; }` |
| Socket subscription success check | `if (!sub.Success) { Console.WriteLine(sub.Error); return; }` |
| Read REST data | Read `result.Data` only after `result.Success` |
| Retry decision | Retry only when `result.Error?.IsTransient == true` |
| Cancellation | Pass `ct: cancellationToken` |

## Common Routing Pitfalls

| Do not use | Use instead |
|---|---|
| `CoinExClient` | `CoinExRestClient` |
| `ApiCredentials` | `CoinExCredentials` |
| `SpotApi` for current examples | `SpotApiV2` |
| `SpotApiV2.ExchangeData.GetTickerAsync(...)` | `SpotApiV2.ExchangeData.GetTickersAsync(new[] { symbol })` |
| `SpotOrderType` / `FuturesOrderType` | `OrderTypeV2` |
| Binance `timeInForce` parameter | CoinEx `OrderTypeV2.Limit`, `ImmediateOrCancel`, `FillOrKill`, `PostOnly` |
| Binance USD-M / COIN-M branches | `FuturesApi` |
| `FuturesApi.Account.GetPositionsAsync(...)` | `FuturesApi.Trading.GetPositionsAsync(...)` |
| `.Data` without `.Success` check | Check `.Success` first |
| `ITickerSocketClient.UnsubscribeAsync(...)` | Keep the concrete socket client and call `socketClient.UnsubscribeAsync(subscription.Data)` |
