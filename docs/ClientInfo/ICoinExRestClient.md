---
title: Rest API documentation
has_children: true
---
*[generated documentation]*  
### CoinExRestClient  
*Client for accessing the CoinEx API.*
  
***
*Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.*  
**void SetApiCredentials(ApiCredentials credentials);**  
***
*Spot endpoints*  
**ICoinExClientSpotApi SpotApi { get; }**  
