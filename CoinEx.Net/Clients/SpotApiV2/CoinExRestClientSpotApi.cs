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
    internal partial class CoinExRestClientSpotApi : RestApiClient<CoinExEnvironment, CoinExV2AuthenticationProvider, CoinExCredentials>, ICoinExRestClientSpotApi
    {
        #region fields
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
            base(logger, CoinExExchange.Metadata.Id, httpClient, options.Environment.RestBaseAddress, options, options.SpotOptions)
        {
            Account = new CoinExRestClientSpotApiAccount(this);
            ExchangeData = new CoinExRestClientSpotApiExchangeData(this);
            Trading = new CoinExRestClientSpotApiTrading(this);

            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;
        }
        #endregion

        /// <inheritdoc />
        protected override IMessageSerializer CreateSerializer() => new SystemTextJsonMessageSerializer(SerializerOptions.WithConverters(CoinExExchange._serializerContext));

        /// <inheritdoc />
        protected override CoinExV2AuthenticationProvider CreateAuthenticationProvider(CoinExCredentials credentials)
            => new CoinExV2AuthenticationProvider(credentials);

        /// <inheritdoc />
        public override string FormatSymbol(string baseAsset, string quoteAsset, TradingMode tradingMode, DateTime? deliverTime = null)
                => CoinExExchange.FormatSymbol(baseAsset, quoteAsset, tradingMode, deliverTime);

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

        internal async Task<HttpResult<T>> SendAsync<T>(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null)
        {
            var result = await base.SendAsync<CoinExApiResult<T>>(definition, parameters, cancellationToken, null, weight).ConfigureAwait(false);
            if (!result.Success)
                return HttpResult.Fail<T>(result);

            if (result.Data.Code != 0)
                return HttpResult.Fail<T>(result, new ServerError(result.Data.Code, GetErrorInfo(result.Data.Code, result.Data.Message!)));

            return HttpResult.Ok(result, result.Data.Data);
        }

        internal async Task<HttpResult<CoinExPaginated<T>>> SendPaginatedAsync<T>(RequestDefinition definition, Parameters? parameters, CancellationToken cancellationToken, int? weight = null)
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

        /// <inheritdoc />
        public ICoinExRestClientSpotApiShared SharedClient => this;

    }
}
