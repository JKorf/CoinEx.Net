using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    public class OrderTypeConverter: BaseConverter<OrderType>
    {
        public OrderTypeConverter() : this(true) { }
        public OrderTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderType, string> Mapping => new Dictionary<OrderType, string>
        {
            { OrderType.Limit, "limit" },
            { OrderType.Market, "market" }
        };
    }
}
