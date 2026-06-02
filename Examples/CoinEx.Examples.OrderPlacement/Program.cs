using CoinEx.Net;
using CoinEx.Net.Clients;
using CoinEx.Net.Enums;

const string spotSymbol = "BTCUSDT";
const string futuresSymbol = "ETHUSDT";

// Replace with valid credentials or order placement will always fail
var apiKey = "KEY";
var apiSecret = "SECRET";

Console.WriteLine("CoinEx.Net order placement example");
Console.WriteLine();
Console.WriteLine("This example can place real orders when valid credentials are configured.");
Console.WriteLine();

var client = new CoinExRestClient(options =>
{
    options.ApiCredentials = new CoinExCredentials(apiKey, apiSecret);
});

await PlaceSpotLimitOrderAsync(client);
Console.WriteLine();
await PlaceFuturesLimitOrderExampleAsync(client);

static async Task PlaceSpotLimitOrderAsync(CoinExRestClient client)
{
    Console.WriteLine($"Placing spot limit buy order for {spotSymbol}...");

    var tickers = await client.SpotApiV2.ExchangeData.GetTickersAsync(new[] { spotSymbol });
    if (!tickers.Success)
    {
        Console.WriteLine($"Failed to get spot ticker: {tickers.Error}");
        return;
    }

    var ticker = tickers.Data.FirstOrDefault();
    if (ticker == null)
    {
        Console.WriteLine($"Failed to get spot ticker: no ticker returned for {spotSymbol}");
        return;
    }

    var safePrice = Math.Round(ticker.LastPrice * 0.95m, 2);
    var order = await client.SpotApiV2.Trading.PlaceOrderAsync(
        symbol: spotSymbol,
        accountType: AccountType.Spot,
        side: OrderSide.Buy,
        type: OrderTypeV2.Limit,
        quantity: 0.001m,
        price: safePrice);

    if (!order.Success)
    {
        Console.WriteLine($"Failed to place spot order: {order.Error}");
        return;
    }

    Console.WriteLine($"Placed spot order {order.Data.Id}, status: {order.Data.Status}");

    var orderStatus = await client.SpotApiV2.Trading.GetOrderAsync(spotSymbol, order.Data.Id);
    if (orderStatus.Success)
        Console.WriteLine($"Spot order status: {orderStatus.Data.Status}, filled: {orderStatus.Data.QuantityFilled}");
    else
        Console.WriteLine($"Failed to query spot order: {orderStatus.Error}");

    var cancel = await client.SpotApiV2.Trading.CancelOrderAsync(spotSymbol, AccountType.Spot, order.Data.Id);
    Console.WriteLine(cancel.Success
        ? $"Cancelled spot order {order.Data.Id}"
        : $"Failed to cancel spot order: {cancel.Error}");
}

static async Task PlaceFuturesLimitOrderExampleAsync(CoinExRestClient client)
{
    Console.WriteLine($"Placing futures limit sell order for {futuresSymbol}...");

    var tickers = await client.FuturesApi.ExchangeData.GetTickersAsync(new[] { futuresSymbol });
    if (!tickers.Success)
    {
        Console.WriteLine($"Failed to get futures ticker: {tickers.Error}");
        return;
    }

    var ticker = tickers.Data.FirstOrDefault();
    if (ticker == null)
    {
        Console.WriteLine($"Failed to get futures ticker: no ticker returned for {futuresSymbol}");
        return;
    }

    var safePrice = Math.Round(ticker.LastPrice * 1.05m, 2);
    var order = await client.FuturesApi.Trading.PlaceOrderAsync(
        symbol: futuresSymbol,
        side: OrderSide.Sell,
        type: OrderTypeV2.Limit,
        quantity: 0.01m,
        price: safePrice);

    if (!order.Success)
    {
        Console.WriteLine($"Failed to place futures order: {order.Error}");
        return;
    }

    Console.WriteLine($"Placed futures order {order.Data.Id}, status: {order.Data.Status}");

    var orderStatus = await client.FuturesApi.Trading.GetOrderAsync(futuresSymbol, order.Data.Id);
    if (orderStatus.Success)
        Console.WriteLine($"Futures order status: {orderStatus.Data.Status}, executed: {orderStatus.Data.QuantityFilled}");
    else
        Console.WriteLine($"Failed to query futures order: {orderStatus.Error}");

    var cancel = await client.FuturesApi.Trading.CancelOrderAsync(futuresSymbol, order.Data.Id);
    Console.WriteLine(cancel.Success
        ? $"Cancelled futures order {order.Data.Id}"
        : $"Failed to cancel futures order: {cancel.Error}");
}
