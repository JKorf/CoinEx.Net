using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Converters
{
    public class TransactionTypeIntConverter: BaseConverter<TransactionType>
    {
        public TransactionTypeIntConverter() : this(true) { }
        public TransactionTypeIntConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<TransactionType, string> Mapping => new Dictionary<TransactionType, string>
        {
            { TransactionType.Either, "0" },
            { TransactionType.Sell, "1" },
            { TransactionType.Buy, "2" },
        };
    }
}
