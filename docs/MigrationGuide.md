---
title: Migrate V4 to V5
nav_order: 4
---

## Migrate from version V4.x.x to V5.x.x

There are a decent amount of breaking changes when moving from version 4.x.x to version 5.x.x. Although the interface has changed, the available endpoints/information have not, so there should be no need to completely rewrite your program.
Most endpoints are now available under a slightly different name or path, and most data models have remained the same, barring a few renames.
In this document most changes will be described. If you have any other questions or issues when updating, feel free to open an issue.

Changes related to `IExchangeClient`, options and client structure are also (partially) covered in the [CryptoExchange.Net Migration Guide](https://jkorf.github.io/CryptoExchange.Net/Migration%20Guide.html)

### Namespaces
There are a few namespace changes: 
 
|Type|Old|New|
|----|---|---|
|Enums|`CoinEx.Net.Objects`|`CoinEx.Net.Enums`  |
|Clients|`CoinEx.Net`|`CoinEx.Net.Clients`  |
|Client interfaces|`CoinEx.Net.Interfaces`|`CoinEx.Net.Interfaces.Clients`  |
|Objects|`CoinEx.Net.Objects`|`CoinEx.Net.Objects.Models`  |
|SymbolOrderBook|`CoinEx.Net`|`CoinEx.Net.SymbolOrderBooks`|

### Client options
The `BaseAddress` and rate limiting options are now under the `SpotApiOptions`.  
*V4*
```csharp
var coinExClient = new CoinExClient(new CoinExClientOptions
{
	ApiCredentials = new ApiCredentials("API-KEY", "API-SECRET"),
	BaseAddress = "ADDRESS",
	RateLimitingBehaviour = RateLimitingBehaviour.Fail
});
```

*V5*
```csharp
var coinExClient = new CoinExClient(new CoinExClientOptions
{
	ApiCredentials = new ApiCredentials("API-KEY", "API-SECRET"),
	SpotApiOptions = new RestApiClientOptions
	{
		BaseAddress = "ADDRESS",
		RateLimitingBehaviour = RateLimitingBehaviour.Fail
	}
});
```

### Client structure
Version 5 adds the `SpotApi` Api client under the `CoinExClient`, and a topic underneath that. This is done to keep the same client structure as other exchange implementations, more info on this [here](https://jkorf.github.io/CryptoExchange.Net/Clients.html).
In the `CoinExSocketClient` a `SpotStreams` Api client is added. This means all calls will have changed, though most will only need to add `SpotApi.[Topic].XXX`/`SpotStreams.XXX`:

*V5*
```csharp
var balances = await coinexClient.GetBalancesAsync();
var withdrawals = await coinexClient.GetWithdrawalHistoryAsync();

var tickers = await coinexClient.GetSymbolStatesAsync();
var symbols = await coinexClient.GetSymbolsAsync();

var order = await coinexClient.PlaceLimitOrderAsync();
var trades = await coinexClient.GetUserTradesAsync("BTCUSDT", 1, 10);

var sub = coinexSocket.SubscribeToSymbolStateUpdatesAsync("BTCUSDT", DataHandler);
```

*V6*  
```csharp
var balances = await coinexClient.SpotApi.Account.GetBalancesAsync();
var withdrawals = await coinexClient.SpotApi.Account.GetWithdrawalHistoryAsync();

var tickers = await coinexClient.SpotApi.ExchangeData.GetTickersAsync();
var symbols = await coinexClient.SpotApi.ExchangeData.GetSymbolsAsync();

var order = await coinexClient.SpotApi.Trading.PlaceOrderAsync();
var trades = await coinexClient.SpotApi.Trading.GetUserTradesAsync("BTCUSDT");

var sub = coinexSocket.SpotStreams.SubscribeToTickerUpdatesAsync("BTCUSDT", DataHandler);
```

### Definitions
Some names have been changed to a common definition. This includes where the name is part of a bigger name  

|Old|New||
|----|---|---|
|`Currency`|`Asset`|`GetCurrenciesAsync` -> `GetAssetsAsync`|
|`Amount`|`Quantity`|`ExecutedAmount` -> `QuantityFilled`|
|`SymbolState`|`Ticker`|`GetSymbolStateAsync()` -> `GetTickerAsync()`|
|`Timestamp/Open/High/Low/Close`|`OpenTime/OpenPrice/HighPrice/LowPrice/ClosePrice`||
|`BestBuyPrice/BestBuyAmount`|`BestBidPrice/BestBidQuantity`||
|`BestSellPrice/BestSellAmount`|`BestAskPrice/BestAskQuantity`||
|`TransactionType`|`Side`||
|`Chain`|`Network`||

### Changed methods
Previously there were different PlaceOrder methods from different types, for example PlaceLimitOrder, PlaceMarketOrder etc. This is because the API has separate endpoints for different types. To keep it more in line with other CryptoExchange.Net implementations this has been combined to a single PlaceOrder call:  
*V5*
```csharp
// Limit order
await coinexClient.PlaceLimitOrderAsync(
	"BTCUSDT",
	TransactionType.Buy,
	0.001m,
	50000);
	
// Market order
await coinexClient.PlaceMarketOrderAsync(
	"BTCUSDT",
	TransactionType.Buy,
	0.001m);
```

*V6*
```csharp
// Limit order
await coinexClient.SpotApi.Trading.PlaceOrderAsync(
	"BTCUSDT",
	OrderSide.Buy,
	OrderType.Limit,
	0.001m,
	50000);
				
// Market order
await coinexClient.SpotApi.Trading.PlaceOrderAsync(
	"BTCUSDT",
	OrderSide.Buy,
	OrderType.Market,
	0.001m);
```

