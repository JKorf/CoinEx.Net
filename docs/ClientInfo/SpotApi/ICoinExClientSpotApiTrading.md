---
title: ICoinExClientSpotApiTrading
has_children: false
parent: ICoinExClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`CoinExClient > SpotApi > Trading`  
*CoinEx trading endpoints, placing and mananging orders.*
  

***

## CancelAllOrdersAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/0315cancel_all](https://github.com/coinexcom/coinex_exchange_api/wiki/0315cancel_all)  
<p>

*Cancels all orders. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.CancelAllOrdersAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult> CancelAllOrdersAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol the orders are on|
|_[Optional]_ ct|Cancellation token|

</p>

***

## CancelAllStopOrdersAsync  

<p>

*Cancels all stop orders. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.CancelAllStopOrdersAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult> CancelAllStopOrdersAsync(string symbol, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol the orders are on|
|_[Optional]_ ct|Cancellation token|

</p>

***

## CancelOrderAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/035cancel](https://github.com/coinexcom/coinex_exchange_api/wiki/035cancel)  
<p>

*Cancels an order. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.CancelOrderAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol the order is on|
|orderId|The id of the order to cancel|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetClosedOrdersAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/036finished](https://github.com/coinexcom/coinex_exchange_api/wiki/036finished)  
<p>

*Retrieves a list of executed orders for a symbol in the last 2 days. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.GetClosedOrdersAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetClosedOrdersAsync(string symbol, int? page = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to retrieve the open orders for|
|_[Optional]_ page|The page of the resulting list|
|_[Optional]_ limit|The number of results per page|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOpenOrdersAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/034pending](https://github.com/coinexcom/coinex_exchange_api/wiki/034pending)  
<p>

*Retrieves a list of open orders for a symbol. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.GetOpenOrdersAsync();  
```  

```csharp  
Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string? symbol = default, int? page = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ symbol|The symbol to retrieve the open orders for|
|_[Optional]_ page|The page of the resulting list|
|_[Optional]_ limit|The number of results per page|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOpenStopOrdersAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/041stop_pending](https://github.com/coinexcom/coinex_exchange_api/wiki/041stop_pending)  
<p>

*Retrieves a list of open stop orders for a symbol. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.GetOpenStopOrdersAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenStopOrdersAsync(string symbol, int? page = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to retrieve the open orders for|
|_[Optional]_ page|The page of the resulting list|
|_[Optional]_ limit|The number of results per page|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOrderAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/037order_status](https://github.com/coinexcom/coinex_exchange_api/wiki/037order_status)  
<p>

*Retrieves details of an order. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.GetOrderAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol the order is for|
|orderId|The id of the order to retrieve|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetOrderTradesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/0311order_deals](https://github.com/coinexcom/coinex_exchange_api/wiki/0311order_deals)  
<p>

*Retrieves execution details of a specific order. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.GetOrderTradesAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExPagedResult<CoinExOrderTrade>>> GetOrderTradesAsync(long orderId, int? page = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|orderId|The id of the order|
|_[Optional]_ page|The page of the resulting list|
|_[Optional]_ limit|The number of results per page|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetUserTradesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/039user_deals](https://github.com/coinexcom/coinex_exchange_api/wiki/039user_deals)  
<p>

*Gets a list of trades you executed on a specific symbol. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.GetUserTradesAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExPagedResult<CoinExOrderTradeExtended>>> GetUserTradesAsync(string symbol, int? page = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to retrieve trades for|
|_[Optional]_ page|The page of the resulting list|
|_[Optional]_ limit|The number of results per page|
|_[Optional]_ ct|Cancellation token|

</p>

***

## PlaceOrderAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/031limit_order](https://github.com/coinexcom/coinex_exchange_api/wiki/031limit_order)  
[https://github.com/coinexcom/coinex_exchange_api/wiki/032market_order](https://github.com/coinexcom/coinex_exchange_api/wiki/032market_order)  
[https://github.com/coinexcom/coinex_exchange_api/wiki/03111stop_limit_order](https://github.com/coinexcom/coinex_exchange_api/wiki/03111stop_limit_order)  
[https://github.com/coinexcom/coinex_exchange_api/wiki/033IOC_order](https://github.com/coinexcom/coinex_exchange_api/wiki/033IOC_order)  
<p>

*Places an order. This is a single method for multiple place order endpoints. The called endpoint depends on the provided order type.*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Trading.PlaceOrderAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExOrder>> PlaceOrderAsync(string symbol, OrderSide side, OrderType type, decimal quantity, decimal? price = default, decimal? stopPrice = default, bool? immediateOrCancel = default, OrderOption? orderOption = default, string? clientOrderId = default, string? sourceId = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|symbol|The symbol to place the order for|
|side|Order side|
|type|Order type|
|quantity|The quantity of the order|
|_[Optional]_ price|The price of a single unit of the order|
|_[Optional]_ stopPrice|The stop-price of a single unit of the order|
|_[Optional]_ immediateOrCancel|True if the order should be filled immediately up on placing, otherwise it will be canceled|
|_[Optional]_ orderOption|Option for the order|
|_[Optional]_ clientOrderId|Client id which can be used to match the order|
|_[Optional]_ sourceId|User defined number|
|_[Optional]_ ct|Cancellation token|

</p>
