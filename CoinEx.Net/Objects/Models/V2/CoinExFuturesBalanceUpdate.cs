using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    [SerializationModel]
    internal record CoinExFuturesBalanceUpdate
    {
        [JsonPropertyName("balance_list")]
        public CoinExFuturesBalance[] Balances { get; set; } = Array.Empty<CoinExFuturesBalance>();
    }
}
