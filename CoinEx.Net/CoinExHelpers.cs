using CoinEx.Net.Objects;

namespace CoinEx.Net
{
    internal static class CoinExHelpers
    {
        public static int ToSeconds(this KlineInterval interval)
        {
            switch (interval)
            {
                case KlineInterval.OneMinute:
                    return 1 * 60;
                case KlineInterval.ThreeMinute:
                    return 3 * 60;
                case KlineInterval.FiveMinute:
                    return 5 * 60;
                case KlineInterval.FiveteenMinute:
                    return 15 * 60;
                case KlineInterval.ThirtyMinute:
                    return 30 * 60;
                case KlineInterval.OneHour:
                    return 1 * 60 * 60;
                case KlineInterval.TwoHour:
                    return 2 * 60 * 60;
                case KlineInterval.FourHour:
                    return 4 * 60 * 60;
                case KlineInterval.SixHour:
                    return 6 * 60 * 60;
                case KlineInterval.TwelveHour:
                    return 12 * 60 * 60;
                case KlineInterval.OneDay:
                    return 1 * 24 * 60 * 60;
                case KlineInterval.ThreeDay:
                    return 3 * 24 * 60 * 60;
                case KlineInterval.OneWeek:
                    return 7 * 24 * 60 * 60;
                default:
                    return 0;
            }
        }

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
    }
}
