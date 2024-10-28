using CryptoExchange.Net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Authentication;
using CoinEx.Net.Objects.Internal;
using Microsoft.Extensions.Logging;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CryptoExchange.Net.SharedApis;

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc cref="ICoinExRestClientFuturesApi" />
    internal partial class CoinExRestClientFuturesApi : RestApiClient, ICoinExRestClientFuturesApi
    {
        #region fields
        internal TimeSyncState _timeSyncState = new TimeSyncState("CoinEx V2 API");

        /// <inheritdoc />
        public new CoinExRestOptions ClientOptions => (CoinExRestOptions)base.ClientOptions;
        #endregion

        /// <inheritdoc />
        public string ExchangeName => "CoinEx";

        #region Api clients
        /// <inheritdoc />
        public ICoinExRestClientFuturesApiAccount Account { get; }
        /// <inheritdoc />
        public ICoinExRestClientFuturesApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public ICoinExRestClientFuturesApiTrading Trading { get; }
        #endregion

        internal readonly string _brokerId;

        #region ctor
        internal CoinExRestClientFuturesApi(ILogger logger, HttpClient? httpClient, CoinExRestOptions options) :
            base(logger, httpClient, options.Environment.RestBaseAddress, options, options.FuturesOptions)
        {
            Account = new CoinExRestClientFuturesApiAccount(this);
            ExchangeData = new CoinExRestClientFuturesApiExchangeData(this);
            Trading = new CoinExRestClientFuturesApiTrading(this);

            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;

            _brokerId = !string.IsNullOrEmpty(options.BrokerId) ? options.BrokerId! : "147866029";

        }
        #endregion

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => CoinExExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

        /// <inheritdoc />
        protected override IStreamMessageAccessor CreateAccessor() => new SystemTextJsonStreamMessageAccessor();
        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer();

        public ICoinExRestClientFuturesApiShared SharedClient => this;

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new CoinExV2AuthenticationProvider(credentials);

        #region methods
        internal async Task<WebCallResult> ExecuteAsync(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false)
        {
            var result = await SendRequestAsync<CoinExApiResult>(uri, method, ct, parameters, signed, requestWeight: 0).ConfigureAwait(false);
            if (!result)
                return result.AsDataless();

            if (result.Data.Code != 0)
                return result.AsDatalessError(new ServerError(result.Data.Code, result.Data.Message!));

            return result.AsDataless();
        }

        internal async Task<WebCallResult<T>> ExecuteAsync<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            var result = await SendRequestAsync<CoinExApiResult<T>>(uri, method, ct, parameters, signed, requestWeight: 0).ConfigureAwait(false);
            if (!result)
                return result.As<T>(default);

            if (result.Data.Code != 0)
                return result.AsError<T>(new ServerError(result.Data.Code, result.Data.Message!));

            return result.As(result.Data.Data);
        }

        internal async Task<WebCallResult<CoinExPaginated<T>>> ExecutePaginatedAsync<T>(Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            var result = await SendRequestAsync<CoinExPageApiResult<IEnumerable<T>>>(uri, method, ct, parameters, signed, requestWeight: 0).ConfigureAwait(false);
            if (!result)
                return result.As<CoinExPaginated<T>>(default);

            if (result.Data.Code != 0)
                return result.AsError<CoinExPaginated<T>>(new ServerError(result.Data.Code, result.Data.Message!));

            var resultPage = new CoinExPaginated<T>
            {
                HasNext = result.Data.Pagination.HasNext,
                Total = result.Data.Pagination.Total,
                Items = result.Data.Data
            };

            return result.As(resultPage);
        }

        internal Uri GetUri(string path) => new Uri(BaseAddress.AppendPath(path));
        #endregion

        /// <inheritdoc />
        protected override ServerError? TryParseError(IMessageAccessor accessor)
        {
            if (!accessor.IsJson)
                return new ServerError(accessor.GetOriginalString());

            var code = accessor.GetValue<int?>(MessagePath.Get().Property("code"));
            if (code == 0)
                return null;
            
            var msg = accessor.GetValue<string>(MessagePath.Get().Property("message"));
            if (msg == null)
                return new ServerError(accessor.GetOriginalString());

            if (code == null)
                return new ServerError(msg);

            return new ServerError(code.Value, msg);
        }

        /// <inheritdoc />
        protected override async Task<WebCallResult<DateTime>> GetServerTimestampAsync() => await ExchangeData.GetServerTimeAsync().ConfigureAwait(false);

        /// <inheritdoc />
        public override TimeSyncInfo? GetTimeSyncInfo()
            => new TimeSyncInfo(_logger, (ApiOptions.AutoTimestamp ?? ClientOptions.AutoTimestamp), (ApiOptions.TimestampRecalculationInterval ?? ClientOptions.TimestampRecalculationInterval), _timeSyncState);

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset()
            => _timeSyncState.TimeOffset;
    }
}
