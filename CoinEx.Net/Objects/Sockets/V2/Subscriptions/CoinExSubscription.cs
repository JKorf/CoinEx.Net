using CoinEx.Net.Objects.Sockets.V2.Queries;
using CryptoExchange.Net.Clients;
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
        private readonly SocketApiClient _client;
        private string _topic;
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<T>> _handler;
        private bool _firstUpdateIsSnapshot;

        public CoinExSubscription(ILogger logger, SocketApiClient client, string topic, IEnumerable<string>? symbols, Dictionary<string, object> parameters, Action<DataEvent<T>> handler, bool authenticated = false, bool firstUpdateIsSnapshot = false) : base(logger, authenticated)
        {
            _client = client;
            _topic = topic;
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            _firstUpdateIsSnapshot = firstUpdateIsSnapshot;
            if (symbols?.Any() != true)
                MessageMatcher = MessageMatcher.Create<CoinExSocketUpdate<T>>(_topic + ".update", DoHandleMessage);
            else
                MessageMatcher = MessageMatcher.Create(_symbols!.Select(x => new MessageHandlerLink<CoinExSocketUpdate<T>>(_topic + ".update" + x, DoHandleMessage)).ToArray());
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<CoinExSocketUpdate<T>> message)
        {
            _handler.Invoke(message.As(message.Data.Data, message.Data.Method, null, _firstUpdateIsSnapshot && ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }


        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery(_client, _topic + ".subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery(_client, _topic + ".unsubscribe", _parameters, false, 1);
    }
}
