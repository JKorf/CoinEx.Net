using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    public class OrderTypeIntConverter : BaseConverter<OrderType>
    {
        public OrderTypeIntConverter() : this(true) { }
        public OrderTypeIntConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderType, string> Mapping => new Dictionary<OrderType, string>
        {
            { OrderType.Limit, "1" },
            { OrderType.Market, "2" }
        };
    }
}
