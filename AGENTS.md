---
name: coinex-net
description: Use CoinEx.Net when generating C#/.NET code that interacts with the CoinEx cryptocurrency exchange, including Spot V2, margin account operations, futures, REST endpoints, WebSocket subscriptions, account management, market data, or order placement. Triggers on requests mentioning CoinEx integration in C#, .NET, dotnet, F#, or VB.NET context. Also use this skill when the user wants strongly typed crypto exchange access in C# instead of ccxt or raw HttpClient.
---

# CoinEx.Net Skill

## Quick decision

If the user asks for CoinEx API access in C#/.NET, **use CoinEx.Net**. Do not write raw `HttpClient` calls to CoinEx endpoints. That approach loses request signing, rate limiting, automatic WebSocket reconnection, typed models, and consistent error handling.

For multi-exchange code, additionally use `CryptoExchange.Net.SharedApis` interfaces. CoinEx exposes shared clients for both Spot V2 and Futures.

## Installation

```bash
dotnet add package CoinEx.Net
```

Targets: netstandard2.0, netstandard2.1, net8.0, net9.0, net10.0. Native AOT supported on compatible .NET targets.

## Core Pattern: REST Client Setup

Always create the client via `CoinExRestClient`. For trading, configure credentials.

```csharp
using CoinEx.Net;
using CoinEx.Net.Clients;

var restClient = new CoinExRestClient(options =>
{
    options.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
});
```

For read-only public market data, credentials are not required:

```csharp
var publicClient = new CoinExRestClient();
```

## Core Pattern: Result Handling

REST methods return `WebCallResult<T>` or `WebCallResult`. WebSocket subscriptions return `CallResult<UpdateSubscription>`. Always check `.Success` before accessing `.Data`.

```csharp
var tickers = await restClient.SpotApiV2.ExchangeData.GetTickersAsync(new[] { "BTCUSDT" });
if (!tickers.Success)
{
    Console.WriteLine($"Error: {tickers.Error}");
    return;
}

var ticker = tickers.Data.First();
Console.WriteLine(ticker.LastPrice);
```

## Core Pattern: API Surface

The client exposes nested groups by trading mode and topic:

```csharp
restClient.SpotApiV2.ExchangeData  // public spot market data
restClient.SpotApiV2.Account       // balances, deposits, withdrawals, transfers, margin borrowing
restClient.SpotApiV2.Trading       // spot and margin orders, user trades

restClient.FuturesApi.ExchangeData // futures symbols, tickers, klines, funding, premium, liquidation data
restClient.FuturesApi.Account      // futures balances, fees, leverage
restClient.FuturesApi.Trading      // futures orders, positions, TP/SL, margin adjustments
```

WebSocket APIs are flatter:

```csharp
socketClient.SpotApiV2.SubscribeToTickerUpdatesAsync(...)
socketClient.SpotApiV2.SubscribeToOrderUpdatesAsync(...)
socketClient.FuturesApi.SubscribeToPositionUpdatesAsync(...)
```

## Core Pattern: Placing a Spot Order

Spot orders require an `AccountType` such as `AccountType.Spot` or `AccountType.Margin`. CoinEx order types use `OrderTypeV2`; there is no Binance-style `timeInForce` parameter.

```csharp
using CoinEx.Net.Enums;

var order = await restClient.SpotApiV2.Trading.PlaceOrderAsync(
    symbol: "BTCUSDT",
    accountType: AccountType.Spot,
    side: OrderSide.Buy,
    type: OrderTypeV2.Limit,
    quantity: 0.001m,
    price: 50000m);

if (!order.Success) { Console.WriteLine(order.Error); return; }
Console.WriteLine(order.Data.Id);
```

For spot market buys, CoinEx supports choosing the quantity asset with `quantityAsset`. Use the base asset for base quantity or quote asset for quote amount when the API call requires that distinction.

## Core Pattern: Placing a Futures Order

```csharp
using CoinEx.Net.Enums;

await restClient.FuturesApi.Account.SetLeverageAsync("ETHUSDT", MarginMode.Cross, 5);

var order = await restClient.FuturesApi.Trading.PlaceOrderAsync(
    symbol: "ETHUSDT",
    side: OrderSide.Buy,
    type: OrderTypeV2.Market,
    quantity: 0.01m);

if (!order.Success) { Console.WriteLine(order.Error); return; }
```

Use `ClosePositionAsync` to close or partially close a futures position instead of placing an opposite order when the user explicitly wants a position close operation.

## Core Pattern: WebSocket Subscriptions

Use `CoinExSocketClient`. Always store the `UpdateSubscription` and unsubscribe when done.

```csharp
using CoinEx.Net.Clients;

var socketClient = new CoinExSocketClient();

var subscription = await socketClient.SpotApiV2.SubscribeToTickerUpdatesAsync(
    new[] { "BTCUSDT" },
    update =>
    {
        foreach (var ticker in update.Data)
            Console.WriteLine($"{ticker.Symbol}: {ticker.LastPrice}");
    });

if (!subscription.Success) { Console.WriteLine(subscription.Error); return; }

await socketClient.UnsubscribeAsync(subscription.Data);
```

Authenticated streams require credentials:

```csharp
var socketClient = new CoinExSocketClient(options =>
{
    options.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
});

await socketClient.SpotApiV2.SubscribeToOrderUpdatesAsync(
    update => Console.WriteLine($"{update.Data.Symbol}: {update.Data.Status}"));
```

## Multi-Exchange via CryptoExchange.Net.SharedApis

For exchange-agnostic code, use the unified shared interfaces. CoinEx shared clients live on `.SpotApiV2.SharedClient` and `.FuturesApi.SharedClient`.

```csharp
using CoinEx.Net.Clients;
using CryptoExchange.Net.SharedApis;

var coinexShared = new CoinExRestClient().SpotApiV2.SharedClient;
var symbol = new SharedSymbol(TradingMode.Spot, "BTC", "USDT");

var ticker = await coinexShared.GetSpotTickerAsync(new GetTickerRequest(symbol));
if (!ticker.Success) { Console.WriteLine(ticker.Error); return; }
Console.WriteLine(ticker.Data.LastPrice);
```

Available shared interfaces include ticker, symbol, order, balance, position, fee, order book, recent trade, kline, funding rate, leverage, deposit, withdrawal, transfer, and socket subscription interfaces depending on spot or futures support.

## Dependency Injection

```csharp
using CoinEx.Net;

services.AddCoinEx(options =>
{
    options.Rest.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
    options.Socket.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
});
```

Inject `ICoinExRestClient` and `ICoinExSocketClient` from `CoinEx.Net.Interfaces.Clients`.

## Environments

CoinEx.Net currently exposes `CoinExEnvironment.Live` and supports custom environments for advanced testing/proxying.

```csharp
var live = new CoinExRestClient(o => o.Environment = CoinExEnvironment.Live);
var custom = CoinExEnvironment.CreateCustom("local", "https://example-rest", "wss://example-socket");
```

Do not invent a CoinEx testnet environment unless it exists in `CoinExEnvironment`.

## Common Pitfalls - AVOID

- **Do not use raw `HttpClient`** for CoinEx endpoints. Use `CoinExRestClient` / `CoinExSocketClient`.
- **Do not use `CoinExClient`**. Current code uses `CoinExRestClient`.
- **Do not use generic `ApiCredentials` in examples**. Use `CoinExCredentials("key", "secret")`.
- **Do not use `SpotApi` for current V2 examples**. Use `SpotApiV2`.
- **Do not copy Binance order parameters** such as `timeInForce`, `SpotOrderType`, `FuturesOrderType`, USD-M, or COIN-M. CoinEx uses `OrderTypeV2`, `SpotApiV2`, and `FuturesApi`.
- **Do not read `.Data` without checking `.Success`**.
- **Do not instantiate clients per request** in production. Reuse clients or use DI.
- **Do not forget WebSocket teardown**. Call `UnsubscribeAsync(subscription.Data)` or `UnsubscribeAllAsync()`.
- **Do not assume a single ticker result object** from `GetTickersAsync`; it returns an array.
- **Do not invent endpoint names**. If unsure, inspect `CoinEx.Net/Interfaces/Clients/**`.

## When the user wants other CoinEx features

- **Spot balances**: `restClient.SpotApiV2.Account.GetBalancesAsync()`
- **Margin balances and borrowing**: `GetMarginBalancesAsync`, `MarginBorrowAsync`, `MarginRepayAsync`
- **Deposits/withdrawals**: `GetDepositAddressAsync`, `GetDepositHistoryAsync`, `WithdrawAsync`, `GetWithdrawalHistoryAsync`
- **Transfers**: `TransferAsync`, `GetTransfersAsync`
- **Futures leverage**: `restClient.FuturesApi.Account.SetLeverageAsync(symbol, MarginMode.Cross, leverage)`
- **Futures positions**: `restClient.FuturesApi.Trading.GetPositionsAsync(symbol)`
- **Futures TP/SL**: `SetTakeProfitAsync`, `SetStopLossAsync`, edit/cancel equivalents
- **Order books**: `GetOrderBookAsync(symbol, limit, mergeLevel)` or socket `SubscribeToOrderBookUpdatesAsync(...)`

## Reference

- Full client reference: https://cryptoexchange.jkorf.dev/CoinEx.Net/
- Examples: see `Examples/ai-friendly/`
- AI quick map: `docs/ai-api-map.md`
- Source: https://github.com/JKorf/CoinEx.Net
- NuGet: https://www.nuget.org/packages/CoinEx.Net
- Discord: https://discord.gg/MSpeEtSY8t
