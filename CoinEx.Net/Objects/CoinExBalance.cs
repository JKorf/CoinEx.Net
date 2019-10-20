using CoinEx.Net.Converters;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Balance info
    /// </summary>
    public class CoinExBalance
    {
        /// <summary>
        /// The amount of the asset that is available
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Available { get; set; }
        /// <summary>
        /// The amount of the asset not currently available
        /// </summary>
        [JsonConverter(typeof(DecimalConverter))]
        public decimal Frozen { get; set; }
    }
}
