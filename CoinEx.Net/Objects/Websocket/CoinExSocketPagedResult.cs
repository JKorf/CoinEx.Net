using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketPagedResult<T>
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
        [JsonProperty("records")]
        public T[] Data { get; set; }
    }
}
