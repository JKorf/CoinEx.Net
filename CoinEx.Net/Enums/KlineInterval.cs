using System.Text.Json.Serialization;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Attributes;

namespace CoinEx.Net.Enums
{
    /// <summary>
    /// Interval for klines, int value represent the time in seconds
    /// </summary>
    [JsonConverter(typeof(EnumConverter<KlineInterval>))]
    public enum KlineInterval
    {
        /// <summary>
        /// ["<c>1min</c>"] 1m
        /// </summary>
        [Map("1min")]
        OneMinute = 60,
        /// <summary>
        /// ["<c>3min</c>"] 3m
        /// </summary>
        [Map("3min")]
        ThreeMinutes = 60 * 3,
        /// <summary>
        /// ["<c>5min</c>"] 5m
        /// </summary>
        [Map("5min")]
        FiveMinutes = 60 * 5,
        /// <summary>
        /// ["<c>15min</c>"] 15m
        /// </summary>
        [Map("15min")]
        FifteenMinutes = 60 * 15,
        /// <summary>
        /// ["<c>30min</c>"] 30m
        /// </summary>
        [Map("30min")]
        ThirtyMinutes = 60 * 30,
        /// <summary>
        /// ["<c>1hour</c>"] 1h
        /// </summary>
        [Map("1hour")]
        OneHour = 60 * 60,
        /// <summary>
        /// ["<c>2hour</c>"] 2h
        /// </summary>
        [Map("2hour")]
        TwoHours = 60 * 60 * 2,
        /// <summary>
        /// ["<c>4hour</c>"] 4h
        /// </summary>
        [Map("4hour")]
        FourHours = 60 * 60 * 4,
        /// <summary>
        /// ["<c>6hour</c>"] 6h
        /// </summary>
        [Map("6hour")]
        SixHours = 60 * 60 * 6,
        /// <summary>
        /// ["<c>12hour</c>"] 12h
        /// </summary>
        [Map("12hour")]
        TwelveHours = 60 * 60 * 12,
        /// <summary>
        /// ["<c>1day</c>"] 1d
        /// </summary>
        [Map("1day")]
        OneDay = 60 * 60 * 24,
        /// <summary>
        /// ["<c>3day</c>"] 3d
        /// </summary>
        [Map("3day")]
        ThreeDays = 60 * 60 * 24 * 3,
        /// <summary>
        /// ["<c>1week</c>"] 1w
        /// </summary>
        [Map("1week")]
        OneWeek = 60 * 60 * 24 * 7
    }
}
