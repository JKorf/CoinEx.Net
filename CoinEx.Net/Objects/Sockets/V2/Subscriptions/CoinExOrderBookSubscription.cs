using CoinEx.Net.Objects.Models.V2;
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
    internal class CoinExOrderBookSubscription : Subscription
    {
        private readonly SocketApiClient _client;
        private IEnumerable<string> _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<CoinExOrderBook>> _handler;

        public CoinExOrderBookSubscription(ILogger logger, SocketApiClient client, string[] symbols, Dictionary<string, object> parameters, Action<DataEvent<CoinExOrderBook>> handler) : base(logger, false)
        {
            _client = client;
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;

            IndividualSubscriptionCount = Math.Min(1, symbols.Length);

            MessageRouter = MessageRouter.CreateWithTopicFilters<CoinExSocketUpdate<CoinExOrderBook>>("depth.update", symbols, DoHandleMessage);
            MessageMatcher = MessageMatcher.Create(_symbols.Select(x => new MessageHandlerLink<CoinExSocketUpdate<CoinExOrderBook>>("depth.update" + x, DoHandleMessage)).ToArray());
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, CoinExSocketUpdate<CoinExOrderBook> message)
        {
            _handler.Invoke(new DataEvent<CoinExOrderBook>(message.Data, receiveTime, originalData)
                .WithStreamId(message.Method)
                .WithSymbol(message.Data.Symbol)
                .WithDataTimestamp(message.Data.Data.UpdateTime)
                .WithUpdateType(ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        protected override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery(_client, "depth.subscribe", _parameters, false, 1);

        protected override Query? GetUnsubQuery(SocketConnection connection)
            => new CoinExQuery(_client, "depth.unsubscribe", new Dictionary<string, object> { {"market_list", _symbols } }, false, 1);
    }
}
