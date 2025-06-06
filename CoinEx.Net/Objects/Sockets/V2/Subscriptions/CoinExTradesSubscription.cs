﻿using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Objects.Sockets.V2.Queries;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinEx.Net.Objects.Sockets.V2.Subscriptions
{
    internal class CoinExTradesSubscription : Subscription<CoinExSocketResponse, CoinExSocketResponse>
    {
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<CoinExTrade[]>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }
        public CoinExTradesSubscription(ILogger logger, IEnumerable<string>? symbols, Dictionary<string, object> parameters, Action<DataEvent<CoinExTrade[]>> handler) : base(logger, false)
        {
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            ListenerIdentifiers = new HashSet<string> { "deals.update" };
        }

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<CoinExTradeWrapper>)message.Data;
            var relevant = data.Data.Trades.Where(d => (_symbols?.Any() != true) || _symbols.Contains(data.Data.Symbol)).ToArray();
            if (!relevant.Any() || !data.Data.Trades.Any())
                return CallResult.SuccessResult;

            _handler.Invoke(message.As<CoinExTrade[]>(relevant, data.Method, data.Data.Symbol, ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                .WithDataTimestamp(data.Data.Trades.Max(x => x.Timestamp)));
            return CallResult.SuccessResult;
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<CoinExTradeWrapper>);

        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery("deals.subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery("deals.unsubscribe", _parameters, false, 1);
    }
}
