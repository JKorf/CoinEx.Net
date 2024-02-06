using System;
using System.Text.RegularExpressions;

namespace CoinEx.Net.ExtensionMethods
{
    /// <summary>
    /// Extension methods specific to using the CoinEx API
    /// </summary>
    public static class CoinExExtensionMethods
    {
        /// <summary>
        /// Validate the string is a valid CoinEx symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateCoinExSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if (!Regex.IsMatch(symbolString, "^([0-9A-Z]{5,})$"))
                throw new ArgumentException($"{symbolString} is not a valid CoinEx symbol. Should be [BaseAsset][QuoteAsset], e.g. ETHBTC");
        }
    }
}
