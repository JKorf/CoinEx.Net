namespace CoinEx.Net.Objects
{
    /// <summary>
    /// Transaction type
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Either (only usable for filtering)
        /// </summary>
        Either,
        /// <summary>
        /// Buy
        /// </summary>
        Buy,
        /// <summary>
        /// Sell
        /// </summary>
        Sell
    }

    /// <summary>
    /// Interval for klines
    /// </summary>
    public enum KlineInterval
    {
        /// <summary>
        /// 1m
        /// </summary>
        OneMinute,
        /// <summary>
        /// 3m
        /// </summary>
        ThreeMinute,
        /// <summary>
        /// 5m
        /// </summary>
        FiveMinute,
        /// <summary>
        /// 15m
        /// </summary>
        FifteenMinute,
        /// <summary>
        /// 30m
        /// </summary>
        ThirtyMinute,
        /// <summary>
        /// 1h
        /// </summary>
        OneHour,
        /// <summary>
        /// 2h
        /// </summary>
        TwoHour,
        /// <summary>
        /// 4h
        /// </summary>
        FourHour,
        /// <summary>
        /// 6h
        /// </summary>
        SixHour,
        /// <summary>
        /// 12h
        /// </summary>
        TwelveHour,
        /// <summary>
        /// 1d
        /// </summary>
        OneDay,
        /// <summary>
        /// 3d
        /// </summary>
        ThreeDay,
        /// <summary>
        /// 1w
        /// </summary>
        OneWeek
    }

    /// <summary>
    /// Status of a withdrawal
    /// </summary>
    public enum WithdrawStatus
    {
        /// <summary>
        /// Under audit
        /// </summary>
        Audit,
        /// <summary>
        /// Passed audit
        /// </summary>
        Pass,
        /// <summary>
        /// Processing
        /// </summary>
        Processing,
        /// <summary>
        /// Confirming
        /// </summary>
        Confirming,
        /// <summary>
        /// Not passed audit
        /// </summary>
        NotPass,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancel,
        /// <summary>
        /// Finished
        /// </summary>
        Finish,
        /// <summary>
        /// Failed
        /// </summary>
        Fail
    }

    /// <summary>
    /// Type of order
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Limit order
        /// </summary>
        Limit,
        /// <summary>
        /// Market order
        /// </summary>
        Market
    }

    /// <summary>
    /// Status of an order
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Executed
        /// </summary>
        Executed,
        /// <summary>
        /// Partially executed
        /// </summary>
        PartiallyExecuted,
        /// <summary>
        /// New
        /// </summary>
        UnExecuted,
        /// <summary>
        /// Cancelled
        /// </summary>
        Canceled
    }

    /// <summary>
    /// Role of a transaction
    /// </summary>
    public enum TransactionRole
    {
        /// <summary>
        /// Maker of a new order book entry
        /// </summary>
        Maker,
        /// <summary>
        /// Taker of an existing order book entry
        /// </summary>
        Taker
    }

    /// <summary>
    /// Type of update
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// New
        /// </summary>
        New,
        /// <summary>
        /// Update
        /// </summary>
        Update,
        /// <summary>
        /// Done
        /// </summary>
        Done
    }

    /// <summary>
    /// Options when placing an order
    /// </summary>
    public enum OrderOption
    {
        /// <summary>
        /// Normal order
        /// </summary>
        Normal,
        /// <summary>
        /// Immediate or cancel order
        /// </summary>
        ImmediateOrCancel,
        /// <summary>
        /// Fill or kill order
        /// </summary>
        FillOrKill,
        /// <summary>
        /// Maker only order
        /// </summary>
        MakerOnly
    }
}
