using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    public class KlineIntervalConverter : BaseConverter<KlineInterval>
    {
        public KlineIntervalConverter() : this(true) { }
        public KlineIntervalConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<KlineInterval, string>> Mapping => new List<KeyValuePair<KlineInterval, string>>
        {
            new KeyValuePair<KlineInterval, string>( KlineInterval.OneMinute, "1min"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThreeMinute, "3min"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FiveMinute, "5min"),
            new KeyValuePair<KlineInterval, string>( KlineInterval.FiveteenMinute, "15min"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThirtyMinute, "30min"),
            new KeyValuePair<KlineInterval, string>( KlineInterval.OneHour, "1hour"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.TwoHour, "2hour"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FourHour, "4hour"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.SixHour, "6hour"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.TwelveHour, "12hour"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneDay, "1day"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThreeDay, "3day"),
            new KeyValuePair<KlineInterval, string>( KlineInterval.OneWeek, "1week")
        };
    }
}
