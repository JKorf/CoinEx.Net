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

        public CoinExOrderBookSubscription(ILogger logger, IEnumerable<string> symbols, Dictionary<string, object> parameters, Action<DataEvent<CoinExOrderBook>> handler) : base(logger, false)
        {
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            MessageMatcher = MessageMatcher.Create(_symbols.Select(x => new MessageHandlerLink<CoinExSocketUpdate<CoinExOrderBook>>("depth.update" + x, DoHandleMessage)).ToArray());
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<CoinExSocketUpdate<CoinExOrderBook>> message)
        {
            _handler.Invoke(message.As(message.Data.Data, message.Data.Method, message.Data.Data.Symbol, ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                .WithDataTimestamp(message.Data.Data.Data.UpdateTime));
            return CallResult.SuccessResult;
        }

        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery("depth.subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery("depth.unsubscribe", new Dictionary<string, object> { {"market_list", _symbols } }, false, 1);
    }
}
