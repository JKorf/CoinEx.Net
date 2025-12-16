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
    internal class CoinExFuturesTickerSubscription : Subscription
    {
        private readonly SocketApiClient _client;
        private IEnumerable<string>? _symbols;
        private Dictionary<string, object> _parameters;
        private Action<DataEvent<CoinExFuturesTickerUpdate[]>> _handler;

        public CoinExFuturesTickerSubscription(ILogger logger, SocketApiClient client, string[] symbols, Dictionary<string, object> parameters, Action<DataEvent<CoinExFuturesTickerUpdate[]>> handler) : base(logger, false)
        {
            _client = client;
            _symbols = symbols.Any() ? symbols : null;
            _parameters = parameters;
            _handler = handler;

            IndividualSubscriptionCount = Math.Min(1, symbols.Length);

            MessageMatcher = MessageMatcher.Create<CoinExSocketUpdate<CoinExFuturesTickerUpdateWrapper>>("state.update", DoHandleMessage);
            MessageRouter = MessageRouter.CreateWithOptionalTopicFilters<CoinExSocketUpdate<CoinExFuturesTickerUpdateWrapper>>("state.update", symbols, DoHandleMessage);
        }

        public CallResult DoHandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, CoinExSocketUpdate<CoinExFuturesTickerUpdateWrapper> message)
        {
            var relevant = message.Data.Tickers.Where(d => _symbols == null || _symbols.Contains(d.Symbol)).ToArray();
            if (!relevant.Any())
                return CallResult.SuccessResult;

            _handler.Invoke(new DataEvent<CoinExFuturesTickerUpdate[]>(CoinExExchange.ExchangeName, relevant, receiveTime, originalData)
                .WithStreamId(message.Method)
                .WithUpdateType(SocketUpdateType.Update));
            return CallResult.SuccessResult;
        }

        protected override Query? GetSubQuery(SocketConnection connection)
            => new CoinExQuery(_client, "state.subscribe", _parameters, false, 1);

        protected override Query? GetUnsubQuery(SocketConnection connection)
            => new CoinExQuery(_client, "state.unsubscribe", _parameters, false, 1);
    }
}
