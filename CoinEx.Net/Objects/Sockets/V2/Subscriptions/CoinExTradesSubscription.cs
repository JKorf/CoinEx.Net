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
    internal class CoinExTradesSubscription : Subscription<CoinExSocketResponse, CoinExSocketResponse>
    {
        private readonly SocketApiClient _client;
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<CoinExTrade[]>> _handler;

        public CoinExTradesSubscription(ILogger logger, SocketApiClient client, IEnumerable<string>? symbols, Dictionary<string, object> parameters, Action<DataEvent<CoinExTrade[]>> handler) : base(logger, false)
        {
            _client = client;
            _symbols = symbols;
            _parameters = parameters;
            _handler = handler;
            MessageMatcher = MessageMatcher.Create<CoinExSocketUpdate<CoinExTradeWrapper>>("deals.update", DoHandleMessage);
        }

        public CallResult DoHandleMessage(SocketConnection connection, DataEvent<CoinExSocketUpdate<CoinExTradeWrapper>> message)
        {
            var relevant = message.Data.Data.Trades.Where(d => (_symbols?.Any() != true) || _symbols.Contains(message.Data.Data.Symbol)).ToArray();
            if (!relevant.Any() || !message.Data.Data.Trades.Any())
                return CallResult.SuccessResult;

            _handler.Invoke(message.As<CoinExTrade[]>(relevant, message.Data.Method, message.Data.Data.Symbol, ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update)
                .WithDataTimestamp(message.Data.Data.Trades.Max(x => x.Timestamp)));
            return CallResult.SuccessResult;
        }

        public override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery(_client, "deals.subscribe", _parameters, false, 1);

        public override Query? GetUnsubQuery()
            => new CoinExQuery(_client, "deals.unsubscribe", _parameters, false, 1);
    }
}
