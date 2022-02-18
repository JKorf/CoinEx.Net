---
title: Getting started
nav_order: 2
---

## Creating client
There are 2 clients available to interact with the CoinEx API, the `CoinExClient` and `CoinExSocketClient`.

*Create a new rest client*
```csharp
var coinExClient = new CoinExClient(new CoinExClientOptions()
{
	// Set options here for this client
});
```

*Create a new socket client*
```csharp
var coinExSocketClient = new CoinExSocketClient(new CoinExSocketClientOptions()
{
	// Set options here for this client
});
```

Different options are available to set on the clients, see this example
```csharp
var coinExClient = new CoinExClient(new CoinExClientOptions()
{
	ApiCredentials = new ApiCredentials("API-KEY", "API-SECRET"),
	LogLevel = LogLevel.Trace,
	RequestTimeout = TimeSpan.FromSeconds(60)
});
```
Alternatively, options can be provided before creating clients by using `SetDefaultOptions`:
```csharp
CoinExClient.SetDefaultOptions(new CoinExClientOptions{
	// Set options here for all new clients
});
var coinExClient = new CoinExClient();
```
More info on the specific options can be found in the [CryptoExchange.Net documentation](https://jkorf.github.io/CryptoExchange.Net/Options.html)

### Dependency injection
See [CryptoExchange.Net documentation](https://jkorf.github.io/CryptoExchange.Net/Clients.html#dependency-injection)