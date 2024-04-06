using CoinEx.Net.Enums;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    internal class TransactionRoleConverter: BaseConverter<TransactionRole>
    {
        public TransactionRoleConverter() : this(true) { }
        public TransactionRoleConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<TransactionRole, string>> Mapping => new List<KeyValuePair<TransactionRole, string>>
        {
            new KeyValuePair<TransactionRole, string>(TransactionRole.Maker, "maker"),
            new KeyValuePair<TransactionRole, string>(TransactionRole.Taker, "taker")
        };
    }
}
