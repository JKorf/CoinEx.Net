using CoinEx.Net.Objects.Models.Socket;
using CoinEx.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.State
{
    internal class CoinExStateSubscription : Subscription<CoinExQueryResponse<CoinExSubscriptionStatus>, CoinExQueryResponse<CoinExSubscriptionStatus>>
    {
        private readonly string? _symbol;
        private readonly IEnumerable<object> _parameters;
        private readonly Action<DataEvent<IEnumerable<CoinExSocketSymbolState>>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }

        public CoinExStateSubscription(ILogger logger, string? symbol, IEnumerable<object> parameters, Action<DataEvent<IEnumerable<CoinExSocketSymbolState>>> handler) : base(logger, false)
        {
            _parameters = parameters;
            _symbol = symbol;
            _handler = handler;
            ListenerIdentifiers = new HashSet<string> { "state.update" };
        }

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<IEnumerable<Dictionary<string, CoinExSocketSymbolState>>>)message.Data;
            foreach (var item in data.Data.First())
                item.Value.Symbol = item.Key;

            var relevant = data.Data.First().Where(d => _symbol == null || d.Key == _symbol).Select(d => d.Value);
            if (!relevant.Any())
                return new CallResult(null);

            _handler.Invoke(message.As(relevant, data.Method, _symbol, SocketUpdateType.Update));
            return new CallResult(null);
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<IEnumerable<Dictionary<string, CoinExSocketSymbolState>>>);
        public override Query? GetSubQuery(SocketConnection connection) => new CoinExQuery<CoinExSubscriptionStatus>("state.subscribe", _parameters, Authenticated);
        public override Query? GetUnsubQuery() => new CoinExQuery<CoinExSubscriptionStatus>("state.unsubscribe", _parameters, Authenticated);
    }
}
