using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models.V2;
using System;
using System.Collections.Generic;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV2
{
    /// <summary>
    /// CoinEx trading endpoints, placing and mananging orders.
    /// </summary>
    public interface ICoinExRestClientSpotApiTrading
    {
        /// <summary>
        /// Place a new order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/put-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type, Spot or Margin</param>
        /// <param name="side">["<c>side</c>"] Order side</param>
        /// <param name="type">["<c>type</c>"] Order type</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity</param>
        /// <param name="price">["<c>price</c>"] Price of the order</param>
        /// <param name="quantityAsset">["<c>ccy</c>"] The asset the quantity is in, for market orders van be the base or quote asset</param>
        /// <param name="clientOrderId">["<c>client_id</c>"] Client order id</param>
        /// <param name="hide">["<c>is_hide</c>"] Hide the order</param>
        /// <param name="stpMode">["<c>stp_mode</c>"] Self trade prevention mode</param>
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
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default);

        /// <summary>
        /// Place a new stop order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/put-stop-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/stop-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] The symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type, Spot or Margin</param>
        /// <param name="side">["<c>side</c>"] Order side</param>
        /// <param name="type">["<c>type</c>"] Order type</param>
        /// <param name="quantity">["<c>amount</c>"] Quantity</param>
        /// <param name="price">["<c>price</c>"] Price of the order</param>
        /// <param name="quantityAsset">["<c>ccy</c>"] The asset the quantity is in, for market orders van be the base or quote asset</param>
        /// <param name="clientOrderId">["<c>client_id</c>"] Client order id</param>
        /// <param name="triggerPrice">["<c>trigger_price</c>"] Price to trigger on</param>
        /// <param name="hide">["<c>is_hide</c>"] Hide the order</param>
        /// <param name="stpMode">["<c>stp_mode</c>"] Self trade prevention mode</param>
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
            SelfTradePreventionMode? stpMode = null,
            CancellationToken ct = default);

        /// <summary>
        /// Place multiple orders in a single call
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/put-multi-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/batch-order
        /// </para>
        /// </summary>
        /// <param name="requests">["<c>orders</c>"] The orders to place</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CallResult<CoinExBatchOrderResult>[]>> PlaceMultipleOrdersAsync(
            IEnumerable<CoinExPlaceOrderRequest> requests,
            CancellationToken ct = default);

        /// <summary>
        /// Place multiple stop orders in a single call
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/put-multi-stop-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/batch-stop-order
        /// </para>
        /// </summary>
        /// <param name="requests">["<c>orders</c>"] The stop orders to place</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CallResult<CoinExStopId>[]>> PlaceMultipleStopOrdersAsync(
           IEnumerable<CoinExPlaceStopOrderRequest> requests,
           CancellationToken ct = default);

        /// <summary>
        /// Get an order by id
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/get-order-status" /><br />
        /// Endpoint:<br />
        /// GET /v2/spot/order-status
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="orderId">["<c>order_id</c>"] Order id</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Get a list of open orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/list-pending-order" /><br />
        /// Endpoint:<br />
        /// GET /v2/spot/pending-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="side">["<c>side</c>"] Filter by side</param>
        /// <param name="clientOrderId">["<c>client_id</c>"] Filter by client order id</param>
        /// <param name="page">["<c>page</c>"] Page number</param>
        /// <param name="pageSize">["<c>limit</c>"] Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExOrder>>> GetOpenOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of closed orders. Note that orders canceled without having any trades will not be returned
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/list-finished-order" /><br />
        /// Endpoint:<br />
        /// GET /v2/spot/finished-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="side">["<c>side</c>"] Filter by side</param>
        /// <param name="clientOrderId">["<c>client_id</c>"] Filter by client order id</param>
        /// <param name="page">["<c>page</c>"] Page number</param>
        /// <param name="pageSize">["<c>limit</c>"] Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExOrder>>> GetClosedOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of open stop orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/list-pending-stop-order" /><br />
        /// Endpoint:<br />
        /// GET /v2/spot/pending-stop-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="side">["<c>side</c>"] Filter by side</param>
        /// <param name="clientOrderId">["<c>client_id</c>"] Filter by client order id</param>
        /// <param name="page">["<c>page</c>"] Page number</param>
        /// <param name="pageSize">["<c>limit</c>"] Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetOpenStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, string? clientOrderId = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get a list of closed stop orders. Note that orders canceled without having any trades will not be returned
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/list-finished-stop-order" /><br />
        /// Endpoint:<br />
        /// GET /v2/spot/finished-stop-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Filter by symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="side">["<c>side</c>"] Filter by side</param>
        /// <param name="page">["<c>page</c>"] Page number</param>
        /// <param name="pageSize">["<c>limit</c>"] Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExStopOrder>>> GetClosedStopOrdersAsync(AccountType accountType, string? symbol = null, OrderSide? side = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Edit an active order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/edit-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/modify-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="orderId">["<c>order_id</c>"] Order id</param>
        /// <param name="quantity">["<c>amount</c>"] New quantity</param>
        /// <param name="price">["<c>price</c>"] New price</param>
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
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/edit-stop-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/modify-stop-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="stopOrderId">["<c>stop_id</c>"] Order id</param>
        /// <param name="quantity">["<c>amount</c>"] New quantity</param>
        /// <param name="price">["<c>price</c>"] New price</param>
        /// <param name="triggerPrice">["<c>trigger_price</c>"] New trigger price</param>
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
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-all-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/cancel-all-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="side">["<c>side</c>"] Only cancel a specific order side</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult> CancelAllOrdersAsync(string symbol, AccountType accountType, OrderSide? side = null, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/cancel-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="orderId">["<c>order_id</c>"] Id of order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, AccountType accountType, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel multiple orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-batch-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/cancel-batch-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="orderIds">["<c>order_ids</c>"] Ids of orders to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExBatchResult<CoinExOrder>[]>> CancelOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active stop order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-stop-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/cancel-stop-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="stopOrderId">["<c>stop_id</c>"] Id of stop order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopOrder>> CancelStopOrderAsync(string symbol, AccountType accountType, long stopOrderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active order by its client order id
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-order-by-client-id" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/cancel-order-by-client-id
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="clientOrderId">["<c>client_id</c>"] Client order id of order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExOrder[]>> CancelOrdersByClientOrderIdAsync(string symbol, AccountType accountType, string clientOrderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel an active stop order by its client order id
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-stop-order-by-client-id" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/cancel-stop-order-by-client-id
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="clientStopOrderId">["<c>client_id</c>"] Client order id of stop order to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExStopOrder[]>> CancelStopOrdersByClientOrderIdAsync(string symbol, AccountType accountType, string clientStopOrderId, CancellationToken ct = default);

        /// <summary>
        /// Cancel multiple stop orders
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/order/http/cancel-batch-stop-order" /><br />
        /// Endpoint:<br />
        /// POST /v2/spot/cancel-batch-stop-order
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="orderIds">["<c>stop_ids</c>"] Stop order ids to cancel</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExBatchResult<CoinExStopOrder>[]>> CancelStopOrdersAsync(string symbol, IEnumerable<long> orderIds, CancellationToken ct = default);

        /// <summary>
        /// Get trade list
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/deal/http/list-user-deals" /><br />
        /// Endpoint:<br />
        /// GET /v2/spot/user-deals
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="side">["<c>side</c>"] Filter by side</param>
        /// <param name="startTime">["<c>start_time</c>"] Filter by start time</param>
        /// <param name="endTime">["<c>end_Time</c>"] Filter by end time</param>
        /// <param name="page">["<c>page</c>"] Page number</param>
        /// <param name="pageSize">["<c>limit</c>"] Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetUserTradesAsync(string symbol, AccountType accountType, OrderSide? side = null, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? pageSize = null, CancellationToken ct = default);

        /// <summary>
        /// Get trades for a specific order
        /// <para>
        /// Docs:<br />
        /// <a href="https://docs.coinex.com/api/v2/spot/deal/http/list-user-order-deals" /><br />
        /// Endpoint:<br />
        /// GET /v2/spot/order-deals
        /// </para>
        /// </summary>
        /// <param name="symbol">["<c>market</c>"] Symbol, for example `ETHUSDT`</param>
        /// <param name="accountType">["<c>market_type</c>"] Account type</param>
        /// <param name="orderId">["<c>order_id</c>"] The order id</param>
        /// <param name="page">["<c>page</c>"] Page number</param>
        /// <param name="pageSize">["<c>limit</c>"] Page size</param>
        /// <param name="ct">Cancelation Token</param>
        /// <returns></returns>
        Task<WebCallResult<CoinExPaginated<CoinExUserTrade>>> GetOrderTradesAsync(string symbol, AccountType accountType, long orderId, int? page = null, int? pageSize = null, CancellationToken ct = default);
    }
}
