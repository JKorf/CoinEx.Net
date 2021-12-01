using System;
using System.Threading.Tasks;
using CoinEx.Net.Clients.Socket;
using CoinEx.Net.Interfaces.Clients.Socket;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Models.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;

namespace CoinEx.Net.SymbolOrderBooks
{
    /// <summary>
    /// Symbol order book implementation
    /// </summary>
    public class CoinExSpotSymbolOrderBook: SymbolOrderBook
    {
        private readonly ICoinExSocketClient socketClient;
        private readonly bool _socketOwner;

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol of the order book</param>
        /// <param name="options">The options for the order book</param>
        public CoinExSpotSymbolOrderBook(string symbol, CoinExOrderBookOptions? options = null) : base("CoinEx[Spot]", symbol, options ?? new CoinExOrderBookOptions())
        {
            symbol.ValidateCoinExSymbol();

            strictLevels = false;
            sequencesAreConsecutive = false;

            socketClient = options?.SocketClient ?? new CoinExSocketClient();
            _socketOwner = options?.SocketClient == null;
            Levels = 20;
        }

        /// <inheritdoc />
        protected override async Task<CallResult<UpdateSubscription>> DoStartAsync()
        {
            var result = await socketClient.SpotStreams.SubscribeToOrderBookUpdatesAsync(Symbol, Levels!.Value, 0, HandleUpdate).ConfigureAwait(false);
            if (!result)
                return result;

            Status = OrderBookStatus.Syncing;

            var setResult = await WaitForSetOrderBookAsync(10000).ConfigureAwait(false);
            return setResult ? result : new CallResult<UpdateSubscription>(null, setResult.Error);
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> DoResyncAsync()
        {
            return await WaitForSetOrderBookAsync(10000).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override void DoReset()
        {
        }

        private void HandleUpdate(DataEvent<CoinExSocketOrderBook> data)
        {
            if (data.Data.FullUpdate)
            { 
                SetInitialOrderBook(DateTime.UtcNow.Ticks, data.Data.Bids, data.Data.Asks);
            }
            else
            {
                UpdateOrderBook(DateTime.UtcNow.Ticks, data.Data.Bids, data.Data.Asks);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            processBuffer.Clear();
            asks.Clear();
            bids.Clear();

            if(_socketOwner)
                socketClient?.Dispose();
        }
    }
}
