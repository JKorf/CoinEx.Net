using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using CryptoExchange.Net.Sockets.Default;
using CryptoExchange.Net.Sockets.Default.Routing;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Objects.Sockets.V2.Queries
{
    internal class CoinExQuery : Query<CoinExSocketResponse>
    {
        private readonly SocketApiClient _client;

        public CoinExQuery(SocketApiClient client, string method, Dictionary<string, object> parameters, bool authenticated = false, int weight = 1) : base(new CoinExSocketRequest
        {
            Id = ExchangeHelpers.NextId(),
            Method = method,
            Parameters = parameters
        }, authenticated, weight)
        {
            _client = client;
            var id = ((CoinExSocketRequest)Request).Id.ToString();

            MessageRouter = MessageRouter.CreateForQuery<CoinExSocketResponse>(id, HandleMessage);
        }

        public CallResult<CoinExSocketResponse> HandleMessage(SocketConnection connection, DateTime receiveTime, string? originalData, CoinExSocketResponse message)
        {
            if (message.Code != 0)
                return CallResult<CoinExSocketResponse>.Fail(new ServerError(message.Code, _client.GetErrorInfo(message.Code, message.Message)), originalData);

            return CallResult<CoinExSocketResponse>.Ok(message, originalData);
        }
    }
}
