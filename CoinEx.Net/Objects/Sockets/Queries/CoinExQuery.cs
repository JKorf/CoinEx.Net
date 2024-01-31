using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinEx.Net.Objects.Sockets.Queries
{
    internal class CoinExQuery<T> : Query<CoinExQueryResponse<T>>
    {
        public override HashSet<string> ListenerIdentifiers { get; set; }

        public CoinExQuery(string method, IEnumerable<object> parameters, bool authenticated = false, int weight = 1) : base(new CoinExSocketRequest
        {
            Id = ExchangeHelpers.NextId(),
            Method = method,
            Parameters = parameters
        }, authenticated, weight)
        {
            ListenerIdentifiers = new HashSet<string>() { ((CoinExSocketRequest)Request).Id.ToString() };
        }

        public override Task<CallResult<CoinExQueryResponse<T>>> HandleMessageAsync(SocketConnection connection, DataEvent<CoinExQueryResponse<T>> message)
        {
            if (message.Data.Error != null)
                return Task.FromResult(new CallResult<CoinExQueryResponse<T>>(new ServerError(message.Data.Error.Code, message.Data.Error.Message)));

            return Task.FromResult(new CallResult<CoinExQueryResponse<T>>(message.Data, message.OriginalData, null));
        }
    }
}
