using CoinEx.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects
{
    public class CoinExBalance
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Available { get; set; }
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Frozen { get; set; }
    }
}
