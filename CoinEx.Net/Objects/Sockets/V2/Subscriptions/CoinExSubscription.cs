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
    internal class CoinExSubscription<T> : Subscription<CoinExSocketResponse, CoinExSocketResponse>
    {
        private string _topic;
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<T>> _handler;
        private bool _firstUpdateIsSnapshot;

        public override HashSet<string> ListenerIdentifiers { get; set; }
        public CoinExSubscription(ILogger logger, string topic, IEnumerable<string>? symbols, Dictionary<string, object> parameters, Action<DataEvent<T>> handler, bool authenticated = false, bool firstUpdateIsSnapshot = false) : base(logger, authenticated)
        {
            _topic = topic;
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            _firstUpdateIsSnapshot = firstUpdateIsSnapshot;
            if (symbols?.Any() != true)
                ListenerIdentifiers = new HashSet<string> { _topic + ".update" };
            else
                ListenerIdentifiers = new HashSet<string>(_symbols.Select(x => _topic + ".update" + x));
        }

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<T>)message.Data;
            _handler.Invoke(message.As(data.Data, data.Method, null, _firstUpdateIsSnapshot && ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            return new CallResult(null);
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<T>);

        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery(_topic + ".subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery(_topic + ".unsubscribe", _parameters, false, 1);
    }
}
