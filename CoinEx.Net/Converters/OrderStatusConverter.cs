using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    public class OrderStatusConverter: BaseConverter<OrderStatus>
    {
        public OrderStatusConverter() : this(true) { }
        public OrderStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<OrderStatus, string> Mapping => new Dictionary<OrderStatus, string>
        {
            { OrderStatus.Executed, "done" },
            { OrderStatus.PartiallyExecuted, "part_deal" },
            { OrderStatus.UnExecuted, "not_deal" },
            { OrderStatus.Canceled, "cancel" }
        };
    }
}
