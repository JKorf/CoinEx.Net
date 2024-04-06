using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Interval for klines, int value represent the time in seconds
    /// </summary>
    public enum KlineInterval
    {
        /// <summary>
        /// 1m
        /// </summary>
        [Map("1min")]
        OneMinute = 60,
        /// <summary>
        /// 3m
        /// </summary>
        [Map("3min")]
        ThreeMinutes = 60 * 3,
        /// <summary>
        /// 5m
        /// </summary>
        [Map("5min")]
        FiveMinutes = 60 * 5,
        /// <summary>
        /// 15m
        /// </summary>
        [Map("15min")]
        FifteenMinutes = 60 * 15,
        /// <summary>
        /// 30m
        /// </summary>
        [Map("30min")]
        ThirtyMinutes = 60 * 30,
        /// <summary>
        /// 1h
        /// </summary>
        [Map("1hour")]
        OneHour = 60 * 60,
        /// <summary>
        /// 2h
        /// </summary>
        [Map("2hour")]
        TwoHours = 60 * 60 * 2,
        /// <summary>
        /// 4h
        /// </summary>
        [Map("4hour")]
        FourHours = 60 * 60 * 4,
        /// <summary>
        /// 6h
        /// </summary>
        [Map("6hour")]
        SixHours = 60 * 60 * 6,
        /// <summary>
        /// 12h
        /// </summary>
        [Map("12hour")]
        TwelveHours = 60 * 60 * 12,
        /// <summary>
        /// 1d
        /// </summary>
        [Map("1day")]
        OneDay = 60 * 60 * 24,
        /// <summary>
        /// 3d
        /// </summary>
        [Map("3day")]
        ThreeDays = 60 * 60 * 24 * 3,
        /// <summary>
        /// 1w
        /// </summary>
        [Map("1week")]
        OneWeek = 60 * 60 * 24 * 7
    }
}
