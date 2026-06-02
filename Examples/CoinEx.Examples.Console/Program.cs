using CoinEx.Net.Clients;

// REST
var restClient = new CoinExRestClient();
var ticker = await restClient.SpotApiV2.ExchangeData.GetTickersAsync(new[] { "ETHUSDT" });
if (!ticker.Success)
{
    Console.WriteLine($"Failed to get ticker: {ticker.Error}");
    return;
}

Console.WriteLine($"Rest client ticker price for ETHUSDT: {ticker.Data.First().LastPrice}");

Console.WriteLine();
Console.WriteLine("Press enter to start websocket subscription");
Console.ReadLine();

// Websocket
var socketClient = new CoinExSocketClient();
var subscription = await socketClient.SpotApiV2.SubscribeToTickerUpdatesAsync(new[] { "ETHUSDT" }, update =>
{
    Console.WriteLine($"Websocket client ticker price for ETHUSDT: {update.Data.First().LastPrice}");
});

if (!subscription.Success)
{
    Console.WriteLine($"Failed to subscribe to ticker updates: {subscription.Error}");
    return;
}

Console.ReadLine();
