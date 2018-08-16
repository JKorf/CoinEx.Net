using CoinEx.Net.Objects;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace CoinEx.Net.Converters
{
    public class WithdrawStatusConverter: BaseConverter<WithdrawStatus>
    {
        public WithdrawStatusConverter() : this(true) { }
        public WithdrawStatusConverter(bool quotes) : base(quotes) { }

        protected override Dictionary<WithdrawStatus, string> Mapping => new Dictionary<WithdrawStatus, string>
        {
            { WithdrawStatus.Audit, "audit" },
            { WithdrawStatus.Cancel, "cancel" },
            { WithdrawStatus.Confirming, "confirming" },
            { WithdrawStatus.Fail, "fail" },
            { WithdrawStatus.Finish, "finish" },
            { WithdrawStatus.NotPass, "not_pass" },
            { WithdrawStatus.Pass, "pass" },
            { WithdrawStatus.Processing, "processing" },
        };
    }
}
