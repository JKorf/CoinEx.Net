using CoinEx.Net.Enums;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    internal class OrderOptionConverter : BaseConverter<OrderOption>
    {
        public OrderOptionConverter() : this(true) { }
        public OrderOptionConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<OrderOption, string>> Mapping => new List<KeyValuePair<OrderOption, string>>
        {
            new KeyValuePair<OrderOption, string>(OrderOption.Normal, "NORMAL"),
            new KeyValuePair<OrderOption, string>(OrderOption.ImmediateOrCancel, "IOC"),
            new KeyValuePair<OrderOption, string>(OrderOption.FillOrKill, "FOK"),
            new KeyValuePair<OrderOption, string>(OrderOption.MakerOnly, "MAKER_ONLY")
        };
    }
}
