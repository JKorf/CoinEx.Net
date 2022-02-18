---
title: ICoinExClientSpotApiExchangeData
has_children: false
parent: ICoinExClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`CoinExClient > SpotApi > ExchangeData`  
*CoinEx exchange data endpoints. Exchange data includes market data (tickers, order books, etc) and system status.*
  

***

## GetAssetsAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/071asset_config](https://github.com/coinexcom/coinex_exchange_api/wiki/071asset_config)  
<p>

*Gets the asset configs*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetAssetsAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, CoinExAssetConfig>>> GetAssetsAsync(string? assetType = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ assetType|Optionally only return a certain type of asset, for example BCH|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetCurrencyRateAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/070currency_rate](https://github.com/coinexcom/coinex_exchange_api/wiki/070currency_rate)  
<p>

*Gets the exchange rates of currencies*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetCurrencyRateAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, decimal>>> GetCurrencyRateAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetKlinesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/024kline](https://github.com/coinexcom/coinex_exchange_api/wiki/024kline)  
<p>

*Retrieves kline data for a specific symbol*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetKlinesAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<CoinExKline>>> GetKlinesAsync(string symbol, KlineInterval interval, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to retrieve klines for|
|interval|The interval of the candles|
|_[Optional]_ limit|Limit of the number of results|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetMiningDifficultyAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/0310mining_difficulty](https://github.com/coinexcom/coinex_exchange_api/wiki/0310mining_difficulty)  
<p>

*Retrieve the mining difficulty. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetMiningDifficultyAsync();  
```  

```csharp  
Task<WebCallResult<CoinExMiningDifficulty>> GetMiningDifficultyAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOrderBookAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/022depth](https://github.com/coinexcom/coinex_exchange_api/wiki/022depth)  
<p>

*Gets the order book for a symbol*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetOrderBookAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExOrderBook>> GetOrderBookAsync(string symbol, int mergeDepth, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to retrieve depth data for|
|mergeDepth|The depth of merging, based on 8 decimals. 1 mergeDepth will merge the last decimals of all order in the book, 7 will merge the last 7 decimals of all orders together|
|_[Optional]_ limit|The limit of results returned|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetSymbolInfoAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/026market_single_info](https://github.com/coinexcom/coinex_exchange_api/wiki/026market_single_info)  
<p>

*Retrieves market data for the exchange*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetSymbolInfoAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to retrieve data for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetSymbolInfoAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/025marketinfo](https://github.com/coinexcom/coinex_exchange_api/wiki/025marketinfo)  
<p>

*Retrieves market data for the exchange*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetSymbolInfoAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, CoinExSymbol>>> GetSymbolInfoAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetSymbolsAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/020market](https://github.com/coinexcom/coinex_exchange_api/wiki/020market)  
<p>

*Gets a list of symbols active on CoinEx*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetSymbolsAsync();  
```  

```csharp  
Task<WebCallResult<IEnumerable<string>>> GetSymbolsAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTickerAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/021ticker](https://github.com/coinexcom/coinex_exchange_api/wiki/021ticker)  
<p>

*Gets the state of a specific symbol*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetTickerAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExSymbolState>> GetTickerAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to retrieve state for|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTickersAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/021ticker](https://github.com/coinexcom/coinex_exchange_api/wiki/021ticker)  
<p>

*Gets the states of all symbols*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetTickersAsync();  
```  

```csharp  
Task<WebCallResult<CoinExSymbolStatesList>> GetTickersAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetTradeHistoryAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/023deals](https://github.com/coinexcom/coinex_exchange_api/wiki/023deals)  
<p>

*Gets the latest trades for a symbol*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.ExchangeData.GetTradeHistoryAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<IEnumerable<CoinExSymbolTrade>>> GetTradeHistoryAsync(string symbol, long? fromId = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to retrieve data for|
|_[Optional]_ fromId|The id from which on to return trades|
|_[Optional]_ ct|Cancellation token|

</p>
