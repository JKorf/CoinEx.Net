using CoinEx.Net.Clients.FuturesApi;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
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

namespace CoinEx.Net.Clients.SpotApiV2
{
    internal partial class CoinExSocketClientSpotSharedApi : 
        SharedApiBase,
        ICoinExSocketClientSpotApiShared,
        ICoinExSocketClientSpotSharedApi
    {
        private readonly CoinExSocketClientSpotApi _api;

        private const string _topicId = "CoinExSpot";
        private const string _exchangeName = "CoinEx";

        public override SharedClientInfo Discover() => SharedUtils.GetClientInfo(CoinExExchange.Metadata, this);

        public CoinExSocketClientSpotSharedApi(CoinExSocketClientSpotApi api)
            : base(
                  SharedTransport.Rest,
                  api.Exchange,
                  [TradingMode.Spot],
                  () => api.Authenticated,
                  api.FormatSymbol)
        {
            _api = api;

            SetCapabilities(
                SubscribeTickerOptions,
                SubscribeAllTickersOptions,
                SubscribeTradeOptions,
                SubscribeBookTickerOptions,
                SubscribeOrderBookOptions,
                SubscribeBalanceOptions,
                SubscribeSpotOrderOptions,
                SubscribeUserTradeOptions
                );
        }
    }
}
