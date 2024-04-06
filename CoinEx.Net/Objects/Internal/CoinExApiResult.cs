using System.Text.Json.Serialization;

namespace CoinEx.Net.Objects.Internal
{
    internal class CoinExApiResult
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("code")]
        public int Code { get; set; }
    }

    internal class CoinExApiResult<T> : CoinExApiResult
    {
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
    }

    internal class CoinExPageApiResult<T> : CoinExApiResult<T>
    {
        [JsonPropertyName("pagination")]
        public CoinExPage Pagination { get; set; } = null!;
        [JsonPropertyName("paginatation")]
        public CoinExPage PaginationTypo { set => Pagination = value; }
    }

    internal class CoinExPage
    {
        [JsonPropertyName("total")]
        public int? Total { get; set; }
        [JsonPropertyName("has_next")]
        public bool HasNext { get; set; }
    }
}
