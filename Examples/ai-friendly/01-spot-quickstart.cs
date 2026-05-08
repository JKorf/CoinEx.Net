// 01-spot-quickstart.cs
//
// Demonstrates: client setup, public market data, authenticated balance,
// limit order placement, order status check.
//
// Setup:
//   dotnet new console -n SpotQuickstart && cd SpotQuickstart
//   dotnet add package CoinEx.Net
//   Copy this file content into Program.cs
//   Substitute API_KEY / API_SECRET below
//   dotnet run

using CoinEx.Net;
using CoinEx.Net.Clients;
using CoinEx.Net.Enums;

// ---- 1. PUBLIC CLIENT (no credentials needed for market data) ----
// Reuse this client across the application; do not create per request.
var publicClient = new CoinExRestClient();

var tickers = await publicClient.SpotApiV2.ExchangeData.GetTickersAsync(new[] { "BTCUSDT" });
if (!tickers.Success)
{
    // .Error contains Code, Message, and may include exchange-specific data.
    Console.WriteLine($"Failed to get ticker: {tickers.Error}");
    return;
}

var ticker = tickers.Data.First();
Console.WriteLine($"{ticker.Symbol} last price: {ticker.LastPrice}");
Console.WriteLine($"24h volume: {ticker.Volume} BTC");

// ---- 2. AUTHENTICATED CLIENT (for account / trading) ----
var tradingClient = new CoinExRestClient(options =>
{
    options.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
});

var balances = await tradingClient.SpotApiV2.Account.GetBalancesAsync();
if (!balances.Success)
{
    Console.WriteLine($"Failed to get balances: {balances.Error}");
    return;
}

foreach (var balance in balances.Data.Where(b => b.Total > 0))
{
    Console.WriteLine($"{balance.Asset}: {balance.Available} available, {balance.Frozen} frozen");
}

// ---- 3. PLACE A LIMIT BUY ORDER ----
// Limit, Buy, 0.001 BTC at a price 5% below current; likely will not fill immediately.
// CoinEx.Net order methods accept an optional clientOrderId. Omit it unless your
// application needs a specific external reconciliation id.
var safePrice = Math.Round(ticker.LastPrice * 0.95m, 2);

var order = await tradingClient.SpotApiV2.Trading.PlaceOrderAsync(
    symbol: "BTCUSDT",
    accountType: AccountType.Spot,
    side: OrderSide.Buy,
    type: OrderTypeV2.Limit,
    quantity: 0.001m,
    price: safePrice);

if (!order.Success)
{
    Console.WriteLine($"Failed to place order: {order.Error}");
    return;
}

Console.WriteLine($"Placed order {order.Data.Id} at {safePrice}, status: {order.Data.Status}");

// ---- 4. CHECK ORDER STATUS ----
var status = await tradingClient.SpotApiV2.Trading.GetOrderAsync("BTCUSDT", order.Data.Id);
if (status.Success)
{
    Console.WriteLine($"Order status: {status.Data.Status}, filled: {status.Data.QuantityFilled}");
}

// ---- 5. CANCEL THE ORDER (cleanup for this example) ----
var cancel = await tradingClient.SpotApiV2.Trading.CancelOrderAsync("BTCUSDT", AccountType.Spot, order.Data.Id);
if (cancel.Success)
{
    Console.WriteLine($"Cancelled order {order.Data.Id}");
}

// Common variations:
//   Market order:       type: OrderTypeV2.Market, omit price
//   Post only order:    type: OrderTypeV2.PostOnly
//   IOC / FOK:          OrderTypeV2.ImmediateOrCancel / OrderTypeV2.FillOrKill
//   Margin order:       accountType: AccountType.Margin
//   Stop order:         tradingClient.SpotApiV2.Trading.PlaceStopOrderAsync(...)
