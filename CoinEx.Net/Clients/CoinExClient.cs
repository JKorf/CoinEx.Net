using CoinEx.Net.Objects;
using CryptoExchange.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.ExchangeInterfaces;
using Newtonsoft.Json.Linq;
using CryptoExchange.Net.Interfaces;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.Rest.Spot;
using CoinEx.Net.Objects.Internal;
using CoinEx.Net.Objects.Models;

namespace CoinEx.Net.Clients.Rest.Spot
{
    /// <summary>
    /// Client for the CoinEx REST API
    /// </summary>
    public class CoinExClient : RestClient, ICoinExClient
    {
        #region fields
        /// <summary>
        /// Event triggered when an order is placed via this client
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderPlaced;
        /// <summary>
        /// Event triggered when an order is canceled via this client. Note that this does not trigger when using CancelAllOrdersAsync
        /// </summary>
        public event Action<ICommonOrderId>? OnOrderCanceled;
        #endregion

        #region Subclients
        public ICoinExClientSpot SpotMarket { get; }
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
        public CoinExClient(CoinExClientOptions options): base("CoinEx", options)
        {
            manualParseError = true;
            ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;

            SpotMarket = new CoinExClientSpot(this, options);
        }
        #endregion

        #region methods
        #region public
        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
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

            return Task.FromResult((ServerError?) null);
        }

        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (error["code"] == null || error["message"] == null)
                return new ServerError(error.ToString());

            return new ServerError((int)error["code"]!, (string)error["message"]!);
        }

        internal async Task<WebCallResult<T>> Execute<T>(RestSubClient subClient, Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<T>>(subClient, uri, method, ct, parameters, signed).ConfigureAwait(false));
        }
        internal async Task<WebCallResult> Execute(RestSubClient subClient, Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) 
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<object>>(subClient, uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        internal async Task<WebCallResult<CoinExPagedResult<T>>> ExecutePaged<T>(RestSubClient subClient, Uri uri, HttpMethod method, CancellationToken ct, Dictionary<string, object>? parameters = null, bool signed = false) where T : class
        {
            return GetResult(await SendRequestAsync<CoinExApiResult<CoinExPagedResult<T>>>(subClient, uri, method, ct, parameters, signed).ConfigureAwait(false));
        }

        private static WebCallResult<T> GetResult<T>(WebCallResult<CoinExApiResult<T>> result) where T : class
        {
            if (result.Error != null || result.Data == null)
                return WebCallResult<T>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error ?? new UnknownError("No data received"));

            return result.As(result.Data.Data);
        }

        private static WebCallResult GetResult(WebCallResult<CoinExApiResult<object>> result) 
        {
            if (result.Error != null || result.Data == null)
                return WebCallResult.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error ?? new UnknownError("No data received"));

            return new WebCallResult(result.ResponseStatusCode, result.ResponseHeaders, null);
        }

        internal void InvokeOrderPlaced(ICommonOrderId id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(ICommonOrderId id)
        {
            OnOrderCanceled?.Invoke(id);
        }

        #endregion

        public override void Dispose()
        {
            SpotMarket.Dispose();
            base.Dispose();
        }
        #endregion
    }
}
