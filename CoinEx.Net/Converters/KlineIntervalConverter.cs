using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    public class KlineIntervalConverter : BaseConverter<KlineInterval>
    {
        public KlineIntervalConverter() : this(true) { }
        public KlineIntervalConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<KlineInterval, string> Mapping => new Dictionary<KlineInterval, string>
        {
            { KlineInterval.OneMinute, "1min" },
            { KlineInterval.ThreeMinute, "3min" },
            { KlineInterval.FiveMinute, "5min" },
            { KlineInterval.FiveteenMinute, "15min" },
            { KlineInterval.ThirtyMinute, "30min" },
            { KlineInterval.OneHour, "1hour" },
            { KlineInterval.TwoHour, "2hour" },
            { KlineInterval.FourHour, "4hour" },
            { KlineInterval.SixHour, "6hour" },
            { KlineInterval.TwelveHour, "12hour" },
            { KlineInterval.OneDay, "1day" },
            { KlineInterval.ThreeDay, "3day" },
            { KlineInterval.OneWeek, "1week" }
        };
    }
}
