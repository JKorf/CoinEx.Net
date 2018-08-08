namespace CoinEx.Net.Objects
{
    public enum TransactionType
    {
        Either,
        Buy,
        Sell
    }

    public enum KlineInterval
    {
        OneMinute,
        ThreeMinute,
        FiveMinute,
        FiveteenMinute,
        ThirtyMinute,
        OneHour,
        TwoHour,
        FourHour,
        SixHour,
        TwelfHour,
        OneDay,
        ThreeDay,
        OneWeek
    }

    public enum WithdrawStatus
    {
        Audit,
        Pass,
        Processing,
        Confirming,
        NotPass,
        Cancel,
        Finish,
        Fail
    }

    public enum OrderType
    {
        Limit,
        Market
    }

    public enum OrderStatus
    {
        Executed,
        PartiallyExecuted,
        Unexecuted,
        Canceled
    }

    public enum TransactionRole
    {
        Maker,
        Taker
    }
}
