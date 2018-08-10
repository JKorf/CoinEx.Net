using CoinEx.Net.Objects;

namespace CoinEx.Net
{
    internal static class CoinExHelpers
    {
        public static int ToSeconds(this KlineInterval interval)
        {
            if (interval == KlineInterval.OneMinute) return 1 * 60;
            if (interval == KlineInterval.ThreeMinute) return 3 * 60;
            if (interval == KlineInterval.FiveMinute) return 5 * 60;
            if (interval == KlineInterval.FiveteenMinute) return 15 * 60;
            if (interval == KlineInterval.ThirtyMinute) return 30 * 60;
            if (interval == KlineInterval.OneHour) return 1 * 60 * 60;
            if (interval == KlineInterval.TwoHour) return 2 * 60 * 60;
            if (interval == KlineInterval.FourHour) return 4 * 60 * 60;
            if (interval == KlineInterval.SixHour) return 6 * 60 * 60;
            if (interval == KlineInterval.TwelfHour) return 12 * 60 * 60;
            if (interval == KlineInterval.OneDay) return 1 * 24 * 60 * 60;
            if (interval == KlineInterval.ThreeDay) return 3 * 24 * 60 * 60;
            if (interval == KlineInterval.OneWeek) return 7 * 24 * 60 * 60;
            else return 0;
        }

        public static string MergeDepthIntToString(int depth)
        {
            string merge = "0";
            if (depth != 8)
            {
                merge += ".";
                for (int i = 0; i < 7 - depth; i++)
                    merge += "0";
                merge += "1";
            }
            return merge;
        }
    }
}
