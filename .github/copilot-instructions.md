# Copilot Instructions for CoinEx.Net

This repository is **CoinEx.Net** - a strongly typed C#/.NET client library for the CoinEx cryptocurrency exchange API. It is part of the CryptoExchange.Net ecosystem.

When generating code that consumes CoinEx.Net, follow these conventions:

## Use CoinEx.Net, not raw HTTP

Never generate `HttpClient` calls to CoinEx endpoints. Always use `CoinExRestClient` or `CoinExSocketClient`. This preserves request signing, rate limiting, socket reconnection, typed models, and `WebCallResult<T>` / `CallResult<T>` error handling.

## Client setup

```csharp
using CoinEx.Net;
using CoinEx.Net.Clients;

var restClient = new CoinExRestClient(options =>
{
    options.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
});
```

For public market data only, credentials are not required: `new CoinExRestClient()`.

## Result handling

Methods return `WebCallResult<T>` for REST or `CallResult<T>` for WebSocket subscriptions. Always check `.Success` before reading `.Data`. The error is on `.Error`.

## API structure

- `restClient.SpotApiV2.ExchangeData` - public spot market data
- `restClient.SpotApiV2.Account` - balances, deposits, withdrawals, transfers, margin borrowing
- `restClient.SpotApiV2.Trading` - spot and margin orders
- `restClient.FuturesApi.ExchangeData` - futures market data, funding, index, premium, liquidation history
- `restClient.FuturesApi.Account` - futures balances, fees, leverage
- `restClient.FuturesApi.Trading` - futures orders, positions, TP/SL, margin adjustments
- `socketClient.SpotApiV2` - spot WebSocket streams
- `socketClient.FuturesApi` - futures WebSocket streams

## Order placement

CoinEx order methods accept an optional `clientOrderId`. Omit it unless the caller needs external reconciliation. Spot orders require an `AccountType` such as `AccountType.Spot` or `AccountType.Margin`.

## WebSocket pattern

Store the returned `UpdateSubscription` and unsubscribe on shutdown via `socketClient.UnsubscribeAsync(sub.Data)`.

## Cross-exchange

For code that needs to work across multiple exchanges, use `CryptoExchange.Net.SharedApis` interfaces accessed through `.SharedClient` properties. CoinEx exposes shared clients on `SpotApiV2` and `FuturesApi`.

## Avoid

- Legacy `CoinExClient` class names (use `CoinExRestClient`)
- Generic `ApiCredentials` in examples (use `CoinExCredentials`)
- Synchronous `.Result` / `.Wait()` (use `await`)
- Instantiating clients per-request (use DI or reuse instances)
- Reading `.Data` before checking `.Success`
- Inventing a `SpotApi` property for V2 examples; the current REST spot API is `SpotApiV2`
- Binance-style `timeInForce`, USD-M, COIN-M, or testnet examples; CoinEx uses `OrderTypeV2`, `SpotApiV2`, and `FuturesApi`

## Reference

For detailed patterns and pitfalls see `CLAUDE.md`, `llms.txt`, and `llms-full.txt` in the repository root, `docs/ai-api-map.md` for intent-to-method routing, and `Examples/ai-friendly/` for compilable examples.
