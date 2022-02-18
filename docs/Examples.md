---
title: Examples
nav_order: 3
---

## Basic operations
Make sure to read the [CryptoExchange.Net documentation](https://jkorf.github.io/CryptoExchange.Net/Clients.html#processing-request-responses) on processing responses.

### Get market data
```csharp
// Getting info on all symbols
var symbolData = await coinexClient.SpotApi.ExchangeData.GetSymbolsAsync();

// Getting tickers for all symbols
var tickerData = await coinexClient.SpotApi.ExchangeData.GetTickersAsync();

// Getting the order book of a symbol
var orderBookData = await coinexClient.SpotApi.ExchangeData.GetOrderBookAsync("BTC-USDT", 0);

// Getting recent trades of a symbol
var tradeHistoryData = await coinexClient.SpotApi.ExchangeData.GetTradeHistoryAsync("BTC-USDT");
```

### Requesting balances
```csharp
var accountData = await coinexClient.SpotApi.Account.GetAccountsAsync();
```
### Placing order
```csharp
// Placing a buy limit order for 0.001 BTC at a price of 50000USDT each
var orderData = await coinexClient.SpotApi.Trading.PlaceOrderAsync(
                "BTCUSDT",
                OrderSide.Buy,
                OrderType.Limit,
                0.001m,
                50000);
									
													
// Place a stop loss order, place a limit order of 0.001 BTC at 39000USDT each when the last trade price drops below 40000USDT
var orderData = await coinexClient.SpotApi.Trading.PlaceOrderAsync(
                "BTCUSDT",
                OrderSide.Buy,
                OrderType.StopLimit,
                0.001m,
                39000,
                stopPrice: 40000);
```

### Requesting a specific order
```csharp
// Request info on order with id `1234`
var orderData = await coinexClient.SpotApi.Trading.GetOrderAsync("BTCUSDT", 1234);
```

### Requesting order history
```csharp
// Get all closed orders conform the parameters
 var ordersData = await coinexClient.SpotApi.Trading.GetClosedOrdersAsync("BTCUSDT");
```

### Cancel order
```csharp
// Cancel order with id `1234`
var orderData = await coinexClient.SpotApi.Trading.CancelOrderAsync("BTCUSDT", 1234);
```

### Get user trades
```csharp
var userTradesResult = await coinexClient.SpotApi.Trading.GetUserTradesAsync("BTCUSDT");
```

### Subscribing to market data updates
```csharp
var subscribeResult = await coinexSocket.SpotStreams.SubscribeToAllTickerUpdatesAsync(data =>
{
	// Handle ticker data
});
```

### Subscribing to order updates
```csharp
var subscribeResult = await kucoinSocketClient.SpotStreams.SubscribeToOrderUpdatesAsync(data =>
{
	// Handle order updates
});
```
