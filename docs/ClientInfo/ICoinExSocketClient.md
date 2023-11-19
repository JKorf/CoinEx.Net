---
title: Socket API documentation
has_children: true
---
*[generated documentation]*  
### CoinExSocketClient  
*Client for accessing the CoinEx websocket API*
  
***
*Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.*  
**void SetApiCredentials(ApiCredentials credentials);**  
***
*Spot streams*  
**[ICoinExSocketClientSpotApi](SpotApi/ICoinExSocketClientSpotApi.html) SpotApi { get; }**  
