using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Models.V2
{
    internal record CoinExFuturesBalanceUpdate
    {
        [JsonPropertyName("balance_list")]
        public IEnumerable<CoinExFuturesBalance> Balances { get; set; } = Array.Empty<CoinExFuturesBalance>();
    }
}
