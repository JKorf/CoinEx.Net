# AI-Friendly Examples

These examples are optimized for AI coding assistants and quick onboarding. Each file is:

- **Compilable** - drop into a console project with `dotnet add package CoinEx.Net` and it builds.
- **Self-contained** - single file, no external setup, no shared helpers.
- **Heavily commented** - explains why each line matters, not only what it does.
- **Idiomatic** - follows current CoinEx.Net 10.x patterns.

## Files

| File | What it shows |
|---|---|
| `01-spot-quickstart.cs` | Client setup, public ticker, authenticated balance, place limit order, query order status |
| `02-futures.cs` | Futures leverage, market order, get position, close position |
| `03-websocket.cs` | Subscribe to ticker, order book, trades, user streams with proper teardown |
| `04-multi-exchange.cs` | `CryptoExchange.Net.SharedApis` pattern, capability discovery, and shared subscriptions |
| `05-error-handling.cs` | `HttpResult`, `WebSocketResult`, and `ExchangeCallResult` patterns, retry, common error scenarios |

## Running

```bash
dotnet new console -n MyCoinExApp
cd MyCoinExApp
dotnet add package CoinEx.Net
# Copy the example .cs file content into Program.cs
# Replace API_KEY / API_SECRET placeholders with your own for private endpoints
dotnet run
```
