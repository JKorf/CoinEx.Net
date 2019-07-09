using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;

namespace CoinEx.Net
{
    public class CoinExSymbolOrderBook: SymbolOrderBook
    {
        private readonly CoinExSocketClient socketClient;
        private bool updateReceived;

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol of the order book</param>
        /// <param name="options">The options for the order book</param>
        public CoinExSymbolOrderBook(string symbol, CoinExOrderBookOptions options = null) : base(symbol, options ?? new CoinExOrderBookOptions())
        {
            socketClient = new CoinExSocketClient();
        }

        protected override async Task<CallResult<UpdateSubscription>> DoStart()
        {
            var result = await socketClient.SubscribeToMarketDepthUpdatesAsync(Symbol, 20, 0, HandleUpdate).ConfigureAwait(false);
            if (!result.Success)
                return result;

            Status = OrderBookStatus.Syncing;

            while (!updateReceived)
                await Task.Delay(10).ConfigureAwait(false); // Wait for first update to fill the order book

            return result;
        }

        protected override async Task<CallResult<bool>> DoResync()
        {
            while (!updateReceived)
                await Task.Delay(10).ConfigureAwait(false); // Wait for first update to fill the order book

            return new CallResult<bool>(true, null);
        }

        protected override void DoReset()
        {
            updateReceived = false;
        }

        private void HandleUpdate(string market, bool full, CoinExSocketMarketDepth data)
        {
            updateReceived = true;
            if (full)
            { 
                SetInitialOrderBook(DateTime.UtcNow.Ticks, data.Asks, data.Bids);
            }
            else
            {
                var processEntries = new List<ProcessEntry>();
                processEntries.AddRange(data.Asks.Select(a => new ProcessEntry(OrderBookEntryType.Ask, a)));
                processEntries.AddRange(data.Bids.Select(b => new ProcessEntry(OrderBookEntryType.Bid, b)));
                UpdateOrderBook(DateTime.UtcNow.Ticks, DateTime.UtcNow.Ticks, processEntries);
            }
        }

        public override void Dispose()
        {
            processBuffer.Clear();
            asks.Clear();
            bids.Clear();

            socketClient?.Dispose();
        }
    }
}
