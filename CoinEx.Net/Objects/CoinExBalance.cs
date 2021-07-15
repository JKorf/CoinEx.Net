using CoinEx.Net.Converters;
using CryptoExchange.Net.ExchangeInterfaces;
using Newtonsoft.Json;

namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Balance info
    /// </summary>
    public class CoinExBalance: ICommonBalance
    {
        /// <summary>
        /// The symbol
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
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

        string ICommonBalance.CommonAsset => Symbol;
        decimal ICommonBalance.CommonAvailable => Available;
        decimal ICommonBalance.CommonTotal => Available + Frozen;
    }
}
