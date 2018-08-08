using CryptoExchange.Net.Interfaces;
using System.Threading.Tasks;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExStream
    {
        internal bool TryReconnect { get; set; } = true;
        public IWebsocket Socket { get; set; }
        public CoinExStreamSubscription StreamResult { get; set; }

        public async Task Close()
        {
            TryReconnect = false;
            await Socket.Close().ConfigureAwait(false);
            Socket.Dispose();
        }
    }
}
