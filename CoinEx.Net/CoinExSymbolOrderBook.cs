using System;
using System.Threading.Tasks;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;

namespace CoinEx.Net
{
    /// <summary>
    /// Symbol order book implementation
    /// </summary>
    public class CoinExSymbolOrderBook: SymbolOrderBook
    {
        private readonly CoinExSocketClient socketClient;

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol of the order book</param>
        /// <param name="options">The options for the order book</param>
        public CoinExSymbolOrderBook(string symbol, CoinExOrderBookOptions? options = null) : base(symbol, options ?? new CoinExOrderBookOptions())
        {
            symbol.ValidateCoinExSymbol();
            socketClient = new CoinExSocketClient();
            Levels = 20;
        }

        /// <inheritdoc />
        protected override async Task<CallResult<UpdateSubscription>> DoStart()
        {
            var result = await socketClient.SubscribeToOrderBookUpdatesAsync(Symbol, Levels!.Value, 0, HandleUpdate).ConfigureAwait(false);
            if (!result)
                return result;

            Status = OrderBookStatus.Syncing;

            var setResult = await WaitForSetOrderBook(10000).ConfigureAwait(false);
            return setResult ? result : new CallResult<UpdateSubscription>(null, setResult.Error);
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> DoResync()
        {
            return await WaitForSetOrderBook(10000).ConfigureAwait(false);
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

            socketClient?.Dispose();
        }
    }
}
