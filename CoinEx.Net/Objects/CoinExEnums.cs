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
        TwelveHour,
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
        UnExecuted,
        Canceled
    }

    public enum TransactionRole
    {
        Maker,
        Taker
    }

    public enum UpdateType
    {
        New,
        Update,
        Done
    }
}
