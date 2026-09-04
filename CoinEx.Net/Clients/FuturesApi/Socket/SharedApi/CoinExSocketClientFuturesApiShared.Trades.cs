using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.FuturesApi
{
    internal partial class CoinExSocketClientFuturesSharedApi
    {

        public SubscribeTradeOptions SubscribeTradeOptions { get; } = new SubscribeTradeOptions(_exchangeName, false)
        {
            SupportsMultipleSymbols = true
        };
        #region Subscribe To Trade Updates

        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(SubscribeTradeRequest request, Action<DataEvent<SharedTrade[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribeTradeOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(_exchangeName, validationError);

            var symbols = request.Symbols?.Length > 0 ? request.Symbols.Select(x => x.GetSymbol(FormatSymbol)) : new[] { request.Symbol!.GetSymbol(FormatSymbol) };
            var result = await _api.SubscribeToTradeUpdatesAsync(symbols, update => handler(update.ToType(update.Data.Select(x => 
                new SharedTrade(ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, update.Symbol), update.Symbol!, new SharedOrderQuantity(x.Quantity), x.Price, x.Timestamp)
            {
                Side = x.Side == OrderSide.Buy ? SharedOrderSide.Buy : SharedOrderSide.Sell
            }).ToArray())), ct).ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
