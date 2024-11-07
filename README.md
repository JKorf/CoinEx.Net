# ![.CoinEx.Net](https://github.com/JKorf/CoinEx.Net/blob/master/CoinEx.Net/Icon/icon.png?raw=true) CoinEx.Net

[![.NET](https://img.shields.io/github/actions/workflow/status/JKorf/CoinEx.Net/dotnet.yml?style=for-the-badge)](https://github.com/JKorf/CoinEx.Net/actions/workflows/dotnet.yml) ![License](https://img.shields.io/github/license/JKorf/CoinEx.Net?style=for-the-badge)

CoinEx.Net is a strongly typed client library for accessing the [CoinEx REST and Websocket API](https://github.com/coinexcom/coinex_exchange_api/wiki).

## Features
* Response data is mapped to descriptive models
* Input parameters and response values are mapped to discriptive enum values where possible
* Automatic websocket (re)connection management 
* Client side order book implementation
* Extensive logging
* Support for different environments
* Easy integration with other exchange client based on the CryptoExchange.Net base library

## Supported Frameworks
The library is targeting both `.NET Standard 2.0` and `.NET Standard 2.1` for optimal compatibility

|.NET implementation|Version Support|
|--|--|
|.NET Core|`2.0` and higher|
|.NET Framework|`4.6.1` and higher|
|Mono|`5.4` and higher|
|Xamarin.iOS|`10.14` and higher|
|Xamarin.Android|`8.0` and higher|
|UWP|`10.0.16299` and higher|
|Unity|`2018.1` and higher|

## Install the library

### NuGet 
[![NuGet version](https://img.shields.io/nuget/v/CoinEx.net.svg?style=for-the-badge)](https://www.nuget.org/packages/CoinEx.Net)  [![Nuget downloads](https://img.shields.io/nuget/dt/CoinEx.Net.svg?style=for-the-badge)](https://www.nuget.org/packages/CoinEx.Net)

	dotnet add package CoinEx.Net
	
### GitHub packages
CoinEx.Net is available on [GitHub packages](https://github.com/JKorf/CoinEx.Net/pkgs/nuget/CoinEx.Net). You'll need to add `https://nuget.pkg.github.com/JKorf/index.json` as a NuGet package source.

### Download release
[![GitHub Release](https://img.shields.io/github/v/release/JKorf/CoinEx.Net?style=for-the-badge&label=GitHub)](https://github.com/JKorf/CoinEx.Net/releases)

The NuGet package files are added along side the source with the latest GitHub release which can found [here](https://github.com/JKorf/CoinEx.Net/releases).

## How to use
*REST Endpoints*  

```csharp
// Get the ETH/USDT ticker via rest request
var restClient = new CoinExRestClient();
var tickerResult = await restClient.SpotApiV2.ExchangeData.GetTickersAsync(new [] { "ETHUSDT" });
var lastPrice = tickerResult.Data.LastPrice;
```

*Websocket streams*  

```csharp
// Subscribe to ETH/USDT ticker updates via the websocket API
var socketClient = new CoinExSocketClient();
var tickerSubscriptionResult = socketClient.SpotApiV2.SubscribeToTickerUpdatesAsync(new [] { "ETHUSDT" }, (update) =>
{
	var lastPrice = update.Data.First().LastPrice;
});
```

For information on the clients, dependency injection, response processing and more see the [CoinEx.Net documentation](https://jkorf.github.io/CoinEx.Net), [CryptoExchange.Net documentation](https://jkorf.github.io/CryptoExchange.Net), or have a look at the examples [here](https://github.com/JKorf/CoinEx.Net/tree/master/Examples) or [here](https://github.com/JKorf/CryptoExchange.Net/tree/master/Examples).

## CryptoExchange.Net
CoinEx.Net is based on the [CryptoExchange.Net](https://github.com/JKorf/CryptoExchange.Net) base library. Other exchange API implementations based on the CryptoExchange.Net base library are available and follow the same logic.

CryptoExchange.Net also allows for [easy access to different exchange API's](https://jkorf.github.io/CryptoExchange.Net#idocs_shared).

|Exchange|Repository|Nuget|
|--|--|--|
|Binance|[JKorf/Binance.Net](https://github.com/JKorf/Binance.Net)|[![Nuget version](https://img.shields.io/nuget/v/Binance.net.svg?style=flat-square)](https://www.nuget.org/packages/Binance.Net)|
|BingX|[JKorf/BingX.Net](https://github.com/JKorf/BingX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.BingX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.BingX.Net)|
|Bitfinex|[JKorf/Bitfinex.Net](https://github.com/JKorf/Bitfinex.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bitfinex.net.svg?style=flat-square)](https://www.nuget.org/packages/Bitfinex.Net)|
|Bitget|[JKorf/Bitget.Net](https://github.com/JKorf/Bitget.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Bitget.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Bitget.Net)|
|BitMart|[JKorf/BitMart.Net](https://github.com/JKorf/BitMart.Net)|[![Nuget version](https://img.shields.io/nuget/v/BitMart.net.svg?style=flat-square)](https://www.nuget.org/packages/BitMart.Net)|
|Bybit|[JKorf/Bybit.Net](https://github.com/JKorf/Bybit.Net)|[![Nuget version](https://img.shields.io/nuget/v/Bybit.net.svg?style=flat-square)](https://www.nuget.org/packages/Bybit.Net)|
|Coinbase|[JKorf/Coinbase.Net](https://github.com/JKorf/Coinbase.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.Coinbase.Net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.Coinbase.Net)|
|CoinGecko|[JKorf/CoinGecko.Net](https://github.com/JKorf/CoinGecko.Net)|[![Nuget version](https://img.shields.io/nuget/v/CoinGecko.net.svg?style=flat-square)](https://www.nuget.org/packages/CoinGecko.Net)|
|Crypto.com|[JKorf/CryptoCom.Net](https://github.com/JKorf/CryptoCom.Net)|[![Nuget version](https://img.shields.io/nuget/v/CryptoCom.net.svg?style=flat-square)](https://www.nuget.org/packages/CryptoCom.Net)|
|Gate.io|[JKorf/GateIo.Net](https://github.com/JKorf/GateIo.Net)|[![Nuget version](https://img.shields.io/nuget/v/GateIo.net.svg?style=flat-square)](https://www.nuget.org/packages/GateIo.Net)|
|HTX|[JKorf/HTX.Net](https://github.com/JKorf/HTX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JKorf.HTX.Net.svg?style=flat-square)](https://www.nuget.org/packages/JKorf.HTX.Net)|
|Kraken|[JKorf/Kraken.Net](https://github.com/JKorf/Kraken.Net)|[![Nuget version](https://img.shields.io/nuget/v/KrakenExchange.net.svg?style=flat-square)](https://www.nuget.org/packages/KrakenExchange.Net)|
|Kucoin|[JKorf/Kucoin.Net](https://github.com/JKorf/Kucoin.Net)|[![Nuget version](https://img.shields.io/nuget/v/Kucoin.net.svg?style=flat-square)](https://www.nuget.org/packages/Kucoin.Net)|
|Mexc|[JKorf/Mexc.Net](https://github.com/JKorf/Mexc.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.Mexc.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.Mexc.Net)|
|OKX|[JKorf/OKX.Net](https://github.com/JKorf/OKX.Net)|[![Nuget version](https://img.shields.io/nuget/v/JK.OKX.net.svg?style=flat-square)](https://www.nuget.org/packages/JK.OKX.Net)|
|WhiteBit|[JKorf/WhiteBit.Net](https://github.com/JKorf/WhiteBit.Net)|[![Nuget version](https://img.shields.io/nuget/v/WhiteBit.net.svg?style=flat-square)](https://www.nuget.org/packages/WhiteBit.Net)|

## Discord
[![Nuget version](https://img.shields.io/discord/847020490588422145?style=for-the-badge)](https://discord.gg/MSpeEtSY8t)  
A Discord server is available [here](https://discord.gg/MSpeEtSY8t). Feel free to join for discussion and/or questions around the CryptoExchange.Net and implementation libraries.

## Supported functionality

### Spot Api V1
|API|Supported|Location|
|--|--:|--|
|Market Data API|✓|`restClient.SpotApi.ExchangeData`|
|Account API|✓|`restClient.SpotApi.Account`|
|Trading API|✓|`restClient.SpotApi.Trading`|
|Websocket API|✓|`socketClient.SpotApi`|

### Futures Api V1
|API|Supported|Location|
|--|--:|--|
|*|X||

### V2 Account
|API|Supported|Location|
|--|--:|--|
|Sub-Account|X||
|Fee Rate|✓|`restClient.SpotApiV2.ExchangeData`|
|Set|✓|`restClient.SpotApiV2.Account`|

### V2 Asset
|API|Supported|Location|
|--|--:|--|
|Balance rest|✓|`restClient.SpotApiV2.Account`|
|Balance websocket|✓|`socketClient.SpotApiV2`|
|Loan & Repayment|✓|`restClient.SpotApiV2.Account`|
|Deposit & Withdrawal|✓|`restClient.SpotApiV2.Account`|
|Transfer|✓|`restClient.SpotApiV2.Account`|
|Aam|✓|`restClient.SpotApiV2.Account`|

### V2 Spot
|API|Supported|Location|
|--|--:|--|
|Ticker rest|✓|`restClient.SpotApiV2.ExchangeData`|
|Ticker websocket|✓|`socketClient.SpotApiV2`|
|Orders rest|✓|`restClient.SpotApiV2.Trading`|
|Orders websocket|✓|`socketClient.SpotApiV2`|
|Executions rest|✓|`restClient.SpotApiV2.Trading`|
|Executions websocket|✓|`socketClient.SpotApiV2`|

### V2 Futures
|API|Supported|Location|
|--|--:|--|
|Ticker rest|✓|`restClient.SpotApiV2.ExchangeData`|
|Ticker websocket|✓|`socketClient.SpotApiV2`|
|Orders rest|✓|`restClient.SpotApiV2.Trading`|
|Orders websocket|✓|`socketClient.SpotApiV2`|
|Executions rest|✓|`restClient.SpotApiV2.Trading`|
|Executions websocket|✓|`socketClient.SpotApiV2`|
|Position rest|✓|`restClient.SpotApiV2.Trading`|
|Position websocket|✓|`socketClient.SpotApiV2`|

## Support the project
Any support is greatly appreciated.

### Donate
Make a one time donation in a crypto currency of your choice. If you prefer to donate a currency not listed here please contact me.

**Btc**:  bc1q277a5n54s2l2mzlu778ef7lpkwhjhyvghuv8qf  
**Eth**:  0xcb1b63aCF9fef2755eBf4a0506250074496Ad5b7   
**USDT (TRX)**  TKigKeJPXZYyMVDgMyXxMf17MWYia92Rjd

### Sponsor
Alternatively, sponsor me on Github using [Github Sponsors](https://github.com/sponsors/JKorf). 

## Release notes
* Version 7.9.0 - 06 Nov 2024
    * Updated CryptoExchange.Net to version 8.2.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.2.0

* Version 7.8.0 - 28 Oct 2024
    * Updated CryptoExchange.Net to version 8.1.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.1.0
    * Moved FormatSymbol to CoinExExchange class
    * Added support Side setting on SharedTrade model
    * Added CoinExTrackerFactory for creating trackers
    * Added overload to Create method on CoinExOrderBookFactory support SharedSymbol parameter

* Version 7.7.2 - 14 Oct 2024
    * Updated CryptoExchange.Net to version 8.0.3, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.0.3
    * Fixed TypeLoadException during initialization

* Version 7.7.1 - 14 Oct 2024
    * Fixed Shared interface REST spot order quantity parsing
    * Fixed Shared interface REST spot order status parsing

* Version 7.7.0 - 27 Sep 2024
    * Updated CryptoExchange.Net to version 8.0.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/8.0.0
    * Added Shared client interfaces implementation for Spot and Futures Rest and Socket clients
    * Added memo parameter to SpotApi.Account.WithdrawAsync
    * Added Role property to CoinExUserTrade model
    * Updated Sourcelink package version
    * Updated QuantityPrecision and PricePrecision property types from decimal to int on CoinExFuturesSymbol
    * Fixed Quantity property type from long to decimal in CoinExDeposit model
    * Fixed QuantityCredited property type from long to decimal? in CoinExDepositModel
    * Fixed FuturesApi.SubscribeToTickerUpdatesAsync subscription
    * Marked ISpotClient references as deprecated

* Version 7.6.1 - 11 Sep 2024
    * Added SpotApiV2.Account.GetTransactionHistoryAsync endpoint

* Version 7.6.0 - 19 Aug 2024
    * Added futures API batch endpoints:FuturesApi.Trading.PlaceMultipleOrdersAsync, PlaceMultipleStopOrdersAsync, CancelOrdersAsync and CancelStopOrdersAsync
    * Added spot API batch endpoints: SpotApiV2.Trading.PlaceMultipleOrdersAsync, PlaceMultipleStopOrdersAsync, CancelOrdersAsync and CancelStopOrdersAsync
    * Added stpMode paramaters to spot and futures PlaceOrderAsync and PlaceStopOrderAsync endpoints

* Version 7.5.0 - 07 Aug 2024
    * Updated CryptoExchange.Net to version 7.11.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.11.0
    * Updated XML code comments
    * Added deprecation notice to V1 Spot API

* Version 7.4.0 - 27 Jul 2024
    * Updated CryptoExchange.Net to version 7.10.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.10.0

* Version 7.3.0 - 16 Jul 2024
    * Updated CryptoExchange.Net to version 7.9.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.9.0
    * Updated internal classes to internal access modifier
    * Added SpotApiV2.ExchangeData.GetAssetsAsync endpoint

* Version 7.2.1 - 02 Jul 2024
    * Updated CryptoExchange.Net to V7.8.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.8.0
    * Added FuturesApi.ExchangeData.GetPremiumIndexPriceHistoryAsync endpoint

* Version 7.2.0 - 23 Jun 2024
    * Updated CryptoExchange.Net to version 7.7.0, see https://github.com/JKorf/CryptoExchange.Net/releases/tag/7.7.0
    * Updated response models from classes to records

* Version 7.1.0 - 11 Jun 2024
    * Updated CryptoExchange.Net to v7.6.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.0.5 - 07 May 2024
    * Updated CryptoExchange.Net to v7.5.2, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.0.4 - 01 May 2024
    * Updated CryptoExchange.Net to v7.5.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.0.3 - 28 Apr 2024
    * Added CoinExExchange static info class
    * Added CoinExOrderBookFactory book creation method
    * Updated signature generation based on API docs update
    * Fixed CoinExOrderBookFactory injection issue
    * Updated CryptoExchange.Net to v7.4.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.0.2 - 23 Apr 2024
    * Updated CryptoExchange.Net to 7.3.3, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.0.1 - 18 Apr 2024
    * Updated CryptoExchange.Net to 7.3.1, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 7.0.0 - 06 Apr 2024
    * Added V2 Spot API implementation
    * Added Futures implementation

* Version 6.2.2 - 03 Apr 2024
    * Added parameter for SubscribeToOrderBookUpdatesAsync for full or dif updates
    * Updated string comparision for improved performance
    * Removed pre-send symbol validation

* Version 6.2.1 - 24 Mar 2024
	* Updated CryptoExchange.Net to 7.2.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes

* Version 6.2.0 - 16 Mar 2024
    * Updated CryptoExchange.Net to 7.1.0, see https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes for release notes
	
* Version 6.1.1 - 26 Feb 2024
    * Fixed order subscription with symbol parameters

* Version 6.1.0 - 25 Feb 2024
    * Updated CryptoExchange.Net and implemented reworked websocket message handling. For release notes for the CryptoExchange.Net base library see: https://github.com/JKorf/CryptoExchange.Net?tab=readme-ov-file#release-notes
    * Fixed issue in DI registration causing http client to not be correctly injected
    * Made various parameters in CoinExSocketClient optional
    * Removed redundant CoinExRestClient constructor overload
    * Updated some namespaces

* Version 6.0.6 - 03 Dec 2023
    * Updated CryptoExchange.Net

* Version 6.0.5 - 14 Nov 2023
    * Fix for broker reference

* Version 6.0.4 - 29 Oct 2023
    * Added broker reference option

* Version 6.0.3 - 24 Oct 2023
    * Updated CryptoExchange.Net

* Version 6.0.2 - 09 Oct 2023
    * Updated CryptoExchange.Net version
    * Added ISpotClient to DI injection

* Version 6.0.1 - 25 Aug 2023
    * Updated CryptoExchange.Net

* Version 6.0.0 - 25 Jun 2023
    * Updated CryptoExchange.Net to version 6.0.0
    * Renamed CoinExClient to CoinExRestClient
    * Renamed SpotStreams to SpotApi on the CoinExSocketClient
    * Updated endpoints to consistently use a base url without any path as basis to make switching environments/base urls clearer
    * Added ICoinExOrderBookFactory and implementation for creating order books
    * Updated dependency injection register method (AddCoinEx)

* Version 5.1.2 - 18 Mar 2023
    * Updated CryptoExchange.Net

* Version 5.1.1 - 14 Feb 2023
    * Updated CryptoExchange.Net
    * Fixed issue with request signing with parameters containing special characters

* Version 5.1.0 - 17 Nov 2022
    * Updated CryptoExchange.Net

* Version 5.0.15 - 18 Jul 2022
    * Updated CryptoExchange.Net

* Version 5.0.14 - 16 Jul 2022
    * Updated CryptoExchange.Net

* Version 5.0.13 - 10 Jul 2022
    * Updated CryptoExchange.Net

* Version 5.0.12 - 12 Jun 2022
    * Updated CryptoExchange.Net

* Version 5.0.11 - 24 May 2022
    * Updated CryptoExchange.Net

* Version 5.0.10 - 22 May 2022
    * Updated CryptoExchange.Net

* Version 5.0.9 - 08 May 2022
    * Updated CryptoExchange.Net

* Version 5.0.8 - 01 May 2022
    * Updated CryptoExchange.Net which fixed an timing related issue in the websocket reconnection logic
    * Added seconds representation to KlineInterval enum

* Version 5.0.7 - 14 Apr 2022
    * Updated CryptoExchange.Net

* Version 5.0.6 - 14 Mar 2022
    * Fixed stopPrice serialization in PlaceOrderAsync

* Version 5.0.5 - 10 Mar 2022
    * Updated CryptoExchange.Net

* Version 5.0.4 - 08 Mar 2022
    * Updated CryptoExchange.Net

* Version 5.0.3 - 01 Mar 2022
    * Updated CryptoExchange.Net improving the websocket reconnection robustness

* Version 5.0.2 - 27 Feb 2022
    * Updated CryptoExchange.Net

* Version 5.0.1 - 24 Feb 2022
    * Updated CryptoExchange.Net

* Version 5.0.0 - 18 Feb 2022
	* Added Github.io page for documentation: https://jkorf.github.io/CoinEx.Net/
	*  unit tests for parsing the returned JSON for each endpoint and subscription
	* Added AddCoinEx extension method on IServiceCollection for easy dependency injection
	* Added URL reference to API endpoint documentation for each endpoint
	* Added default rate limiter

	* Refactored different PlaceOrderAsync methods to a single PlaceOrderAsync method to be consistent across exchange implementations
	* Refactored client structure to be consistent across exchange implementations
	* Renamed various properties to be consistent across exchange implementations

	* Cleaned up project structure
	* Fixed various models

	* Updated CryptoExchange.Net, see https://github.com/JKorf/CryptoExchange.Net#release-notes
	* See https://jkorf.github.io/CoinEx.Net/MigrationGuide.html for additional notes for updating from V4 to V5

* Version 4.2.4 - 03 Nov 2021
    * Updated CoinExAssetConfig model
    * Fixed typo in OrderOptionConverter

* Version 4.2.3 - 08 Oct 2021
    * Updated CryptoExchange.Net to fix some socket issues

* Version 4.2.2 - 06 Oct 2021
    * Updated CryptoExchange.Net, fixing socket issue when calling from .Net Framework

* Version 4.2.1 - 05 Oct 2021
    * Added GetOpenStopOrdersAsync endpoint
    * Added CancelAllStopOrdersAsync endpoint

* Version 4.2.0 - 29 Sep 2021
    * Fixed DELETE endpoints
    * Changed GetBalances parameter from `params` to `IEnumerable`
    * Updated CryptoExchange.Net

* Version 4.1.2 - 22 Sep 2021
    * Fixed nonce provider when running multiple program instances

* Version 4.1.1 - 21 Sep 2021
    * Fix for nonce provider not working correctly in combination with other exchanges

* Version 4.1.0 - 20 Sep 2021
    * Added custom nonce provider support
    * Added PlaceStopMarketOrderAsync endpoint
    * Added missing SetApiCredentials method
    * Updated CryptoExchange.Net

* Version 4.0.6 - 15 Sep 2021
    * Updated CryptoExchange.Net

* Version 4.0.5 - 02 Sep 2021
    * Fix for disposing order book closing socket even if there are other connections

* Version 4.0.4 - 26 Aug 2021
    * Updated CryptoExchange.Net

* Version 4.0.3 - 26 Aug 2021
    * Added PlaceStopLimitOrderAsync endpoint

* Version 4.0.2 - 24 Aug 2021
    * Updated CryptoExchange.Net, improving websocket and SymbolOrderBook performance

* Version 4.0.1 - 13 Aug 2021
    * Fix for OperationCancelledException being thrown when closing a socket from a .net framework project

* Version 4.0.0 - 12 Aug 2021
	* Release version with new CryptoExchange.Net version 4.0.0
		* Multiple changes regarding logging and socket connection, see [CryptoExchange.Net release notes](https://github.com/JKorf/CryptoExchange.Net#release-notes)
		
* Version 4.0.0-beta3 - 09 Aug 2021
    * Renamed GetSymbolTradesAsync to GetTradesHistoryAsync
    * Renamed GetExecutedOrderDetailsAsync to GetOrderTradesAsync
    * Renamed GetOrderStatusAsync to GetOrderAsync
    * Renamed GetTradesAsync to GetUserTradesAsync

* Version 4.0.0-beta2 - 26 Jul 2021
    * Updated CryptoExchange.Net

* Version 4.0.0-beta1 - 09 Jul 2021
    * Added Async postfix for async methods
    * Updated CryptoExchange.Net

* Version 3.3.0-beta10 - 15 Jun 2021
    * WithrawAsync fixed

* Version 3.3.0-beta9 - 14 Jun 2021
    * Fixed typo in WithdrawAsync

* Version 3.3.0-beta8 - 07 Jun 2021
    * Fixed GetWithdrawalHistory
    * Updated CryptoExchange.Net

* Version 3.3.0-beta7 - 03 Jun 2021
    * Fixed order subscription (again)

* Version 3.3.0-beta6 - 03 Jun 2021
    * Added ClientId to order update model
    * Fixed order subscription parameters

* Version 3.3.0-beta5 - 02 Jun 2021
    * Added optional PlaceLimitOrderAsync parameters
    * Fix for WithdrawAsync

* Version 3.3.0-beta4 - 02 Jun 2021
    * Added GetCurrencyRateAsync endpoint
    * Added GetAssetConfigAsync endpoint
    * Added GetDepositAddressAsync

* Version 3.3.0-beta3 - 26 May 2021
    * Removed non-async calls
    * Updated to CryptoExchange.Net changes

* Version 3.3.0-beta2 - 06 mei 2021
    * Updated CryptoExchange.Net

* Version 3.3.0-beta1 - 30 apr 2021
    * Updated to CryptoExchange.Net 4.0.0-beta1, new websocket implementation
	
* Version 3.2.6 - 04 mei 2021
    * Fix for trades subscription deserialization when extra array item is received
    * Fix parameter type in Withdraw method

* Version 3.2.5 - 28 apr 2021
    * Fix trade deserialization without order id
    * Allow symbols starting with numeric character
    * Update CryptoExchange.Net
    * Fixed check in socket balance update

* Version 3.2.4 - 19 apr 2021
    * Fixed Withdraw parameters

* Version 3.2.3 - 19 apr 2021
    * Fixed SubscribeToOrderUpdates serialization
    * Updated CryptoExchange.Net

* Version 3.2.2 - 30 mrt 2021
    * Updated CryptoExchange.Net

* Version 3.2.1 - 01 mrt 2021
    * Added Nuget SymbolPackage

* Version 3.2.0 - 01 mrt 2021
    * Added config for deterministic build
    * Updated CryptoExchange.Net

* Version 3.1.2 - 22 jan 2021
    * Updated for ICommonKline

* Version 3.1.1 - 14 jan 2021
    * Updated CryptoExchange.Net

* Version 3.1.0 - 21 dec 2020
    * Update CryptoExchange.Net
    * Updated to latest IExchangeClient

* Version 3.0.14 - 11 dec 2020
    * Updated CryptoExchange.Net
    * Implemented IExchangeClient

* Version 3.0.13 - 19 nov 2020
    * Updated CryptoExchange.Net

* Version 3.0.12 - 22 okt 2020
    * Fixed parsing of orders

* Version 3.0.11 - 28 Aug 2020
    * Updated CrytpoExchange.Net

* Version 3.0.10 - 12 Aug 2020
    * Updated CryptoExchange.Net

* Version 3.0.9 - 21 Jun 2020
    * Updated CryptoExchange

* Version 3.0.8 - 16 Jun 2020
    * Updated CryptoExchange.Net

* Version 3.0.7 - 07 Jun 2020
    * Updated CryptoExchange
	
* Version 3.0.6 - 03 Mar 2020
    * Updated CryptoExchange

* Version 3.0.5 - 03 Mar 2020
    * Updated CryptoExchange

* Version 3.0.4 - 27 Jan 2020
    * Updated CryptoExchange.Net

* Version 3.0.3 - 12 Nov 2019
    * Added DepositHistory and GetMarketInfo endpoints

* Version 3.0.2 - 23 Oct 2019
	* Fixed validation length symbols again

* Version 3.0.1 - 23 Oct 2019
	* Fixed validation length symbols

* Version 3.0.0 - 23 Oct 2019
	* See CryptoExchange.Net 3.0 release notes
	* Added input validation
	* Added CancellationToken support to all requests
	* Now using IEnumerable<> for collections
	* Renamed Market -> Symbol
	* Renamed MarketDepth -> OrderBook
	* Renamed Transaction -> Trade
	
* Version 2.0.10 - 11 Sep 2019
    * Updated CryptoExchange.Net

* Version 2.0.9 - 07 Aug 2019
    * Updated CryptoExchange.Net

* Version 2.0.8 - 05 Aug 2019
    * Added xml for code docs

* Version 2.0.7 - 09 jul 2019
	* Updated CoinExSymbolOrderBook

* Version 2.0.6 - 14 may 2019
	* Added an order book implementation for easily keeping an updated order book
	* Added additional constructor to ApiCredentials to be able to read from file

* Version 2.0.5 - 01 may 2019
	* Updated to latest CryptoExchange.Net
		* Adds response header to REST call result
		* Added rate limiter per API key
		* Unified socket client workings

* Version 2.0.4 - 07 mar 2019
	* Updated to latest CryptoExchange.Net

* Version 2.0.3 - 01 feb 2019
	* Updated to latest CryptoExchange.Net

* Version 2.0.2 - 06 dec 2018
	* Fix for user-agent error on .Net framework

* Version 2.0.1 - 06 dec 2018
	* Fixed freezes if called from the UI thread

* Version 2.0.0 - 05 dec 2018
	* Updated to CryptoExchange.Net version 2
		* Libraries now use the same standard functionalities
		* Objects returned by socket subscriptions standardized across libraries

* Version 1.0.0 - 21 sep 2018
	* Updated CryptoExchange.Net

* Version 0.0.6 - 20 aug 2018
	* Fix for default api credentials getting disposed

* Version 0.0.5 - 20 aug 2018
	* Updated CryptoExchange.Net for bug fix

* Version 0.0.4 - 17 aug 2018
	* Added handling for incosistent data in socket update
	* Added additional logging
	* Small reconnection fixes

* Version 0.0.3 - 16 aug 2018
	* Added client interfaces
	* Fixed minor Resharper warnings

* Version 0.0.2 - 13 aug 2018
	* Upped CryptoExchange.Net to fix bug

* Version 0.0.1 - 13 aug 2018
	* Initial release