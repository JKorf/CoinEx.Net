using CryptoExchange.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;
using CryptoExchange.Net.Authentication;
using System.Threading;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Objects.Sockets;
using CoinEx.Net.Objects.Sockets.Queries;
using CoinEx.Net.Objects.Sockets;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Clients;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net.Converters.SystemTextJson;
using System.Net.WebSockets;
using CoinEx.Net.Objects.Sockets.V2.Subscriptions;

namespace CoinEx.Net.Clients.SpotApi
{
    /// <inheritdoc cref="ICoinExSocketClientSpotApi" />
    public class CoinExSocketClientSpotApi : SocketApiClient, ICoinExSocketClientSpotApi
    {
        #region fields
        /// <inheritdoc />
        public new CoinExSocketOptions ClientOptions => (CoinExSocketOptions)base.ClientOptions;

        private static readonly MessagePath _idPath = MessagePath.Get().Property("id");
        private static readonly MessagePath _methodPath = MessagePath.Get().Property("method");
        private static readonly MessagePath _symbolPath = MessagePath.Get().Property("data").Property("market");
        private static readonly MessagePath _symbolPathDepth = MessagePath.Get().Property("params").Index(2);
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExSocketClient with default options
        /// </summary>
        internal CoinExSocketClientSpotApi(ILogger logger, CoinExSocketOptions options)
            : base(logger, options.Environment.SocketBaseAddress, options, options.SpotOptions)
        {
            RegisterPeriodicQuery("Ping", TimeSpan.FromMinutes(1), q => (new CoinExQuery<string>("server.ping", new object[] { })), null);
        }
        #endregion

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new CoinExAuthenticationProvider(credentials, ClientOptions.NonceProvider ?? new CoinExNonceProvider());

        #region methods

        /// <inheritdoc />
        protected override IByteMessageAccessor CreateAccessor() => new SystemTextJsonByteMessageAccessor();
        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer();

        /// <inheritdoc />
        public override string? GetListenerIdentifier(IMessageAccessor messageAccessor)
        {
            var id = messageAccessor.GetValue<int?>(_idPath);
            if (id != null)
                return id.ToString();

            var method = messageAccessor.GetValue<string>(_methodPath);
            if (!string.Equals(method, "state.update", StringComparison.Ordinal))
            {
                var symbol = messageAccessor.GetValue<string>(_symbolPath);
                return method + symbol;
            }

            //if (string.Equals(method, "depth.update", StringComparison.Ordinal))
            //{
            //    var symbol = messageAccessor.GetValue<string>(_symbolPathDepth);
            //    return method + symbol;
            //}

            return method;
        }

        /// <inheritdoc />
        protected override Query? GetAuthenticationRequest()
        {
            var authProvider = (CoinExAuthenticationProvider)AuthenticationProvider!;
            var authParams = authProvider.GetSocketAuthParameters();
            return new CoinExQuery<CoinExSubscriptionStatus>("server.sign", authParams, false);
        }

        public override ReadOnlyMemory<byte> PreprocessStreamMessage(WebSocketMessageType type, ReadOnlyMemory<byte> data)
            => data.DecompressGzip();

        #region public

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(Action<DataEvent<CoinExTickerUpdateWrapper>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExTickerUpdateWrapper>(_logger, "state", new Dictionary<string, object>
            {
                { "market_list", new object[] { } }
            }, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<CallResult<UpdateSubscription>> SubscribeToTickerUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<CoinExTickerUpdateWrapper>> onMessage, CancellationToken ct = default)
        {
            var subscription = new CoinExSubscription<CoinExTickerUpdateWrapper>(_logger, "state", new Dictionary<string, object>
            {
                { "market_list", symbols }
            }, onMessage);
            return await SubscribeAsync(BaseAddress.AppendPath("v2/spot"), subscription, ct).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
