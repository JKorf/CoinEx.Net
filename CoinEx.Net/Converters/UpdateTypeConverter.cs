using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Converters
{
    public class UpdateTypeConverter: BaseConverter<UpdateType>
    {
        public UpdateTypeConverter() : this(true) { }
        public UpdateTypeConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<UpdateType, string> Mapping => new Dictionary<UpdateType, string>
        {
            { UpdateType.New, "1" },
            { UpdateType.Update, "2" },
            { UpdateType.Done, "3" },
        };
    }
}
