using System;
using System.Text;
using System.Threading.Tasks;
using CoinEx.Net.Clients;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Models.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;

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
        public CoinExSpotSymbolOrderBook(string symbol, CoinExOrderBookOptions? options = null) : base("CoinEx", symbol, options ?? new CoinExOrderBookOptions())
        {
            symbol.ValidateCoinExSymbol();

            strictLevels = false;
            sequencesAreConsecutive = false;

            socketClient = options?.SocketClient ?? new CoinExSocketClient();
            _socketOwner = options?.SocketClient == null;
            Levels = options?.Limit ?? 20;
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
            if (data.Data.Checksum != null)
            {
                AddChecksum((int)data.Data.Checksum.Value);
            }
        }

        /// <inheritdoc />
        protected override bool DoChecksum(int checksum)
        {
            var checkStringBuilder = new StringBuilder();
            foreach (var bid in bids)
            {
                checkStringBuilder.Append(bid.Value.Price.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + bid.Value.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":");
            }
            foreach (var ask in asks)
            {
                checkStringBuilder.Append(ask.Value.Price.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + ask.Value.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":");
            }
            var checkString = checkStringBuilder.ToString().TrimEnd(':');
            var checkBytes = Encoding.ASCII.GetBytes(checkString);
            var checkHexCrc32 = Force.Crc32.Crc32Algorithm.Compute(checkBytes);
            var result = checkHexCrc32 == (uint)checksum;
            if (!result)
            {
                log.Write(LogLevel.Debug, $"{Id} order book {Symbol} failed checksum. Expected {checkHexCrc32}, received {checksum}");
            }
            else
            {
                log.Write(LogLevel.Trace, $"{Id} order book {Symbol} checksum OK.");
            }
            return result;
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
