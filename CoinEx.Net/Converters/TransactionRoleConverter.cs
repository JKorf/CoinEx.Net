using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    public class TransactionRoleConverter: BaseConverter<TransactionRole>
    {
        public TransactionRoleConverter() : this(true) { }
        public TransactionRoleConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<TransactionRole, string> Mapping => new Dictionary<TransactionRole, string>
        {
            { TransactionRole.Maker, "maker" },
            { TransactionRole.Taker, "taker" }
        };
    }
}
