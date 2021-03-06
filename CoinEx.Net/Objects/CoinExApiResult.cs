﻿using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Objects
{
    internal class CoinExApiResult<T>
    {
        public string? Message { get; set; }
        public int Code { get; set; }
        [JsonOptionalProperty] public T Data { get; set; } = default!;
    }
}
