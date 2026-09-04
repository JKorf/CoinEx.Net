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
    internal partial class CoinExSocketClientFuturesSharedApi :
        SharedApiBase,
        ICoinExSocketClientFuturesApiShared,
        ICoinExSocketClientFuturesSharedApi
    {
        private readonly CoinExSocketClientFuturesApi _api;

        private const string _topicId = "CoinExFutures";
        private const string _exchangeName = "CoinEx";
        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(CoinExExchange.Metadata, this);

        public CoinExSocketClientFuturesSharedApi(CoinExSocketClientFuturesApi api)
            : base(
                  SharedTransport.Socket,
                  api.Exchange,
                  [TradingMode.PerpetualLinear, TradingMode.PerpetualInverse],
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                SubscribeAllTickersOptions,
                SubscribeTickerOptions,
                SubscribeTradeOptions,
                SubscribeBookTickerOptions,
                SubscribeOrderBookOptions,
                SubscribeBalanceOptions,
                SubscribeFuturesOrderOptions,
                SubscribeUserTradeOptions,
                SubscribePositionOptions
                );
        }
    }
}
