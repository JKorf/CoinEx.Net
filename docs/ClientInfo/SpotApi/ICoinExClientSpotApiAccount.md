---
title: ICoinExClientSpotApiAccount
has_children: false
parent: ICoinExClientSpotApi
grand_parent: Rest API documentation
---
*[generated documentation]*  
`CoinExClient > SpotApi > Account`  
*CoinEx account endpoints. Account endpoints include balance info, withdraw/deposit info and requesting and account settings*
  

***

## CancelWithdrawalAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/064cancel_withdraw](https://github.com/coinexcom/coinex_exchange_api/wiki/064cancel_withdraw)  
<p>

*Cancel a specific withdrawal. Requires API credentials and withdrawal permission on the API key*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Account.CancelWithdrawalAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<bool>> CancelWithdrawalAsync(long withdrawId, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|withdrawId|The id of the withdrawal to cancel|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetBalancesAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/060balance](https://github.com/coinexcom/coinex_exchange_api/wiki/060balance)  
<p>

*Retrieves a list of balances. Requires API credentials*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Account.GetBalancesAsync();  
```  

```csharp  
Task<WebCallResult<Dictionary<string, CoinExBalance>>> GetBalancesAsync(CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetDepositAddressAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/072get_deposit_address](https://github.com/coinexcom/coinex_exchange_api/wiki/072get_deposit_address)  
<p>

*Get the deposit address of an asset*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Account.GetDepositAddressAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExDepositAddress>> GetDepositAddressAsync(string asset, string? smartContractName = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|The asset to deposit|
|_[Optional]_ smartContractName|Name of the network to use|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetDepositHistoryAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/065get_deposit_list](https://github.com/coinexcom/coinex_exchange_api/wiki/065get_deposit_list)  
<p>

*Retrieves a list of deposits. Requires API credentials and withdrawal permission on the API key*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Account.GetDepositHistoryAsync();  
```  

```csharp  
Task<WebCallResult<CoinExPagedResult<CoinExDeposit>>> GetDepositHistoryAsync(string? asset = default, int? page = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ asset|The asset to get history for|
|_[Optional]_ page|The page in the results to retrieve|
|_[Optional]_ limit|The number of results to return per page|
|_[Optional]_ ct|Cancellation token|

</p>

***

## GetWithdrawalHistoryAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/061get_withdraw_list](https://github.com/coinexcom/coinex_exchange_api/wiki/061get_withdraw_list)  
<p>

*Retrieves a list of withdrawals. Requires API credentials and withdrawal permission on the API key*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Account.GetWithdrawalHistoryAsync();  
```  

```csharp  
Task<WebCallResult<CoinExPagedResult<CoinExWithdrawal>>> GetWithdrawalHistoryAsync(string? asset = default, long? withdrawId = default, int? page = default, int? limit = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|_[Optional]_ asset|The asset to get history for|
|_[Optional]_ withdrawId|Retrieve a withdrawal with a specific id|
|_[Optional]_ page|The page in the results to retrieve|
|_[Optional]_ limit|The number of results to return per page|
|_[Optional]_ ct|Cancellation token|

</p>

***

## WithdrawAsync  

[https://github.com/coinexcom/coinex_exchange_api/wiki/062submit_withdraw](https://github.com/coinexcom/coinex_exchange_api/wiki/062submit_withdraw)  
<p>

*Withdraw assets from CoinEx to a specific address. Requires API credentials and withdrawal permission on the API key*  

```csharp  
var client = new CoinExClient();  
var result = await client.SpotApi.Account.WithdrawAsync(/* parameters */);  
```  

```csharp  
Task<WebCallResult<CoinExWithdrawal>> WithdrawAsync(string asset, string address, bool localTransfer, decimal quantity, string? network = default, CancellationToken ct = default);  
```  

|Parameter|Description|
|---|---|
|asset|The asset to withdraw|
|address|The address to withdraw to|
|localTransfer|Is it a local transfer between users or onchain|
|quantity|The quantity to withdraw. This is the quantity AFTER fees have been deducted. For fee rates see https://www.coinex.com/fees |
|_[Optional]_ network|The network to use for the withdrawal|
|_[Optional]_ ct|Cancellation token|

</p>
