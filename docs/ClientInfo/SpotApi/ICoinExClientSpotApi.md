---
title: ICoinExClientSpotApi
has_children: true
parent: Rest API documentation
---
*[generated documentation]*  
`CoinExRestClient > SpotApi`  
*Spot API*
  
***
*Get the ISpotClient for this client. This is a common interface which allows for some basic operations without knowing any details of the exchange.*  
**ISpotClient CommonSpotClient { get; }**  
***
*Endpoints related to account settings, info or actions*  
**ICoinExClientSpotApiAccount Account { get; }**  
***
*Endpoints related to retrieving market and system data*  
**ICoinExClientSpotApiExchangeData ExchangeData { get; }**  
***
*Endpoints related to orders and trades*  
**ICoinExClientSpotApiTrading Trading { get; }**  
