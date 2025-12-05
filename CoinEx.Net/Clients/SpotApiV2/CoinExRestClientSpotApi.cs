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
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CoinEx.Net.Enums;
using System.Linq;
using System.Globalization;
using CryptoExchange.Net.SharedApis;
using CryptoExchange.Net.Objects.Errors;
using System.Net.Http.Headers;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CoinEx.Net.Clients.MessageHandlers;

namespace CoinEx.Net.Clients.SpotApiV2
{
    /// <inheritdoc cref="ICoinExRestClientSpotApi" />
    internal partial class CoinExRestClientSpotApi : RestApiClient, ICoinExRestClientSpotApi
    {
        #region fields
        internal TimeSyncState _timeSyncState = new TimeSyncState("CoinEx V2 API");

        /// <inheritdoc />
        public new CoinExRestOptions ClientOptions => (CoinExRestOptions)base.ClientOptions;

        protected override ErrorMapping ErrorMapping => CoinExErrors.RestErrorMapping;

        protected override IRestMessageHandler MessageHandler { get; } = new CoinExRestMessageHandler(CoinExErrors.RestErrorMapping);
        #endregion

        /// <inheritdoc />
        public string ExchangeName => "CoinEx";

        #region Api clients
        /// <inheritdoc />
        public ICoinExRestClientSpotApiAccount Account { get; }
        /// <inheritdoc />
        public ICoinExRestClientSpotApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public ICoinExRestClientSpotApiTrading Trading { get; }
        #endregion

        #region ctor
        internal CoinExRestClientSpotApi(ILogger logger, HttpClient? httpClient, CoinExRestOptions options) :
            base(logger, httpClient, options.Environment.RestBaseAddress, options, options.SpotOptions)
        {
            Account = new CoinExRestClientSpotApiAccount(this);
            ExchangeData = new CoinExRestClientSpotApiExchangeData(this);
            Trading = new CoinExRestClientSpotApiTrading(this);

            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;
        }
        #endregion

        /// <inheritdoc />
        protected override IStreamMessageAccessor CreateAccessor() => new SystemTextJsonStreamMessageAccessor(SerializerOptions.WithConverters(CoinExExchange._serializerContext));
        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(CoinExExchange._serializerContext));

        /// <inheritdoc />
        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
            => new CoinExV2AuthenticationProvider(credentials);

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => CoinExExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

        #region methods

        internal Task<WebCallResult> SendAsync(RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null)
            => SendToAddressAsync(BaseAddress, definition, parameters, cancellationToken, weight);

        internal async Task<WebCallResult> SendToAddressAsync(string baseAddress, RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null)
        {
            var result = await base.SendAsync<CoinExApiResult>(baseAddress, definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            if (!result)
                return result.AsDataless();

            if (result.Data.Code != 0)
                return result.AsDatalessError(new ServerError(result.Data.Code, GetErrorInfo(result.Data.Code, result.Data.Message!)));

            return result.AsDataless();
        }

        internal Task<WebCallResult<T>> SendAsync<T>(RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null) where T : class
            => SendToAddressAsync<T>(BaseAddress, definition, parameters, cancellationToken, weight);

        internal async Task<WebCallResult<T>> SendToAddressAsync<T>(string baseAddress, RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null) where T : class
        {
            var result = await base.SendAsync<CoinExApiResult<T>>(baseAddress, definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            if (!result)
                return result.As<T>(default);

            if (result.Data.Code != 0)
                return result.AsError<T>(new ServerError(result.Data.Code, GetErrorInfo(result.Data.Code, result.Data.Message!)));

            return result.As(result.Data.Data);
        }

        internal async Task<WebCallResult<CoinExPaginated<T>>> SendPaginatedAsync<T>(RequestDefinition definition, ParameterCollection? parameters, CancellationToken cancellationToken, int? weight = null) where T : class
        {
            var result = await base.SendAsync<CoinExPageApiResult<T[]>>(BaseAddress, definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            if (!result)
                return result.As<CoinExPaginated<T>>(default);

            if (result.Data.Code != 0)
                return result.AsError<CoinExPaginated<T>>(new ServerError(result.Data.Code, GetErrorInfo(result.Data.Code, result.Data.Message!)));

            var resultPage = new CoinExPaginated<T>
            {
                HasNext = result.Data.Pagination.HasNext,
                Total = result.Data.Pagination.Total,
                Items = result.Data.Data
            };

            return result.As(resultPage);
        }
        #endregion

        /// <inheritdoc />
        protected override Error? TryParseError(RequestDefinition request, HttpResponseHeaders responseHeaders, IMessageAccessor accessor)
        {
            if (!accessor.IsValid)
                return new ServerError(ErrorInfo.Unknown);

            var code = accessor.GetValue<int?>(MessagePath.Get().Property("code"));
            if (code == 0)
                return null;
            
            var msg = accessor.GetValue<string>(MessagePath.Get().Property("message"));
            if (msg == null)
                return new ServerError(ErrorInfo.Unknown);

            if (code == null)
                return new ServerError(ErrorInfo.Unknown with { Message = msg });

            return new ServerError(code.Value, GetErrorInfo(code.Value, msg));
        }

        /// <inheritdoc />
        protected override async Task<WebCallResult<DateTime>> GetServerTimestampAsync() => await ExchangeData.GetServerTimeAsync().ConfigureAwait(false);

        /// <inheritdoc />
        public override TimeSyncInfo? GetTimeSyncInfo()
            => new TimeSyncInfo(_logger, (ApiOptions.AutoTimestamp ?? ClientOptions.AutoTimestamp), (ApiOptions.TimestampRecalculationInterval ?? ClientOptions.TimestampRecalculationInterval), _timeSyncState);

        /// <inheritdoc />
        public override TimeSpan? GetTimeOffset()
            => _timeSyncState.TimeOffset;

        /// <inheritdoc />
        public ICoinExRestClientSpotApiShared SharedClient => this;

    }
}
