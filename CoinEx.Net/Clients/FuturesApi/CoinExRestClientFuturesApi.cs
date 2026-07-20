using CoinEx.Net.Clients.MessageHandlers;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.FuturesApi
{
    /// <inheritdoc cref="ICoinExRestClientFuturesApi" />
    internal partial class CoinExRestClientFuturesApi : RestApiClient<CoinExEnvironment, CoinExV2AuthenticationProvider, CoinExCredentials>, ICoinExRestClientFuturesApi
    {
        #region fields
        /// <inheritdoc />
        public new CoinExRestOptions ClientOptions => (CoinExRestOptions)base.ClientOptions;

        protected override IRestMessageHandler MessageHandler { get; } = new CoinExRestMessageHandler(CoinExErrors.RestErrorMapping);
        protected override ErrorMapping ErrorMapping => CoinExErrors.RestErrorMapping;

        internal CoinExRestClient _baseClient;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public ICoinExRestClientFuturesApiAccount Account { get; }
        /// <inheritdoc />
        public ICoinExRestClientFuturesApiExchangeData ExchangeData { get; }
        /// <inheritdoc />
        public ICoinExRestClientFuturesApiTrading Trading { get; }
        #endregion

        #region ctor
        internal CoinExRestClientFuturesApi(CoinExRestClient baseClient, ILoggerFactory? loggerFactory, HttpClient? httpClient, CoinExRestOptions options) :
            base(loggerFactory, CoinExExchange.Metadata.Id, httpClient, options.Environment.RestBaseAddress, options, options.FuturesOptions)
        {
            _baseClient = baseClient;

            Account = new CoinExRestClientFuturesApiAccount(this);
            ExchangeData = new CoinExRestClientFuturesApiExchangeData(this);
            Trading = new CoinExRestClientFuturesApiTrading(this);

            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;

        }
        #endregion

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => CoinExExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(CoinExExchange._serializerContext));

        public ICoinExRestClientFuturesApiShared SharedClient => this;

        /// <inheritdoc />
        protected override CoinExV2AuthenticationProvider CreateAuthenticationProvider(CoinExCredentials credentials)
            => new CoinExV2AuthenticationProvider(credentials);


        #region methods

        internal async Task<HttpResult> SendAsync(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null)
        {
            var result = await base.SendAsync<CoinExApiResult>(definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            if (!result.Success)
                return result;

            if (result.Data.Code != 0)
                return HttpResult.Fail(result, new ServerError(result.Data.Code, GetErrorInfo(result.Data.Code, result.Data.Message!)));

            return result;
        }

        internal async Task<HttpResult<T>> SendAsync<T>(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null) where T : class
        {
            var result = await base.SendAsync<CoinExApiResult<T>>(definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<T>(result);

            if (result.Data.Code != 0)
                return HttpResult.Fail<T>(result, new ServerError(result.Data.Code, GetErrorInfo(result.Data.Code, result.Data.Message!)));

            return HttpResult.Ok(result, result.Data.Data);
        }

        internal async Task<HttpResult<CoinExPaginated<T>>> SendPaginatedAsync<T>(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null) where T : class
        {
            var result = await base.SendAsync<CoinExPageApiResult<T[]>>(definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<CoinExPaginated<T>>(result);

            if (result.Data.Code != 0)
                return HttpResult.Fail<CoinExPaginated<T>>(result, new ServerError(result.Data.Code, GetErrorInfo(result.Data.Code, result.Data.Message!)));

            var resultPage = new CoinExPaginated<T>
            {
                HasNext = result.Data.Pagination.HasNext,
                Total = result.Data.Pagination.Total,
                Items = result.Data.Data
            };

            return HttpResult.Ok(result, resultPage);
        }
        #endregion

        /// <inheritdoc />
        protected override async Task<HttpResult<DateTime>> GetServerTimestampAsync() => await ExchangeData.GetServerTimeAsync().ConfigureAwait(false);

    }
}
