using CoinEx.Net.Objects.Models.V2;
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
    internal class CoinExOrderBookSubscription : Subscription<CoinExSocketResponse, CoinExSocketResponse>
    {
        private IEnumerable<string> _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<CoinExOrderBook>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }
        public CoinExOrderBookSubscription(ILogger logger, IEnumerable<string> symbols, Dictionary<string, object> parameters, Action<DataEvent<CoinExOrderBook>> handler) : base(logger, false)
        {
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            ListenerIdentifiers = new HashSet<string>(_symbols.Select(x => "depth.update" + x));
        }

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<CoinExOrderBook>)message.Data;
            _handler.Invoke(message.As(data.Data, data.Method, data.Data.Symbol, ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                .WithDataTimestamp(data.Data.Data.UpdateTime));
            return new CallResult(null);
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<CoinExOrderBook>);

        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery("depth.subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery("depth.unsubscribe", new Dictionary<string, object> { {"market_list", _symbols } }, false, 1);
    }
}
