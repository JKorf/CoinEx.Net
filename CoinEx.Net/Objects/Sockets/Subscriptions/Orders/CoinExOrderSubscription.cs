using CoinEx.Net.Objects.Models.Socket;
using CoinEx.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.MessageParsing.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.Orders
{
    internal class CoinExOrderSubscription : Subscription<CoinExQueryResponse<CoinExSubscriptionStatus>, CoinExQueryResponse<CoinExSubscriptionStatus>>
    {
        private readonly IEnumerable<string>? _symbols;
        private readonly Action<DataEvent<CoinExSocketOrderUpdate>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }

        public CoinExOrderSubscription(ILogger logger, IEnumerable<string> symbols, Action<DataEvent<CoinExSocketOrderUpdate>> handler) : base(logger, true)
        {
            _symbols = symbols;
            _handler = handler;
            ListenerIdentifiers = new HashSet<string> { "order.update" };
        }

        public override Task<CallResult> DoHandleMessageAsync(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<CoinExOrderUpdate>)message.Data;
            _handler.Invoke(message.As(new CoinExSocketOrderUpdate
            {
                UpdateType = data.Data.UpdateType,
                Order = data.Data.Order
            }, data.Data.Order.Symbol, SocketUpdateType.Update));
            return Task.FromResult(new CallResult(null));
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<CoinExOrderUpdate>);
        public override Query? GetSubQuery(SocketConnection connection) => new CoinExQuery<CoinExSubscriptionStatus>("order.subscribe", _symbols?.Any() != true ? Array.Empty<object>() : _symbols.ToArray(), Authenticated);
        public override Query? GetUnsubQuery() => new CoinExQuery<CoinExSubscriptionStatus>("order.unsubscribe", _symbols?.Any() != true ? Array.Empty<object>() : _symbols.ToArray(), Authenticated);
    }
}
