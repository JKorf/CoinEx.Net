namespace CoinEx.Net.Objects.Internal
{
    internal class CoinExApiResult<T>
    {
        public string? Message { get; set; }
        public int Code { get; set; }
        public T Data { get; set; } = default!;
    }
}
