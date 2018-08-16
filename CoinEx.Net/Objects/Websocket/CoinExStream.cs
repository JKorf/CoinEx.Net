using CryptoExchange.Net.Interfaces;
using System.Threading.Tasks;

namespace CoinEx.Net.Objects.Websocket
{
    internal class CoinExStream
    {
        internal bool TryReconnect { get; set; };
        public IWebsocket Socket { get; set; }
        public CoinExStreamSubscription StreamResult { get; set; }
        internal CoinExSocketRequest Request { get; set; }
        internal CoinExSubscription Subscription { get; set; }
        internal bool Authenticated { get; set; }

        public async Task Close(bool reconnect = false)
        {
            TryReconnect = reconnect;
            await Socket.Close().ConfigureAwait(false);
        }
    }
}
