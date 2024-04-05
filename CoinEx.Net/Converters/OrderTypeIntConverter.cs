using CoinEx.Net.Enums;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    internal class OrderTypeIntConverter : BaseConverter<OrderType>
    {
        public OrderTypeIntConverter() : this(true) { }
        public OrderTypeIntConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<OrderType, string>> Mapping => new List<KeyValuePair<OrderType, string>>
        {
            new KeyValuePair<OrderType, string>(OrderType.Limit, "1"),
            new KeyValuePair<OrderType, string>(OrderType.Market, "2")
        };
    }
}
