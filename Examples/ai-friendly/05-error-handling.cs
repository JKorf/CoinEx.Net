// 05-error-handling.cs
//
// Demonstrates: WebCallResult patterns, retry logic, common error scenarios.
//
// Setup: dotnet add package CoinEx.Net

using CoinEx.Net;
using CoinEx.Net.Clients;
using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;

var client = new CoinExRestClient(options =>
{
    options.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
});

// ---- 1. THE BASIC PATTERN ----
// Every method returns WebCallResult<T> (REST) or CallResult<T> (WebSocket).
// .Success is true/false. .Data is the payload, only valid when .Success is true.
// .Error contains structured error info when .Success is false.
// .Error.IsTransient hints if a retry might succeed.

var result = await client.SpotApiV2.ExchangeData.GetTickersAsync(new[] { "BTCUSDT" });

if (result.Success)
{
    Console.WriteLine($"Price: {result.Data.First().LastPrice}");
}
else
{
    Console.WriteLine($"Code:      {result.Error?.Code}");
    Console.WriteLine($"Message:   {result.Error?.Message}");
    Console.WriteLine($"Type:      {result.Error?.ErrorType}");
    Console.WriteLine($"Transient: {result.Error?.IsTransient}");
}

// ---- 2. SIMPLE RETRY WITH BACKOFF ----
// Retry only on transient errors such as network issues, rate limits, or server overload.
// Do not retry validation errors, bad credentials, or insufficient balance.

async Task<WebCallResult<T>> WithRetry<T>(
    Func<Task<WebCallResult<T>>> call,
    int maxAttempts = 3)
{
    WebCallResult<T> last = default!;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        last = await call();
        if (last.Success)
            return last;

        if (last.Error?.IsTransient != true)
            return last;

        await Task.Delay(TimeSpan.FromMilliseconds(250 * Math.Pow(2, attempt)));
    }

    return last;
}

var ticker = await WithRetry(
    () => client.SpotApiV2.ExchangeData.GetTickersAsync(new[] { "BTCUSDT" }));

if (ticker.Success)
    Console.WriteLine($"Retry helper price: {ticker.Data.First().LastPrice}");

// ---- 3. COMMON COINEX ERROR SCENARIOS ----
//
// Authentication or signature error:
//   API key, secret, permissions, or timestamp/signature setup is wrong.
//   Permanent until configuration changes; do not retry indefinitely.
//
// Rate limit / temporary service errors:
//   Usually transient. Retry with backoff and keep clients reused so client-side
//   rate limiting has a chance to work.
//
// Invalid symbol or unsupported market:
//   Permanent for that request. Use GetSymbolsAsync before placing orders or
//   when accepting user-supplied symbols.
//
// Invalid order quantity / price:
//   Permanent until the caller adjusts order size, price, or order type.
//   Query symbol metadata and round to the exchange precision instead of using
//   ad hoc string truncation.
//
// Insufficient balance:
//   Permanent for that account state. Surface to the caller.
//
// Order not found:
//   May be expected if the order already filled, was canceled, or the wrong
//   account type was used for spot/margin.

// ---- 4. ORDER PLACEMENT WITH SYMBOL VALIDATION ----
var symbols = await client.SpotApiV2.ExchangeData.GetSymbolsAsync();
if (!symbols.Success)
{
    Console.WriteLine($"Cannot fetch symbol info: {symbols.Error}");
    return;
}

var btcusdt = symbols.Data.FirstOrDefault(s => s.Name == "BTCUSDT");
if (btcusdt == null)
{
    Console.WriteLine("BTCUSDT is not available on CoinEx spot.");
    return;
}

decimal rawQuantity = 0.00123456m;
decimal rawPrice = 50000.123456m;

// CoinEx symbols expose precision fields. Round using exchange metadata
// before placing orders to avoid quantity/price precision errors.
var validQuantity = Math.Round(rawQuantity, btcusdt.QuantityPrecision, MidpointRounding.ToZero);
var validPrice = Math.Round(rawPrice, btcusdt.PricePrecision, MidpointRounding.ToZero);

var order = await client.SpotApiV2.Trading.PlaceOrderAsync(
    symbol: "BTCUSDT",
    accountType: AccountType.Spot,
    side: OrderSide.Buy,
    type: OrderTypeV2.Limit,
    quantity: validQuantity,
    price: validPrice);

if (!order.Success)
{
    var category = order.Error?.IsTransient == true
        ? "Transient - retry with backoff"
        : "Permanent - surface to user";

    Console.WriteLine($"{category}: {order.Error?.Code} {order.Error?.Message}");
}

// ---- 5. EXCEPTIONS VS ERROR RESULTS ----
// CoinEx.Net returns exchange, rate limit, and network errors via result.Error.
// Exceptions are generally for misconfiguration, disposal, cancellation, or
// programmer errors. Pass CancellationToken with `ct:` when requests need to be cancelable.

// Common variations:
//   With CancellationToken:    pass `ct: cancellationToken` to any method
//   With timeout per request:  options.RequestTimeout = TimeSpan.FromSeconds(10);
//   Polly integration:         use IsTransient as the retry predicate
