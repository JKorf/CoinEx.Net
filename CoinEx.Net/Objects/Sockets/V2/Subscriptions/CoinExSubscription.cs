using CoinEx.Net.Objects.Sockets.V2.Queries;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Sockets.V2.Subscriptions
{
    internal class CoinExSubscription<T> : Subscription<CoinExSocketResponse, CoinExSocketResponse>
    {
        private string _topic;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<T>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }
        public CoinExSubscription(ILogger logger, string topic, Dictionary<string, object> parameters, Action<DataEvent<T>> handler) : base(logger, false)
        {
            _topic = topic;
            _parameters = parameters;
            _handler = handler;
            ListenerIdentifiers = new HashSet<string> { _topic + ".update" };
        }

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (T)message.Data;
            _handler.Invoke(message.As(data));
            return new CallResult(null);
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(T);

        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery<CoinExSocketResponse>(_topic + ".subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery<CoinExSocketResponse>(_topic + ".unsubscribe", _parameters, false, 1);
    }
}
