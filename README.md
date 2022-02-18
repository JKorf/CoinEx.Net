# CoinEx.Net
![Build status](https://travis-ci.com/JKorf/CoinEx.Net.svg?branch=master) ![Nuget version](https://img.shields.io/nuget/v/CoinEx.net.svg)  ![Nuget downloads](https://img.shields.io/nuget/dt/CoinEx.Net.svg)

CoinEx.Net is a wrapper around the CoinEx API as described on [CoinEx](https://github.com/coinexcom/coinex_exchange_api/wiki), including all features the API provides using clear and readable objects, both for the REST  as the websocket API's.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/JKorf/CoinEx.Net/issues)**

[Documentation](https://jkorf.github.io/CoinEx.Net/)

## Donations
I develop and maintain this package on my own for free in my spare time. Donations are greatly appreciated. If you prefer to donate any other currency please contact me.

**Btc**:  12KwZk3r2Y3JZ2uMULcjqqBvXmpDwjhhQS  
**Eth**:  0x069176ca1a4b1d6e0b7901a6bc0dbf3bb0bf5cc2  
**Nano**: xrb_1ocs3hbp561ef76eoctjwg85w5ugr8wgimkj8mfhoyqbx4s1pbc74zggw7gs  

## Discord
A Discord server is available [here](https://discord.gg/MSpeEtSY8t). Feel free to join for discussion and/or questions around the CryptoExchange.Net and implementation libraries.

## Release notes
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