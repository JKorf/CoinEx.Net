using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
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
            MessageMatcher = MessageMatcher.Create<CoinExSocketResponse>(((CoinExSocketRequest)Request).Id.ToString(), HandleMessage);
        }

        public CallResult<CoinExSocketResponse> HandleMessage(SocketConnection connection, DataEvent<CoinExSocketResponse> message)
        {
            if (message.Data.Code != 0)
                return new CallResult<CoinExSocketResponse>(new ServerError(message.Data.Code, _client.GetErrorInfo(message.Data.Code, message.Data.Message)));

            return new CallResult<CoinExSocketResponse>(message.Data, message.OriginalData, null);
        }
    }
}
