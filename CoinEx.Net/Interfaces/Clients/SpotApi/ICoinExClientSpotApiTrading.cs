using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System.Collections.Generic;

namespace CoinEx.Net.Interfaces.Clients.SpotApi
{
    /// <summary>
    /// CoinEx trading endpoints, placing and mananging orders.
    /// </summary>
    public interface ICoinExClientSpotApiTrading
    {
        /// <summary>
        /// Place a new order
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/put-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="accountType">Account type, Spot or Margin</param>
        /// <param name="side">Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="price">Price of the order</param>
        /// <param name="quantityAsset">The asset the quantity is in, for market orders van be the base or quote asset</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="hide">Hide the order</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder>> PlaceOrderAsync(
            string symbol,
            AccountType accountType,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal? price = null,
            string? quantityAsset = null,
            string? clientOrderId = null,
            bool? hide = null,
            CancellationToken ct = default);

        /// <summary>
        /// Place a new stop order
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/put-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="accountType">Account type, Spot or Margin</param>
        /// <param name="side">Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="price">Price of the order</param>
        /// <param name="quantityAsset">The asset the quantity is in, for market orders van be the base or quote asset</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="triggerPrice">Price to trigger on</param>
        /// <param name="hide">Hide the order</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopId>> PlaceStopOrderAsync(
            string symbol,
            AccountType accountType,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal triggerPrice,
            decimal? price = null,
            string? quantityAsset = null,
            string? clientOrderId = null,
            bool? hide = null,
            CancellationToken ct = default);

        /// <summary>
        /// Get an order by id
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/get-order-status" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="orderId">Order id</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder>> GetOrderAsync(string symbol, string orderId, CancellationToken ct = default);

        /// <summary>
        /// Get a list of open orders
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/list-pending-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPage<CoinExOrder>>> GetOpenOrdersAsync(string symbol, AccountType accountType, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);
    }
}
