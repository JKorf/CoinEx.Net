# ![Icon](https://github.com/JKorf/CoinEx.Net/blob/master/Resources/icon.png?raw=true) CoinEx.Net 

![Build status](https://travis-ci.org/JKorf/CoinEx.Net.svg?branch=master)

A .Net wrapper for the CoinEX API as described on [CoinEx](https://github.com/coinexcom/coinex_exchange_api/wiki), including all features the API provides using clear and readable objects.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/JKorf/CoinEx.Net/issues)**

## CryptoExchange.Net
Implementation is build upon the CryptoExchange.Net library, make sure to also check out the documentation on that: [docs](https://github.com/JKorf/CryptoExchange.Net)

Other CryptoExchange.Net implementations:
<table>
<tr>
<td><a href="https://github.com/JKorf/Bittrex.Net"><img src="https://github.com/JKorf/Bittrex.Net/blob/master/Resources/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Bittrex.Net">Bittrex</a>
</td>
<td><a href="https://github.com/JKorf/Bitfinex.Net"><img src="https://github.com/JKorf/Bitfinex.Net/blob/master/Resources/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Bitfinex.Net">Bitfinex</a>
</td>
<td><a href="https://github.com/JKorf/Binance.Net"><img src="https://github.com/JKorf/Binance.Net/blob/master/Resources/binance-coin.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Binance.Net">Binance</a>
</td>
<td><a href="https://github.com/JKorf/Huobi.Net"><img src="https://github.com/JKorf/Huobi.Net/blob/master/Resources/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Huobi.Net">Huobi</a>
</td>
<td><a href="https://github.com/JKorf/Kucoin.Net"><img src="https://github.com/JKorf/Kucoin.Net/blob/master/Resources/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Kucoin.Net">Kucoin</a>
</td>
<td><a href="https://github.com/JKorf/Kraken.Net"><img src="https://github.com/JKorf/Kraken.Net/blob/master/Resources/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Kraken.Net">Kraken</a>
</td>
</tr>
</table>

Implementations from third parties
<table>
<tr>
<td><a href="https://github.com/Zaliro/Switcheo.Net"><img src="https://github.com/Zaliro/Switcheo.Net/blob/master/Resources/switcheo-coin.png?raw=true"></a>
<br />
<a href="https://github.com/Zaliro/Switcheo.Net">Switcheo</a>
</td>
	<td><a href="https://github.com/ridicoulous/LiquidQuoine.Net"><img src="https://github.com/ridicoulous/LiquidQuoine.Net/blob/master/Resources/icon.png?raw=true"></a>
<br />
<a href="https://github.com/ridicoulous/LiquidQuoine.Net">Liquid</a>
</td>
</tr>
</table>


## Donations
Donations are greatly appreciated and a motivation to keep improving.

**Btc**:  12KwZk3r2Y3JZ2uMULcjqqBvXmpDwjhhQS  
**Eth**:  0x069176ca1a4b1d6e0b7901a6bc0dbf3bb0bf5cc2  
**Nano**: xrb_1ocs3hbp561ef76eoctjwg85w5ugr8wgimkj8mfhoyqbx4s1pbc74zggw7gs  


## Installation
![Nuget version](https://img.shields.io/nuget/v/coinex.net.svg)  ![Nuget downloads](https://img.shields.io/nuget/dt/CoinEx.Net.svg)
Available on [Nuget](https://www.nuget.org/packages/CoinEx.Net/).
```
pm> Install-Package CoinEx.Net
```
To get started with CoinEx.Net first you will need to get the library itself. The easiest way to do this is to install the package into your project using  [NuGet](https://www.nuget.org/packages/CoinEx.Net/). Using Visual Studio this can be done in two ways.

### Using the package manager
In Visual Studio right click on your solution and select 'Manage NuGet Packages for solution...'. A screen will appear which initially shows the currently installed packages. In the top bit select 'Browse'. This will let you download net package from the NuGet server. In the search box type 'CoinEx.Net' and hit enter. The CoinEx.Net package should come up in the results. After selecting the package you can then on the right hand side select in which projects in your solution the package should install. After you've selected all project you wish to install and use CoinEx.Net in hit 'Install' and the package will be downloaded and added to you projects.

### Using the package manager console
In Visual Studio in the top menu select 'Tools' -> 'NuGet Package Manager' -> 'Package Manager Console'. This should open up a command line interface. On top of the interface there is a dropdown menu where you can select the Default Project. This is the project that CoinEx.Net will be installed in. After selecting the correct project type  `Install-Package CoinEx.Net`  in the command line interface. This should install the latest version of the package in your project.

After doing either of above steps you should now be ready to actually start using CoinEx.Net.
## Getting started
After installing it's time to actually use it. To get started you have to add the CoinEx.Net namespace: `using CoinEx.Net;`.

CoinEx.Net provides two clients to interact with the CoinEx API. The  `CoinExClient`  provides all rest API calls. The `CoinExSocketClient` provides functions to interact with the websocket provided by the CoinEx API. Both clients are disposable and as such can be used in a `using` statement.

## Examples
Examples can be found in the Examples folder.

## Release notes
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
