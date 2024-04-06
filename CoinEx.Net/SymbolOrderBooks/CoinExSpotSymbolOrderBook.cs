using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Clients;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.OrderBook;
using Microsoft.Extensions.Logging;

namespace CoinEx.Net.SymbolOrderBooks
{
    /// <summary>
    /// Symbol order book implementation
    /// </summary>
    public class CoinExSpotSymbolOrderBook: SymbolOrderBook
    {
        private readonly ICoinExSocketClient _socketClient;
        private readonly bool _clientOwner;
        private readonly TimeSpan _initialDataTimeout;

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        public CoinExSpotSymbolOrderBook(string symbol, Action<CoinExOrderBookOptions>? optionsDelegate = null)
            : this(symbol, optionsDelegate, null, null)
        {
        }

        /// <summary>
        /// Create a new order book instance
        /// </summary>
        /// <param name="symbol">The symbol the order book is for</param>
        /// <param name="optionsDelegate">Option configuration delegate</param>
        /// <param name="logger">Logger</param>
        /// <param name="socketClient">Socket client instance</param>
        public CoinExSpotSymbolOrderBook(string symbol,
            Action<CoinExOrderBookOptions>? optionsDelegate,
            ILoggerFactory? logger,
            ICoinExSocketClient? socketClient) : base(logger, "CoinEx", "Spot", symbol)
        {
            var options = CoinExOrderBookOptions.Default.Copy();
            if (optionsDelegate != null)
                optionsDelegate(options);
            Initialize(options);

            _strictLevels = false;
            _sequencesAreConsecutive = false;
            _initialDataTimeout = options?.InitialDataTimeout ?? TimeSpan.FromSeconds(30);

            _socketClient = socketClient ?? new CoinExSocketClient();
            _clientOwner = socketClient == null;
            Levels = options?.Limit ?? 20;
        }

        /// <inheritdoc />
        protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct)
        {
            var result = await _socketClient.SpotApiV2.SubscribeToOrderBookUpdatesAsync(Symbol, Levels!.Value, null, true, HandleUpdate).ConfigureAwait(false);
            if (!result)
                return result;

            if (ct.IsCancellationRequested)
            {
                await result.Data.CloseAsync().ConfigureAwait(false);
                return result.AsError<UpdateSubscription>(new CancellationRequestedError());
            }

            Status = OrderBookStatus.Syncing;

            var setResult = await WaitForSetOrderBookAsync(_initialDataTimeout, ct).ConfigureAwait(false);
            return setResult ? result : new CallResult<UpdateSubscription>(setResult.Error!);
        }

        /// <inheritdoc />
        protected override async Task<CallResult<bool>> DoResyncAsync(CancellationToken ct)
        {
            return await WaitForSetOrderBookAsync(_initialDataTimeout, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override void DoReset()
        {
        }

        private void HandleUpdate(DataEvent<CoinExOrderBook> data)
        {
            SetInitialOrderBook(DateTime.UtcNow.Ticks, data.Data.Data.Bids, data.Data.Data.Asks);
            AddChecksum((int)data.Data.Data.Checksum);
        }

        /// <inheritdoc />
        protected override bool DoChecksum(int checksum)
        {
            var checkStringBuilder = new StringBuilder();
            foreach (var bid in _bids)
            {
                checkStringBuilder.Append(bid.Value.Price.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + bid.Value.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":");
            }
            foreach (var ask in _asks)
            {
                checkStringBuilder.Append(ask.Value.Price.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + ask.Value.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":");
            }
            var checkString = checkStringBuilder.ToString().TrimEnd(':');
            var checkBytes = Encoding.ASCII.GetBytes(checkString);
            var checkHexCrc32 = Force.Crc32.Crc32Algorithm.Compute(checkBytes);
            var result = checkHexCrc32 == (uint)checksum;
            if (!result)
            {
                _logger.Log(LogLevel.Debug, $"{Api} order book {Symbol} failed checksum. Expected {checkHexCrc32}, received {checksum}");
            }
            else
            {
                _logger.Log(LogLevel.Trace, $"{Api} order book {Symbol} checksum OK.");
            }
            return result;
        }
        /// <inheritdoc />
        /// <summary>
        /// Dispose
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (_clientOwner)
                _socketClient?.Dispose();

            base.Dispose(disposing);
        }
    }
}
