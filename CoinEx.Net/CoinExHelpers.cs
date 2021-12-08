using CoinEx.Net.Enums;
using System;
using System.Text.RegularExpressions;

namespace CoinEx.Net
{
    /// <summary>
    /// CoinEx helpers
    /// </summary>
    public static class CoinExHelpers
    {
        /// <summary>
        /// Kline interval to seconds
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static int ToSeconds(this KlineInterval interval)
        {
            return interval switch
            {
                KlineInterval.OneMinute => 1 * 60,
                KlineInterval.ThreeMinutes => 3 * 60,
                KlineInterval.FiveMinutes => 5 * 60,
                KlineInterval.FifteenMinutes => 15 * 60,
                KlineInterval.ThirtyMinutes => 30 * 60,
                KlineInterval.OneHour => 1 * 60 * 60,
                KlineInterval.TwoHours => 2 * 60 * 60,
                KlineInterval.FourHours => 4 * 60 * 60,
                KlineInterval.SixHours => 6 * 60 * 60,
                KlineInterval.TwelveHours => 12 * 60 * 60,
                KlineInterval.OneDay => 1 * 24 * 60 * 60,
                KlineInterval.ThreeDays => 3 * 24 * 60 * 60,
                KlineInterval.OneWeek => 7 * 24 * 60 * 60,
                _ => 0,
            };
        }

        /// <summary>
        /// Merge depth to string parameter
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static string MergeDepthIntToString(int depth)
        {
            var merge = "0";
            if (depth == 8)
                return merge;

            merge += ".";
            for (var i = 0; i < 7 - depth; i++)
                merge += "0";
            merge += "1";
            return merge;
        }

        /// <summary>
        /// Validate the string is a valid CoinEx symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateCoinExSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if (!Regex.IsMatch(symbolString, "^([0-9A-Z]{5,})$"))
                throw new ArgumentException($"{symbolString} is not a valid CoinEx symbol. Should be [QuoteAsset][BaseAsset], e.g. ETHBTC");
        }
    }
}
