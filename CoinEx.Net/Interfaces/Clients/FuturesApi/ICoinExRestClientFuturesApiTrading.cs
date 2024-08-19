using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Interfaces.Clients.FuturesApi
{
    /// <summary>
    /// CoinEx trading endpoints, placing and mananging orders.
    /// </summary>
    public interface ICoinExRestClientFuturesApiTrading
    {
        /// <summary>
        /// Place a new order
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/put-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETHUSDT`</param>
        /// <param name="side">Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="price">Price of the order</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="hide">Hide the order</param>
        /// <param name="stpMode">Self trade prevention mode</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExFuturesOrder>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal? price = null,
            string? clientOrderId = null,
            bool? hide = null,
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default);

        /// <summary>
        /// Place a new stop order
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/put-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol, for example `ETHUSDT`</param>
        /// <param name="side">Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="price">Price of the order</param>
        /// <param name="triggerPriceType">Price type for the trigger</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="triggerPrice">Price to trigger on</param>
        /// <param name="hide">Hide the order</param>
        /// <param name="stpMode">Self trade prevention mode</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopId>> PlaceStopOrderAsync(
            string symbol,
            OrderSide side,
            OrderTypeV2 type,
            decimal quantity,
            decimal triggerPrice,
            TriggerPriceType triggerPriceType,
            decimal? price = null,
            string? clientOrderId = null,
            bool? hide = null,
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default);

        /// <summary>
        /// Place multiple orders in a single call
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/put-multi-order" /></para>
        /// </summary>
        /// <param name="requests">Orders to place</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExFuturesOrder>>>> PlaceMultipleOrdersAsync(
            IEnumerable<CoinExFuturesPlaceOrderRequest> requests,
            CancellationToken ct = default);

        /// <summary>
        /// Place multiple stop orders in a single call
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/put-multi-stop-order" /></para>
        /// </summary>
        /// <param name="requests">Stop orders to place</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExStopId>>>> PlaceMultipleStopOrdersAsync(
            IEnumerable<CoinExFuturesPlaceStopOrderRequest> requests,
            CancellationToken ct = default);

        /// <summary>
        /// Get an order by id
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/get-order-status" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="orderId">Order id</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExFuturesOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default);


        /// <summary>
        /// Get a list of open orders
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/list-pending-order" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExFuturesOrder>>> GetOpenOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of closed orders. Note that orders canceled without having any trades will not be returned
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/list-finished-order" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExFuturesOrder>>> GetClosedOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of open stop orders
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/list-pending-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of closed stop orders. Note that orders canceled without having any trades will not be returned
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/list-finished-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="side">Filter by side</param>
        /// <param name="clientOrderId">Filter by client order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Edit an active order
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/edit-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="orderId">Order id</param>
        /// <param name="quantity">New quantity</param>
        /// <param name="price">New price</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExFuturesOrder>> EditOrderAsync(
            string symbol,
            long orderId,
            decimal quantity,
            decimal? price = null,
            CancellationToken ct = default);

        /// <summary>
        /// Edit an active stop order
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/edit-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="stopOrderId">Order id</param>
        /// <param name="quantity">New quantity</param>
        /// <param name="price">New price</param>
        /// <param name="triggerPrice">New trigger price</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopId>> EditStopOrderAsync(
            string symbol,
            long stopOrderId,
            decimal quantity,
            decimal triggerPrice,
            decimal? price = null,
            CancellationToken ct = default);

        /// <summary>
        /// Cancel all orders for a symbol
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/cancel-all-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="side">Only cancel a specific order side</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult> CancelAllOrdersAsync(string symbol, OrderSide? side = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active order
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/cancel-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="orderId">Id of order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExFuturesOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active stop order
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/cancel-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="stopOrderId">Id of stop order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopOrder>> CancelStopOrderAsync(string symbol, long stopOrderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active order by its client order id
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/cancel-order-by-client-id" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="clientOrderId">Client order id of order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExFuturesOrder>> CancelOrderByClientOrderIdAsync(string symbol, string clientOrderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active stop order by its client order id
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/cancel-stop-order-by-client-id" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="clientStopOrderId">Client order id of stop order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopOrder>> CancelStopOrderByClientOrderIdAsync(string symbol, string clientStopOrderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel multiple orders
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/cancel-batch-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="orderIds">Ids of orders to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExFuturesOrder>>>> CancelOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default);

        /// <summary>
        /// Cancel multiple stop orders
        /// <para><a href="https://docs.coinex.com/api/v2/futures/order/http/cancel-batch-stop-order" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="orderIds">Ids of stop orders to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<IEnumerable<CoinExBatchResult<CoinExStopOrder>>>> CancelStopOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default);

        /// <summary>
        /// Get trade list
        /// <para><a href="https://docs.coinex.com/api/v2/futures/deal/http/list-user-deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="side">Filter by side</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetUserTradesAsync(string symbol, OrderSide? side = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get trades for a specific order
        /// <para><a href="https://docs.coinex.com/api/v2/futures/deal/http/list-user-order-deals" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="orderId">The order id</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetOrderTradesAsync(string symbol, long orderId, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get user positions
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/list-pending-position" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExPosition>>> GetPositionsAsync(string? symbol = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get position history
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/list-finished-position" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExPosition>>> GetPositionHistoryAsync(string? symbol = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// (partially) Close an open position
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/close-position" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="orderType">Order type to use</param>
        /// <param name="price">Price of the order</param>
        /// <param name="quantity">Quantity to close</param>
        /// <param name="clientOrderId">Client order id</param>
        /// <param name="hidden">Is hidden</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExFuturesOrder>> ClosePositionAsync(string symbol, OrderTypeV2 orderType, decimal? price = null, decimal? quantity = null, string? clientOrderId = null, bool? hidden = null, CancellationToken ct = default);

        /// <summary>
        /// Adjust the margin for a position. Positive quantity for increasing, negative quantity for decreasing
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/close-position" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="quantity">Quantity to increase (positive number) or decrease (negative number)</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPosition>> AdjustPositionMarginAsync(string symbol, decimal quantity, CancellationToken ct = default);

        /// <summary>
        /// Set stop loss for a position
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/set-position-stop-loss" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="stopLossType">Stop loss price type</param>
        /// <param name="stopLossPrice">Stop loss price</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPosition>> SetStopLossAsync(string symbol, PriceType stopLossType, decimal stopLossPrice, CancellationToken ct = default);

        /// <summary>
        /// Set take profit for a position
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/set-position-take-profit" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="takeProfitType">Take profit price type</param>
        /// <param name="takeProfitPrice">Take profit price</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPosition>> SetTakeProfitAsync(string symbol, PriceType takeProfitType, decimal takeProfitPrice, CancellationToken ct = default);

        /// <summary>
        /// Get margin adjustment history
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/set-position-take-profit" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="positionId">Position id</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExPositionMargin>>> GetMarginHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get funding rate history
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/list-position-funding-history" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="positionId">Position id</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExPositionFundingRate>>> GetFundingRateHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get auto deleveraging history
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/list-positiing-adl-history" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="positionId">Position id</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExPositionAdl>>> GetAutoDeleverageHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get position settlement history
        /// <para><a href="https://docs.coinex.com/api/v2/futures/position/http/list-position-settle-history" /></para>
        /// </summary>
        /// <param name="symbol">Symbol, for example `ETHUSDT`</param>
        /// <param name="positionId">Position id</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExPositionSettlement>>> GetAutoSettlementHistoryAsync(string symbol, long positionId, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);
    }
}
