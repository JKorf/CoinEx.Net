using CoinEx.Net.Objects;
using CryptoExchange.Net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json.Linq;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.Clients.SpotApi;
using CryptoExchange.Net.CommonObjects;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc cref="ICoinExClient" />
    public class CoinExClient : BaseRestClient, ICoinExClient
    {
        #region fields
        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<OrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client. Note that this does not trigger when using CancelAllOrdersAsync
        /// </summary>
        public event Action<OrderId>? OnOrderCanceled;
        #endregion

        #region Api clients
        /// <inheritdoc />
        public ICoinExClientSpotApi SpotApi { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExClient with default options
        /// </summary>
        public CoinExClient() : this(CoinExClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of CoinExClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public CoinExClient(CoinExClientOptions options) : base("CoinEx", options)
        {
            manualParseError = true;
            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;

            SpotApi = AddApiClient(new CoinExClientSpotApi(log, this, options));
        }
        #endregion

        #region methods
        #region public
        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(CoinExClientOptions options)
        {
            CoinExClientOptions.Default = options;
        }
        #endregion

        #region private

        /// <inheritdoc />
        protected override Task<ServerError?> TryParseErrorAsync(JToken data)
        {
            if (data["code"] != null && data["message"] != null)
            {
                if (data["code"]!.Value<int>() != 0)
                {
                    return Task.FromResult((ServerError?)ParseErrorResponse(data));
                }
            }

            return Task.FromResult((ServerError?)null);
        }

        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (error["code"] == null || error["message"] == null)
                return new ServerError(error.ToString());

            return new ServerError((int)error["code"]!, (string)error["message"]!);
        }

        internal async Task<WebCallResult<T>> Execute<T>(RestApiClient apiClient, Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<T>>(apiClient, uri, method, ct, parameters, signed).ConfigureAwait(false));
        }
        internal async Task<WebCallResult> Execute(RestApiClient apiClient, Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false)
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<object>>(apiClient, uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        internal async Task<WebCallResult<CoinExPagedResult<T>>> ExecutePaged<T>(RestApiClient apiClient, Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<CoinExPagedResult<T>>>(apiClient, uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        private static WebCallResult<T> GetResult<T>(WebCallResult<CoinExApiResult<T>> result) where T : class
        {
            if (result.Error != null || result.Data == null)
                return result.AsError<T>(result.Error ?? new UnknownError("No data received"));

            return result.As(result.Data.Data);
        }

        private static WebCallResult GetResult(WebCallResult<CoinExApiResult<object>> result)
        {
            if (result.Error != null || result.Data == null)
                return result.AsDatalessError(result.Error ?? new UnknownError("No data received"));

            return result.AsDataless();
        }

        internal void InvokeOrderPlaced(OrderId id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(OrderId id)
        {
            OnOrderCanceled?.Invoke(id);
        }

        #endregion
        #endregion
    }
}
