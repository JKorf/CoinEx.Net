---
title: ICoinExSocketClientSpotStreams
has_children: false
parent: Socket API documentation
---
*[generated documentation]*  
`CoinExSocketClient > SpotStreams`  
*Spot streams*
  

***

## GetBalancesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/051asset](https://github.com/coinexcom/coinex_exchange_api/wiki/051asset)  
<p>

*Get balances of assets. Requires API credentials*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.GetBalancesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(IEnumerable<string> assets);  
```  

|Parameter|Description|
|---|---|
|assets|The assets to get the balances for, empty for all|

</p>

***

## GetKlinesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/046kline](https://github.com/coinexcom/coinex_exchange_api/wiki/046kline)  
<p>

*Gets symbol kline data*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.GetKlinesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<CoinExKline>> GetKlinesAsync(string symbol, KlineInterval interval);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the data for|
|interval|The interval of the candles|

</p>

***

## GetOpenOrdersAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/052order](https://github.com/coinexcom/coinex_exchange_api/wiki/052order)  
<p>

*Gets a list of open orders for a symbol*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.GetOpenOrdersAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<CoinExSocketPagedResult<CoinExSocketOrder>>> GetOpenOrdersAsync(string symbol, OrderSide side, int offset, int limit);  
```  

|Parameter|Description|
|---|---|
|symbol|Symbol to get open orders for|
|side|Order side|
|offset|The offset in the list|
|limit|The limit of results|

</p>

***

## GetOrderBookAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/044depth](https://github.com/coinexcom/coinex_exchange_api/wiki/044depth)  
<p>

*Get an order book*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.GetOrderBookAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<CoinExSocketOrderBook>> GetOrderBookAsync(string symbol, int limit, int mergeDepth);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the order book for|
|limit|The limit of results returned|
|mergeDepth|The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together|

</p>

***

## GetServerTimeAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/049time](https://github.com/coinexcom/coinex_exchange_api/wiki/049time)  
<p>

*Gets the server time*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.GetServerTimeAsync();  
```  

```csharp  
Task<CallResult<DateTime>> GetServerTimeAsync();  
```  

|Parameter|Description|
|---|---|

</p>

***

## GetTickerAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/053state](https://github.com/coinexcom/coinex_exchange_api/wiki/053state)  
<p>

*Get the symbol ticker*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.GetTickerAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<CoinExSocketSymbolState>> GetTickerAsync(string symbol, int cyclePeriod);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the state for|
|cyclePeriod|The period to get data over, specified in seconds. i.e. one minute = 60, one day = 86400|

</p>

***

## GetTradeHistoryAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/045deals](https://github.com/coinexcom/coinex_exchange_api/wiki/045deals)  
<p>

*Gets the latest trades on a symbol*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.GetTradeHistoryAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<IEnumerable<CoinExSocketSymbolTrade>>> GetTradeHistoryAsync(string symbol, int limit, int? fromId = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to get the trades for|
|limit|The limit of trades|
|_[Optional]_ fromId|Return trades since this id|

</p>

***

## PingAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/048ping](https://github.com/coinexcom/coinex_exchange_api/wiki/048ping)  
<p>

*Pings the server*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.PingAsync();  
```  

```csharp  
Task<CallResult<bool>> PingAsync();  
```  

|Parameter|Description|
|---|---|

</p>

***

## SubscribeToAllTickerUpdatesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/053state](https://github.com/coinexcom/coinex_exchange_api/wiki/053state)  
<p>

*Subscribe to ticker updates for all symbols*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.SubscribeToAllTickerUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToAllTickerUpdatesAsync(Action<DataEvent<IEnumerable<CoinExSocketSymbolState>>> onMessage, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|onMessage|Data handler|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToBalanceUpdatesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/051asset](https://github.com/coinexcom/coinex_exchange_api/wiki/051asset)  
<p>

*Subscribe to updates of your balances, Receives updates whenever the balance for an asset changes*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.SubscribeToBalanceUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToBalanceUpdatesAsync(Action<DataEvent<IEnumerable<CoinExBalance>>> onMessage, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|onMessage|Data handler|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToKlineUpdatesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/046kline](https://github.com/coinexcom/coinex_exchange_api/wiki/046kline)  
<p>

*Subscribe to kline updates for a symbol*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.SubscribeToKlineUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<IEnumerable<CoinExKline>>> onMessage, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to receive updates for|
|interval|The interval of the candle to receive updates for|
|onMessage|Data handler|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToOrderBookUpdatesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/044depth](https://github.com/coinexcom/coinex_exchange_api/wiki/044depth)  
<p>

*Subscribe to order book updates*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.SubscribeToOrderBookUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToOrderBookUpdatesAsync(string symbol, int limit, int mergeDepth, Action<DataEvent<CoinExSocketOrderBook>> onMessage, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to receive updates for|
|limit|The limit of results to receive in a update|
|mergeDepth|The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together|
|onMessage|Data handler|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToOrderUpdatesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/052order](https://github.com/coinexcom/coinex_exchange_api/wiki/052order)  
<p>

*Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.SubscribeToOrderUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|onMessage|Data handler|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToOrderUpdatesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/052order](https://github.com/coinexcom/coinex_exchange_api/wiki/052order)  
<p>

*Subscribe to updates of active orders. Receives updates whenever an order is placed, updated or finished*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.SubscribeToOrderUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToOrderUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExSocketOrderUpdate>> onMessage, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbols|The symbols to receive order updates from|
|onMessage|Data handler|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToTickerUpdatesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/053state](https://github.com/coinexcom/coinex_exchange_api/wiki/053state)  
<p>

*Subscribe to symbol ticker updates for a specific symbol*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.SubscribeToTickerUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(string symbol, Action<DataEvent<CoinExSocketSymbolState>> onMessage, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|Symbol to receive updates for|
|onMessage|Data handler|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>

***

## SubscribeToTradeUpdatesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/045deals](https://github.com/coinexcom/coinex_exchange_api/wiki/045deals)  
<p>

*Subscribe to symbol trade updates for a symbol*  

```csharp  
var client = new CoinExSocketClient();  
var result = await client.SpotStreams.SubscribeToTradeUpdatesAsync(/* parameters */);  
```  

```csharp  
Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<IEnumerable<CoinExSocketSymbolTrade>>> onMessage, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to receive updates from|
|onMessage|Data handler|
|_[Optional]_ ct|Cancellation token for closing this subscription|

</p>
