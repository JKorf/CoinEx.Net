using CoinEx.Net.Clients.FuturesApi;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients.SpotApiV2;
using CoinEx.Net.Objects.Models.V2;
using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using CryptoExchange.Net.SharedApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.SpotApiV2
{
    internal partial class CoinExRestClientSpotSharedApi
    {
        #region Spot Symbol client

        public SharedSymbolCatalog? SpotSymbolCatalog => ExchangeSymbolCache.GetSymbolCatalog(_exchangeName, _topicId, _api.EnvironmentName, null);
        public GetSpotSymbolsOptions GetSpotSymbolsOptions { get; } = new GetSpotSymbolsOptions(_exchangeName, false);
        public async Task<HttpResult<SharedSpotSymbol[]>> GetSpotSymbolsAsync(GetSymbolsRequest request, CancellationToken ct)
        {
            var validationError = GetSpotSymbolsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedSpotSymbol[]>(Exchange, validationError);

            var symbolsTask = _api.ExchangeData.GetSymbolsAsync(ct: ct);
            var assetsTask = _api.ExchangeData.GetAssetsAsync(ct: ct);
            await Task.WhenAll(symbolsTask, assetsTask).ConfigureAwait(false);
            var symbolsResult = symbolsTask.Result;
            var assetsResult = assetsTask.Result;
            if (!symbolsResult.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(symbolsResult);
            if (!assetsResult.Success)
                return HttpResult.Fail<SharedSpotSymbol[]>(assetsResult);

            var data = symbolsResult.Data
                .Select(x => ParseSymbol(x, assetsResult.Data))
                .ToArray();

            ExchangeSymbolCache.UpdateSymbolInfo(_topicId, _api.EnvironmentName, null, data);
            return HttpResult.Ok(symbolsResult, SharedUtils.ApplySymbolFilter(data, request));
        }

        private SharedSpotSymbol ParseSymbol(CoinExSymbol s, CoinExAsset[] assets)
        {
            var result = new SharedSpotSymbol(s.BaseAsset, s.QuoteAsset, s.Name, true)
            {
                MinTradeQuantity = s.MinOrderQuantity,
                PriceDecimals = s.PricePrecision,
                QuantityDecimals = s.QuantityPrecision,
                DisplayName = s.Name,
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

            result.QuoteAssetType = SharedAssetType.Crypto;
            if (LibraryHelpers.IsStableCoin(result.QuoteAsset))
                result.QuoteAssetSubType = SharedAssetSubType.StableCoin;

            return result;
        }

        public async Task<ExchangeCallResult<SharedSymbol[]>> GetSpotSymbolsForBaseAssetAsync(string baseAsset)
        {
            if (!ExchangeSymbolCache.HasCached(_topicId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<SharedSymbol[]>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<SharedSymbol[]>.Ok(Exchange, ExchangeSymbolCache.GetSymbolsForBaseAsset(_topicId, _api.EnvironmentName, null, baseAsset));
        }

        public async Task<ExchangeCallResult<bool>> SupportsSpotSymbolAsync(SharedSymbol symbol)
        {
            if (symbol.TradingMode != TradingMode.Spot)
                throw new ArgumentException(nameof(symbol), "Only Spot symbols allowed");

            if (!ExchangeSymbolCache.HasCached(_topicId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicId, _api.EnvironmentName, null, symbol));
        }

        public async Task<ExchangeCallResult<bool>> SupportsSpotSymbolAsync(string symbolName)
        {
            if (!ExchangeSymbolCache.HasCached(_topicId, _api.EnvironmentName, null))
            {
                var symbols = await GetSpotSymbolsAsync(new GetSymbolsRequest(), default).ConfigureAwait(false);
                if (!symbols.Success)
                    return ExchangeCallResult<bool>.Fail(Exchange, symbols.Error!);
            }

            return ExchangeCallResult<bool>.Ok(Exchange, ExchangeSymbolCache.SupportsSymbol(_topicId, _api.EnvironmentName, null, symbolName));
        }
        #endregion
    }
}
