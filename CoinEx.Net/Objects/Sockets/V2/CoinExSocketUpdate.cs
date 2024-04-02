using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Sockets.V2
{
    internal class CoinExSocketUpdate<T>
    {
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
