using CryptoExchange.Net.Objects;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.FuturesApi;
using CryptoExchange.Net;
using CoinEx.Net.Objects.Models.V2;
using System.Drawing;
using CryptoExchange.Net.Objects.Errors;

namespace CoinEx.Net.Clients.FuturesApi
{
    internal partial class CoinExRestClientFuturesSharedApi
    {

        #region Get Futures Symbols

        async Task<ICallResult<SharedFuturesSymbol[]>> IGetFuturesSymbols.GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
            => await GetFuturesSymbolsAsync(request, ct).ConfigureAwait(false);

        public SharedSymbolCatalog? FuturesSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_exchangeName, _topicId, _api.EnvironmentName, null);
        public GetFuturesSymbolsOptions GetFuturesSymbolsOptions { get; } = new GetFuturesSymbolsOptions(_exchangeName, false);
        public async Task<HttpResult<SharedFuturesSymbol[]>> GetFuturesSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = GetFuturesSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedFuturesSymbol[]>(Exchange, validationError);

            var symbolsTask = _api.ExchangeData.GetSymbolsAsync(ct: ct);
            var assetsTask = _api._baseClient.SpotApiV2.ExchangeData.GetAssetsAsync(ct: ct);
            await Task.WhenAll(symbolsTask, assetsTask).ConfigureAwait(false);
            var symbolsResult = symbolsTask.Result;
            var assetsResult = assetsTask.Result;
            if (!symbolsResult.Success)
                return HttpResult.Fail<SharedFuturesSymbol[]>(symbolsResult);
            if (!assetsResult.Success)
                return HttpResult.Fail<SharedFuturesSymbol[]>(assetsResult);

            var data = symbolsResult.Data
                .Select(x => ParseSymbol(x, assetsResult.Data))
                .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicId, _api.EnvironmentName, null, data);
            return HttpResult.Ok(symbolsResult, SharedUtils.ApplySymbolFilter(data, request));
        }

        #endregion

        private SharedFuturesSymbol ParseSymbol(CoinExFuturesSymbol s, CoinExAsset[] assets)
        {
            var result = new SharedFuturesSymbol(
                s.ContractType == ContractType.Inverse ? TradingMode.PerpetualInverse : TradingMode.PerpetualLinear,
                s.BaseAsset, s.QuoteAsset, s.Symbol, s.TradingAvailable)
            {
                MinTradeQuantity = s.MinOrderQuantity,
                QuantityDecimals = s.QuantityPrecision,
                PriceDecimals = s.PricePrecision,
                ContractSize = 1,
                DisplayName = s.Symbol,
                QuoteAssetType = SharedAssetType.Crypto,
                QuoteAssetSubType = SharedAssetSubType.StableCoin,
                MakerFeePercentage = s.MakerFeeRate * 100,
                TakerFeePercentage = s.TakerFeeRate * 100
            };

            if (LibraryHelpers.IsCommodity(result.BaseAsset))
            {
                result.BaseAssetType = SharedAssetType.TradFi;
                result.BaseAssetSubType = SharedAssetSubType.Commodity;
            }
            else
            {
                var baseAsset = assets.SingleOrDefault(x => x.ShortName == s.BaseAsset);
                if (baseAsset != null)
                {
                    if (baseAsset.FullName.Contains("xStock"))
                    {
                        result.BaseAssetType = SharedAssetType.TradFi;
                        result.BaseAssetSubType = SharedAssetSubType.Equity;
                    }
                    else
                    {
                        result.BaseAssetType = SharedAssetType.Crypto;
                        if (LibraryHelpers.IsStableCoin(result.BaseAsset))
                            result.BaseAssetSubType = SharedAssetSubType.StableCoin;
                    }
                }
            }

            return result;
        }

        public async Task<ExchangeCallResult<SharedSymbol[]>> GetFuturesSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicId, _api.EnvironmentName, null, baseAsset));
        }

        public async Task<ExchangeCallResult<bool>> SupportsFuturesSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode == TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Spot symbols not allowed");

            if (!ExchangeSymbolCache.HasCached(_topicId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicId, _api.EnvironmentName, null, symbol));
        }

        public async Task<ExchangeCallResult<bool>> SupportsFuturesSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicId, _api.EnvironmentName, null))
            {
                var symbols = await GetFuturesSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicId, _api.EnvironmentName, null, symbolName));
        }
    }
}
