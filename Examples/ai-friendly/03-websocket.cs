// 03-websocket.cs
//
// Demonstrates: WebSocket subscriptions - public ticker, order book,
// trades, authenticated user streams. Includes proper teardown.
//
// Setup: dotnet add package CoinEx.Net

using CoinEx.Net;
using CoinEx.Net.Clients;

// ---- 1. PUBLIC SOCKET CLIENT - for market data streams ----
// Reuse a single client instance across subscriptions.
var publicSocket = new CoinExSocketClient();

var tickerSub = await publicSocket.SpotApiV2.SubscribeToTickerUpdatesAsync(
    new[] { "BTCUSDT" },
    update =>
    {
        foreach (var ticker in update.Data)
            Console.WriteLine($"{ticker.Symbol}: {ticker.LastPrice} (24h vol {ticker.Volume})");
    });

if (!tickerSub.Success)
{
    Console.WriteLine($"Failed to subscribe ticker: {tickerSub.Error}");
    return;
}

var bookSub = await publicSocket.SpotApiV2.SubscribeToOrderBookUpdatesAsync(
    symbol: "BTCUSDT",
    depth: 20,
    mergeLevel: "0",
    fullBookUpdates: true,
    onMessage: update =>
    {
        var bestBid = update.Data.Data.Bids.FirstOrDefault();
        var bestAsk = update.Data.Data.Asks.FirstOrDefault();
        Console.WriteLine($"Book {update.Data.Symbol}: bid={bestBid?.Price} ask={bestAsk?.Price}");
    });

if (!bookSub.Success)
{
    Console.WriteLine($"Failed to subscribe order book: {bookSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    return;
}

var tradeSub = await publicSocket.SpotApiV2.SubscribeToTradeUpdatesAsync(
    "ETHUSDT",
    update =>
    {
        foreach (var trade in update.Data)
            Console.WriteLine($"ETH trade {trade.Side}: {trade.Quantity} at {trade.Price}");
    });

if (!tradeSub.Success)
{
    Console.WriteLine($"Failed to subscribe trades: {tradeSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    await publicSocket.UnsubscribeAsync(bookSub.Data);
    return;
}

// ---- 2. AUTHENTICATED SOCKET CLIENT - for user data ----
// User streams push order updates, trades, and balance updates.
var authSocket = new CoinExSocketClient(options =>
{
    options.ApiCredentials = new CoinExCredentials("API_KEY", "API_SECRET");
});

var orderSub = await authSocket.SpotApiV2.SubscribeToOrderUpdatesAsync(
    update =>
    {
        var order = update.Data.Order;
        Console.WriteLine($"Order {order.Id} {order.Symbol}: filled {order.QuantityFilled}/{order.Quantity}");
    });

if (!orderSub.Success)
{
    Console.WriteLine($"Failed to subscribe order updates: {orderSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    await publicSocket.UnsubscribeAsync(bookSub.Data);
    await publicSocket.UnsubscribeAsync(tradeSub.Data);
    return;
}

var balanceSub = await authSocket.SpotApiV2.SubscribeToBalanceUpdatesAsync(
    update =>
    {
        foreach (var balance in update.Data)
            Console.WriteLine($"Balance {balance.Asset}: available={balance.Available} frozen={balance.Frozen}");
    });

if (!balanceSub.Success)
{
    Console.WriteLine($"Failed to subscribe balances: {balanceSub.Error}");
    await publicSocket.UnsubscribeAsync(tickerSub.Data);
    await publicSocket.UnsubscribeAsync(bookSub.Data);
    await publicSocket.UnsubscribeAsync(tradeSub.Data);
    await authSocket.UnsubscribeAsync(orderSub.Data);
    return;
}

Console.WriteLine("All subscriptions active. Press Enter to teardown...");
Console.ReadLine();

// ---- 3. TEARDOWN - IMPORTANT ----
await publicSocket.UnsubscribeAsync(tickerSub.Data);
await publicSocket.UnsubscribeAsync(bookSub.Data);
await publicSocket.UnsubscribeAsync(tradeSub.Data);
await authSocket.UnsubscribeAsync(orderSub.Data);
await authSocket.UnsubscribeAsync(balanceSub.Data);

Console.WriteLine("Clean shutdown complete.");

// Common variations:
//   All spot tickers:        SubscribeToTickerUpdatesAsync(handler)
//   Futures ticker stream:   socketClient.FuturesApi.SubscribeToTickerUpdatesAsync(...)
//   Futures positions:       socketClient.FuturesApi.SubscribeToPositionUpdatesAsync(...)
//   Book ticker / BBO:       SubscribeToBookPriceUpdatesAsync(symbol, handler)
//   Premium index:           socketClient.FuturesApi.SubscribeToPremiumIndexUpdatesAsync(...)
