using CoinEx.Net.Objects.Models.V2;
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
    internal class CoinExTradesSubscription : Subscription
    {
        private readonly SocketApiClient _client;
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<CoinExTrade[]>> _handler;

        public CoinExTradesSubscription(ILogger logger, SocketApiClient client, IEnumerable<string> symbols, Dictionary<string, object> parameters, Action<DataEvent<CoinExTrade[]>> handler) : base(logger, false)
        {
            _client = client;
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            MessageMatcher = MessageMatcher.Create<CoinExSocketUpdate<CoinExTradeWrapper>>("deals.update", DoHandleMessage);
            MessageRouter = MessageRouter.CreateWithOptionalTopicFilters<CoinExSocketUpdate<CoinExTradeWrapper>>("deals.update", symbols, DoHandleMessage);
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, CoinExSocketUpdate<CoinExTradeWrapper> message)
        {
            //var relevant = message.Data.Trades.Where(d => (_symbols?.Any() != true) || _symbols.Contains(message.Data.Symbol)).ToArray();
            //if (!relevant.Any() || !message.Data.Trades.Any())
            //    return CallResult.SuccessResult;

            _handler.Invoke(new DataEvent<CoinExTrade[]>(message.Data.Trades, receiveTime, originalData)
                .WithStreamId(message.Method)
                .WithSymbol(message.Data.Symbol)
                .WithDataTimestamp(message.Data.Trades.Max(x => x.Timestamp))
                .WithUpdateType(ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        protected override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery(_client, "deals.subscribe", _parameters, false, 1);

        protected override Query? GetUnsubQuery(SocketConnection connection)
            => new CoinExQuery(_client, "deals.unsubscribe", _parameters, false, 1);
    }
}
