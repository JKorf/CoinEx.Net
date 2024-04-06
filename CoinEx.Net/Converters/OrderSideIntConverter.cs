using CoinEx.Net.Enums;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    internal class OrderSideIntConverter : BaseConverter<OrderSide>
    {
        public OrderSideIntConverter() : this(true) { }
        public OrderSideIntConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<OrderSide, string>> Mapping => new List<KeyValuePair<OrderSide, string>>
        {
            new KeyValuePair<OrderSide, string>(OrderSide.Either, "0"),
            new KeyValuePair<OrderSide, string>(OrderSide.Sell, "1"),
            new KeyValuePair<OrderSide, string>(OrderSide.Buy, "2")
        };
    }
}
