using CoinEx.Net.Objects.Models.Socket;
using CoinEx.Net.Objects.Sockets.Queries;
using CoinEx.Net.Objects.Sockets.Subscriptions.Balance;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.Deals
{
    internal class CoinExDealsSubscription : Subscription<CoinExQueryResponse<CoinExSubscriptionStatus>, CoinExQueryResponse<CoinExSubscriptionStatus>>
    {
        private readonly string? _symbol;
        private readonly IEnumerable<object> _parameters;
        private readonly Action<DataEvent<IEnumerable<CoinExSocketSymbolTrade>>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }

        public CoinExDealsSubscription(ILogger logger, string? symbol, IEnumerable<object> parameters, Action<DataEvent<IEnumerable<CoinExSocketSymbolTrade>>> handler) : base(logger, false)
        {
            _parameters = parameters;
            _symbol = symbol;
            _handler = handler;
            ListenerIdentifiers = new HashSet<string> { "deals.update" + symbol };
        }


        public override CallResult DoHandleMessage(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<CoinExDealsUpdate>)message.Data;
            _handler.Invoke(message.As(data.Data.Trades, data.Method, _symbol, ConnectionInvocations == 1 ? SocketUpdateType.Snapshot : SocketUpdateType.Update));
            return new CallResult(null);
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<CoinExDealsUpdate>);
        public override Query? GetSubQuery(SocketConnection connection) => new CoinExQuery<CoinExSubscriptionStatus>("deals.subscribe", _parameters, Authenticated);
        public override Query? GetUnsubQuery() => new CoinExQuery<CoinExSubscriptionStatus>("deals.unsubscribe", _parameters, Authenticated);
    }
}
