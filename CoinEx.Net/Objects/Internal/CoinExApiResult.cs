namespace CoinEx.Net.Objects.Internal
{
    internal class CoinExApiResult
    {
        public string? Message { get; set; }
        public int Code { get; set; }
    }

    internal class CoinExApiResult<T> : CoinExApiResult
    {
        public T Data { get; set; } = default!;
    }
}
