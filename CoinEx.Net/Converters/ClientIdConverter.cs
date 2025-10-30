using CryptoExchange.Net;
using CryptoExchange.Net.Converters.SystemTextJson;

namespace CoinEx.Net.Converters
{
    internal class ClientIdConverter : ReplaceConverter
    {
        public ClientIdConverter() : base($"{LibraryHelpers.GetClientReference(() => null, "CoinEx")}{LibraryHelpers.ClientOrderIdSeparator}->") { }
    }
}
