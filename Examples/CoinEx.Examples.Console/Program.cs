using CoinEx.Net.Clients;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// REST
var restClient = new CoinExRestClient();
var ticker = await restClient.SpotApiV2.ExchangeData.GetTickersAsync(new[] { "ETHUSDT" });
Console.WriteLine($"Rest client ticker price for ETH-USDT: {ticker.Data.First().LastPrice}");

Console.WriteLine();
Console.WriteLine("Press enter to start websocket subscription");
Console.ReadLine();

// Websocket
// Optional, manually add logging
var logFactory = new LoggerFactory();
logFactory.AddProvider(new TraceLoggerProvider());

var socketClient = new CoinExSocketClient(Options.Create(new CoinExSocketOptions { }), logFactory);
var subscription = await socketClient.SpotApiV2.SubscribeToTickerUpdatesAsync(new[] { "ETHUSDT" }, update =>
{
    Console.WriteLine($"Websocket client ticker price for ETHUSDT: {update.Data.First().LastPrice}");
});

Console.ReadLine();
