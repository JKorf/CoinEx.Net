using CoinEx.Net.Objects.Models.Socket;
using CoinEx.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.Depth
{
    internal class CoinExDepthSubscription : Subscription<CoinExQueryResponse<CoinExSubscriptionStatus>, CoinExQueryResponse<CoinExSubscriptionStatus>>
    {
        private readonly string? _symbol;
        private readonly IEnumerable<object> _parameters;
        private readonly Action<DataEvent<CoinExSocketOrderBook>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }

        public CoinExDepthSubscription(ILogger logger, string? symbol, IEnumerable<object> parameters, Action<DataEvent<CoinExSocketOrderBook>> handler) : base(logger, false)
        {
            _parameters = parameters;
            _symbol = symbol;
            _handler = handler;
            ListenerIdentifiers = new HashSet<string> { "depth.update" + symbol };
        }

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<CoinExOrderBookUpdate>)message.Data;
            _handler.Invoke(message.As(data.Data.Book, data.Method, _symbol, data.Data.Snapshot ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            return new CallResult(null);
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<CoinExOrderBookUpdate>);
        public override Query? GetSubQuery(SocketConnection connection) => new CoinExQuery<CoinExSubscriptionStatus>("depth.subscribe", _parameters, Authenticated);
        public override Query? GetUnsubQuery() => new CoinExQuery<CoinExSubscriptionStatus>("depth.unsubscribe", _parameters, Authenticated);
    }
}
