using CryptoExchange.Net.Converters.SystemTextJson;

namespace CoinEx.Net.Converters
{
    internal class ClientIdConverter : ReplaceConverter
    {
        public ClientIdConverter() : base($"{CoinExExchange.ClientOrderIdPrefix}->") { }
    }
}
