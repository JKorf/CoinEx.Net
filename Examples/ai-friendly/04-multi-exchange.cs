// 04-multi-exchange.cs
//
// Demonstrates: writing exchange-agnostic code using CryptoExchange.Net.SharedApis
// with capability discovery and shared REST/WebSocket calls.
// Same code works against CoinEx, Binance, OKX, Bybit, Kraken, and other
// exchanges from the CryptoExchange.Net family.
//
// Setup:
//   dotnet add package CoinEx.Net
//   dotnet add package Binance.Net   // optional, for a Binance comparison
//   dotnet add package JK.OKX.Net    // optional, for an OKX comparison

using CoinEx.Net.Clients;
using CryptoExchange.Net.SharedApis;

// ---- THE PATTERN ----
// Each exchange client exposes a `.SharedClient` property on its API surfaces.
// SharedClient implements interfaces like ISpotTickerRestClient, ISpotOrderRestClient,
// IBalanceRestClient, etc. - a common abstraction across exchanges.

ISpotTickerRestClient coinexShared = new CoinExRestClient().SpotApiV2.SharedClient;
var info = coinexShared.Discover();
Console.WriteLine(info);

// To add Binance or OKX, install the package and:
//   ISpotTickerRestClient binanceShared = new BinanceRestClient().SpotApi.SharedClient;
//   ISpotTickerRestClient okxShared     = new OKXRestClient().UnifiedApi.SharedClient;

// Common symbol type - handles formatting differences between exchanges automatically.
// CoinEx uses "BTCUSDT", OKX uses "BTC-USDT", others may differ.
var btcusdt = new SharedSymbol(TradingMode.Spot, "BTC", "USDT");

await PrintTicker(coinexShared, btcusdt);
// await PrintTicker(binanceShared, btcusdt);
// await PrintTicker(okxShared, btcusdt);

// ---- AGNOSTIC METHOD - works against any exchange ----
async Task PrintTicker(ISpotTickerRestClient client, SharedSymbol symbol)
{
    var result = await client.GetSpotTickerAsync(new GetTickerRequest(symbol));
    if (!result.Success)
    {
        Console.WriteLine($"[{client.Exchange}] Failed: {result.Error}");
        return;
    }

    Console.WriteLine($"[{client.Exchange}] {result.Data.Symbol}: {result.Data.LastPrice}");
}

// ---- WHY THIS MATTERS ----
// You can build:
//   - Multi-exchange arbitrage scanners
//   - Best-execution routers
//   - Unified portfolio dashboards
//   - Exchange comparison tools
// without writing per-exchange branches everywhere.

// ---- AVAILABLE SHARED INTERFACES ----
// CoinEx Spot REST includes:
//   ISpotTickerRestClient, ISpotSymbolRestClient, ISpotOrderRestClient
//   ISpotOrderClientIdRestClient, ISpotTriggerOrderRestClient
//   IBalanceRestClient, IFeeRestClient, IOrderBookRestClient
//   IRecentTradeRestClient, IKlineRestClient, IDepositRestClient
//   IWithdrawalRestClient, ITransferRestClient, IBookTickerRestClient
// CoinEx Futures REST includes:
//   IFuturesTickerRestClient, IFuturesSymbolRestClient, IFuturesOrderRestClient
//   IFuturesOrderClientIdRestClient, IFuturesTriggerOrderRestClient
//   IFuturesTpSlRestClient, IPositionRestClient, ILeverageRestClient
//   IFundingRateRestClient, IOpenInterestRestClient
// WebSocket shared clients include ticker, trade, order book, balance,
// order, user trade, and futures position subscriptions.

// ---- WEBSOCKET EXAMPLE - SHARED SUBSCRIPTION ----
var coinexSocket = new CoinExSocketClient();
ITickerSocketClient tickerSocket = coinexSocket.SpotApiV2.SharedClient;

var sub = await tickerSocket.SubscribeToTickerUpdatesAsync(
    new SubscribeTickerRequest(btcusdt),
    update => Console.WriteLine($"[{tickerSocket.Exchange}] {update.Data.Symbol}: {update.Data.LastPrice}"));

if (!sub.Success)
{
    Console.WriteLine($"Subscribe failed: {sub.Error}");
    return;
}

Console.WriteLine("Press Enter to exit");
Console.ReadLine();

// The shared ticker interface does not expose UnsubscribeAsync; keep the concrete client.
await coinexSocket.UnsubscribeAsync(sub.Data);

// Common variations:
//   Multi-exchange arbitrage:  loop over List<ISpotTickerRestClient>, find max bid / min ask
//   Cross-exchange orderbook:  IOrderBookSocketClient on each exchange, merge into composite book
//   Best execution:            ISpotOrderRestClient on N exchanges, route by liquidity
//   Futures comparison:        use IFuturesTickerRestClient from client.FuturesApi.SharedClient
