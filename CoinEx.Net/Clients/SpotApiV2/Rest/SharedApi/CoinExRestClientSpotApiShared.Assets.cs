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
        #region Asset client

        public GetAssetOptions GetAssetOptions { get; } = new GetAssetOptions(_exchangeName, false);
        public async Task<HttpResult<SharedAsset>> GetAssetAsync(GetAssetRequest request, CancellationToken ct)
        {
            var validationError = GetAssetOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset>(Exchange, validationError);

            var asset = await _api.Account.GetDepositWithdrawalConfigAsync(request.Asset, ct: ct).ConfigureAwait(false);
            if (!asset.Success)
                return HttpResult.Fail<SharedAsset>(asset);

            return HttpResult.Ok(asset, new SharedAsset(asset.Data.Asset.Asset)
            {
                Networks = asset.Data.Networks.Select(x => new SharedAssetNetwork(x.Network)
                {
                    DepositEnabled = x.DepositEnabled,
                    WithdrawEnabled = x.WithdrawEnabled,
                    WithdrawFee = x.WithdrawalFee,
                    MinWithdrawQuantity = x.MinWithdrawQuantity,
                    MinConfirmations = x.SafeConfirmations
                }).ToArray()
            });
        }

        Task<HttpResult<SharedAsset[]>> IAssetsRestClient.GetAssetsAsync(GetAssetsRequest request, CancellationToken ct)
            => GetAllAssetsAsync(request, ct);
        GetAllAssetsOptions IAssetsRestClient.GetAssetsOptions => GetAllAssetsOptions;

        public GetAllAssetsOptions GetAllAssetsOptions { get; } = new GetAllAssetsOptions(_exchangeName, false);
        public async Task<HttpResult<SharedAsset[]>> GetAllAssetsAsync(GetAssetsRequest request, CancellationToken ct)
        {
            var validationError = GetAllAssetsOptions.ValidateRequest(request, this);
            if (validationError != null)
                return HttpResult.Fail<SharedAsset[]>(Exchange, validationError);

            var assets = _api.ExchangeData.GetAssetsAsync(ct: ct);
            var assetConfigs = _api.Account.GetAllDepositWithdrawalConfigsAsync(ct: ct);
            await Task.WhenAll(assets, assetConfigs).ConfigureAwait(false);
            if (!assets.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assets.Result);
            if (!assetConfigs.Result.Success)
                return HttpResult.Fail<SharedAsset[]>(assetConfigs.Result);

            return HttpResult.Ok(assets.Result, assets.Result.Data.Select(x =>
            {
                var config = assetConfigs.Result.Data.SingleOrDefault(y => y.Asset.Asset.Equals(x.ShortName));
                return new SharedAsset(x.ShortName)
                {
                    FullName = x.FullName,
                    Networks = config?.Networks.Select(x => new SharedAssetNetwork(x.Network)
                    {
                        DepositEnabled = x.DepositEnabled,
                        WithdrawEnabled = x.WithdrawEnabled,
                        WithdrawFee = x.WithdrawalFee,
                        MinWithdrawQuantity = x.MinWithdrawQuantity,
                        MinConfirmations = x.SafeConfirmations
                    }).ToArray() ?? []
                };
            }).ToArray());
        }

        #endregion
    }
}
