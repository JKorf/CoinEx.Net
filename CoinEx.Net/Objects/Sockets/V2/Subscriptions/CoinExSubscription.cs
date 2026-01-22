using CoinEx.Net.Objects.Sockets.V2.Queries;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinEx.Net.Objects.Sockets.V2.Subscriptions
{
    internal class CoinExSubscription<T> : Subscription
    {
        private readonly SocketApiClient _client;
        private string _topic;
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DateTime, string?, int, CoinExSocketUpdate<T>> _handler;

        public CoinExSubscription(ILogger logger, SocketApiClient client, string topic, string[]? symbols, Dictionary<string, object> parameters, Action<DateTime, string?, int, CoinExSocketUpdate<T>> handler, bool authenticated = false, bool firstUpdateIsSnapshot = false) : base(logger, authenticated)
        {
            _client = client;
            _topic = topic;
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;

            IndividualSubscriptionCount = Math.Max(1, symbols?.Length ?? 1);

            MessageRouter = MessageRouter.CreateWithOptionalTopicFilters<CoinExSocketUpdate<T>>(_topic + ".update", symbols, DoHandleMessage);
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, CoinExSocketUpdate<T> message)
        {
            _handler.Invoke(receiveTime, originalData, ConnectionInvocations, message);
            return CallResult.SuccessResult;
        }


        protected override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery(_client, _topic + ".subscribe", _parameters, false, 1);

        protected override Query? GetUnsubQuery(SocketConnection connection)
            => new CoinExQuery(_client, _topic + ".unsubscribe", _parameters, false, 1);
    }
}
