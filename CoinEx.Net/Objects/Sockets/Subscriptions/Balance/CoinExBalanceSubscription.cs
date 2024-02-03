using CoinEx.Net.Objects.Models;
using CoinEx.Net.Objects.Sockets.Queries;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.MessageParsing.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoinEx.Net.Objects.Sockets.Subscriptions.Balance
{
    internal class CoinExBalanceSubscription : Subscription<CoinExQueryResponse<CoinExSubscriptionStatus>, CoinExQueryResponse<CoinExSubscriptionStatus>>
    {
        private readonly Action<DataEvent<IEnumerable<CoinExBalance>>> _handler;

        public override HashSet<string> ListenerIdentifiers { get; set; }

        public CoinExBalanceSubscription(ILogger logger, Action<DataEvent<IEnumerable<CoinExBalance>>> handler) : base(logger, true)
        {
            _handler = handler;
            ListenerIdentifiers = new HashSet<string> { "asset.update" };
        }

        public override Task<CallResult> DoHandleMessageAsync(SocketConnection connection, DataEvent<object> message)
        {
            var data = (CoinExSocketUpdate<CoinExBalanceUpdate>)message.Data;
            foreach (var item in data.Data.Balances)
                item.Value.Asset = item.Key;

            _handler.Invoke(message.As(data.Data.Balances.Values.AsEnumerable(), null, SocketUpdateType.Update));
            return Task.FromResult(new CallResult(null));
        }

        public override Type? GetMessageType(IMessageAccessor message) => typeof(CoinExSocketUpdate<CoinExBalanceUpdate>);
        public override Query? GetSubQuery(SocketConnection connection) => new CoinExQuery<CoinExSubscriptionStatus>("asset.subscribe", Array.Empty<object>(), Authenticated);
        public override Query? GetUnsubQuery() => new CoinExQuery<CoinExSubscriptionStatus>("asset.unsubscribe", Array.Empty<object>(), Authenticated);
    }
}
