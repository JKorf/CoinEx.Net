using Newtonsoft.Json;

namespace CoinEx.Net.Objects.Sockets
{
    internal class CoinExSubscriptionStatus
    {
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
    }
}
