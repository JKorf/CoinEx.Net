using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    public class TransactionTypeIntConverter: BaseConverter<TransactionType>
    {
        public TransactionTypeIntConverter() : this(true) { }
        public TransactionTypeIntConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<TransactionType, string>> Mapping => new List<KeyValuePair<TransactionType, string>>
        {
            new KeyValuePair<TransactionType, string>(TransactionType.Either, "0"),
            new KeyValuePair<TransactionType, string>(TransactionType.Sell, "1"),
            new KeyValuePair<TransactionType, string>(TransactionType.Buy, "2")
        };
    }
}
