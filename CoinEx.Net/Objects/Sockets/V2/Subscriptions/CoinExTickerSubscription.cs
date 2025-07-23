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
    internal class CoinExTickerSubscription : Subscription<CoinExSocketResponse, CoinExSocketResponse>
    {
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<CoinExTicker[]>> _handler;

        public CoinExTickerSubscription(ILogger logger, IEnumerable<string>? symbols, Dictionary<string, object> parameters, Action<DataEvent<CoinExTicker[]>> handler) : base(logger, false)
        {
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            if (symbols?.Any() != true)
                MessageMatcher = MessageMatcher.Create<CoinExSocketUpdate<CoinExTickerUpdateWrapper>>("state.update", DoHandleMessage);
            else
                MessageMatcher = MessageMatcher.Create(symbols.Select(x => new MessageHandlerLink<CoinExSocketUpdate<CoinExTickerUpdateWrapper>>("state.update" + x, DoHandleMessage)).ToArray());
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<CoinExSocketUpdate<CoinExTickerUpdateWrapper>> message)
        {
            var relevant = message.Data.Data.Tickers.Where(d => _symbols == null || _symbols.Contains(d.Symbol)).ToArray();
            if (!relevant.Any())
                return CallResult.SuccessResult;

            _handler.Invoke(message.As(relevant, message.Data.Method, null, SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery("state.subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery("state.unsubscribe", _parameters, false, 1);
    }
}
