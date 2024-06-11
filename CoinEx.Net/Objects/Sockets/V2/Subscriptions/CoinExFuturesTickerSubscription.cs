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
    internal class CoinExFuturesTickerSubscription : Subscription<CoinExSocketResponse, CoinExSocketResponse>
    {
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<IEnumerable<CoinExFuturesTickerUpdate>>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }
        public CoinExFuturesTickerSubscription(ILogger logger, IEnumerable<string>? symbols, Dictionary<string, object> parameters, Action<DataEvent<IEnumerable<CoinExFuturesTickerUpdate>>> handler) : base(logger, false)
        {
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            ListenerIdentifiers = new HashSet<string> { "state.update" };
        }

        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<CoinExFuturesTickerUpdateWrapper>)message.Data;
            var relevant = data.Data.Tickers.Where(d => _symbols == null || _symbols.Contains(d.Symbol)).ToList();
            if (!relevant.Any())
                return new CallResult(null);

            _handler.Invoke(message.As<IEnumerable<CoinExFuturesTickerUpdate>>(relevant, data.Method, null, SocketUpdateType.Update));
            return new CallResult(null);
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<CoinExFuturesTickerUpdateWrapper>);

        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery("state.subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery("state.unsubscribe", _parameters, false, 1);
    }
}
