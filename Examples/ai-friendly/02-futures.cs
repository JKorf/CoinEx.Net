// 02-futures.cs
//
// Demonstrates: CoinEx futures - set leverage, place market order,
// retrieve open position, close position.
//
// Setup: dotnet add package CoinEx.Net
// Substitute API_KEY / API_SECRET. The API key must have futures trading enabled.

using CoinEx.Net;
using CoinEx.Net.Clients;
using CoinEx.Net.Enums;

var client = new CoinExRestClient(options =>
{
    options.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
});

const string symbol = "ETHUSDT";

// ---- 1. SET LEVERAGE ----
// Leverage is per symbol. Choose Cross or Isolated based on account risk policy.
var leverage = await client.FuturesApi.Account.SetLeverageAsync(symbol, MarginMode.Cross, 5);
if (!leverage.Success)
{
    Console.WriteLine($"Failed to set leverage: {leverage.Error}");
    return;
}
Console.WriteLine($"Leverage set for {symbol}");

// ---- 2. PLACE MARKET ORDER (open long position) ----
// Market order fills immediately at best available price. CoinEx.Net futures
// order placement does not take AccountType; it is already under FuturesApi.
var openOrder = await client.FuturesApi.Trading.PlaceOrderAsync(
    symbol: symbol,
    side: OrderSide.Buy,
    type: OrderTypeV2.Market,
    quantity: 0.01m);

if (!openOrder.Success)
{
    Console.WriteLine($"Failed to open position: {openOrder.Error}");
    return;
}
Console.WriteLine($"Opened position via order {openOrder.Data.Id}");

// ---- 3. GET CURRENT POSITION ----
var positions = await client.FuturesApi.Trading.GetPositionsAsync(symbol);
if (!positions.Success)
{
    Console.WriteLine($"Failed to get positions: {positions.Error}");
    return;
}

var position = positions.Data.Items.FirstOrDefault(p => p.OpenInterest != 0);
if (position == null)
{
    Console.WriteLine("No open position found (may not have filled yet).");
    return;
}

Console.WriteLine($"Position: {position.Side} {position.OpenInterest} {symbol} at avg {position.AverageEntryPrice}");
Console.WriteLine($"Unrealized PnL: {position.UnrealizedPnl}");
Console.WriteLine($"Liquidation price: {position.LiquidationPrice}");

// ---- 4. CLOSE THE POSITION ----
// Prefer ClosePositionAsync when the intent is closing/reducing a futures position.
var closeOrder = await client.FuturesApi.Trading.ClosePositionAsync(
    symbol: symbol,
    orderType: OrderTypeV2.Market,
    quantity: position.CloseAvailable);

if (closeOrder.Success)
{
    Console.WriteLine($"Closed position via order {closeOrder.Data.Id}");
}

// Common variations:
//   Limit order:          type: OrderTypeV2.Limit, add price
//   Stop order:           client.FuturesApi.Trading.PlaceStopOrderAsync(...)
//   Take profit:          client.FuturesApi.Trading.SetTakeProfitAsync(...)
//   Stop loss:            client.FuturesApi.Trading.SetStopLossAsync(...)
//   Isolated leverage:    client.FuturesApi.Account.SetLeverageAsync(symbol, MarginMode.Isolated, leverage)
//   Adjust margin:        client.FuturesApi.Trading.AdjustPositionMarginAsync(symbol, quantity)
