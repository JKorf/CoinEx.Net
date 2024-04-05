﻿using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV2
{
    /// <summary>
    /// CoinEx trading endpoints, placing and mananging orders.
    /// </summary>
    public interface ICoinExRestClientSpotApiTrading
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
        Task<WebCallResult<CoinExOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Get a list of open orders
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/list-pending-order" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExOrder>>> GetOpenOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of closed orders. Note that orders canceled without having any trades will not be returned
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/list-finished-order" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExOrder>>> GetClosedOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of open stop orders
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/list-pending-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of closed stop orders. Note that orders canceled without having any trades will not be returned
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/list-finished-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Edit an active order
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/edit-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="orderId">Order id</param>
        /// <param name="quantity">New quantity</param>
        /// <param name="price">New price</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder>> EditOrderAsync(
            string symbol,
            AccountType accountType,
            long orderId,
            decimal quantity,
            decimal? price = null,
            CancellationToken ct = default);

        /// <summary>
        /// Edit an active stop order
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/edit-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="stopOrderId">Order id</param>
        /// <param name="quantity">New quantity</param>
        /// <param name="price">New price</param>
        /// <param name="triggerPrice">New trigger price</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopId>> EditStopOrderAsync(
            string symbol,
            AccountType accountType,
            long stopOrderId,
            decimal quantity,
            decimal triggerPrice,
            decimal? price = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel all orders for a symbol
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-all-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="side">Only cancel a specific order side</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult> CancelAllOrdersAsync(string symbol, AccountType accountType, OrderSide? side = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active order
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="orderId">Id of order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, AccountType accountType, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active stop order
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="stopOrderId">Id of stop order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopOrder>> CancelStopOrderAsync(string symbol, AccountType accountType, long stopOrderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active order by its client order id
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-order-by-client-id" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="clientOrderId">Client order id of order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder>> CancelOrderByClientOrderIdAsync(string symbol, AccountType accountType, string clientOrderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active stop order by its client order id
        /// <para><a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-stop-order-by-client-id" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="clientStopOrderId">Client order id of stop order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopOrder>> CancelStopOrderByClientOrderIdAsync(string symbol, AccountType accountType, string clientStopOrderId, CancellationToken ct = default);

        /// <summary>
        /// Get trade list
        /// <para><a href="https://docs.coinex.com/api/v2/spot/deal/http/list-user-deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="side">Filter by side</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetUserTradesAsync(string symbol, AccountType accountType, OrderSide? side = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get trades for a specific order
        /// <para><a href="https://docs.coinex.com/api/v2/spot/deal/http/list-user-order-deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="accountType">Account type</param>
        /// <param name="orderId">The order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetOrderTradesAsync(string symbol, AccountType accountType, long orderId, int? page = null, int? pageSize = null, CancellationToken ct = default);
    }
}
