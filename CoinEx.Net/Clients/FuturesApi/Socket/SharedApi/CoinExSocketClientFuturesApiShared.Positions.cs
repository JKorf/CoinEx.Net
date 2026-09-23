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
        public SubscribePositionOptions SubscribePositionOptions { get; } = new SubscribePositionOptions(_exchangeName, true);
        #region Subscribe To Position Updates

        public async Task<WebSocketResult<UpdateSubscription>> SubscribeToPositionUpdatesAsync(SubscribePositionRequest request, Action<DataEvent<SharedPosition[]>> handler, CancellationToken ct)
        {
            var validationError = SubscribePositionOptions.ValidateRequest(request, this);
            if (validationError != null)
                return WebSocketResult.Fail<UpdateSubscription>(_exchangeName, validationError);

            var result = await _api.SubscribeToPositionUpdatesAsync(
                update => handler(update.ToType<SharedPosition[]>(new[] { 
                    new SharedPosition(
                        ExchangeSymbolCache.ParseSymbol(_topicId, _api.EnvironmentName, null, update.Data.Position.Symbol), 
                        update.Data.Position.Symbol,
                        new SharedOrderQuantity(update.Data.Position.OpenInterest),
                        update.Data.Position.UpdateTime)
                    {
                        AverageOpenPrice = update.Data.Position.AverageEntryPrice,
                        PositionMode = SharedPositionMode.OneWay,
                        PositionSide = update.Data.Position.Side == Enums.PositionSide.Short ? SharedPositionSide.Short : SharedPositionSide.Long,
                        LiquidationPrice = update.Data.Position.LiquidationPrice,
                        Leverage = update.Data.Position.Leverage,
                        UnrealizedPnl = update.Data.Position.UnrealizedPnl,
                        TakeProfitPrice = update.Data.Position.TakeProfitPrice == 0 ? null : update.Data.Position.TakeProfitPrice,
                        StopLossPrice = update.Data.Position.StopLossPrice == 0 ? null : update.Data.Position.StopLossPrice,
                    } 
                })),
                ct: ct).ConfigureAwait(false);

            return result;
        }

        #endregion

    }
}
