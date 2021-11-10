using CoinEx.Net.Enums;
using CoinEx.Net.Objects;
using CryptoExchange.Net.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoinEx.Net.Interfaces.Clients.Rest.Spot
{
    public interface ICoinExClientSpotTrading
    {
        /// <summary>
        /// Places an order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="side">Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">The quantity of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="orderOption">Option for the order</param>
        /// <param name="stopPrice">The stop-price of a single unit of the order</param>
        /// <param name="immediateOrCancel">True if the order should be filled immediately up on placing, otherwise it will be canceled</param>
        /// <param name="clientId">Client id which can be used to match the order</param>
        /// <param name="sourceId">User defined number</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        Task<WebCallResult<CoinExOrder>> PlaceOrderAsync(
            string symbol,
            OrderType type,
            OrderSide side,
            decimal quantity,

            decimal? price = null,
            decimal? stopPrice = null,
            bool? immediateOrCancel = null,
            OrderOption? orderOption = null,
            string? clientId = null,
            string? sourceId = null,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of open orders for a symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of open stop orders for a symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenStopOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of executed orders for a symbol in the last 2 days. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetClosedOrdersAsync(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        Task<WebCallResult<CoinExOrder>> GetOrderAsync(long orderId, string symbol, CancellationToken ct = default);

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrderTrade>>> GetOrderTradesAsync(long orderId, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of trades you executed on a specific symbol. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrderTradeExtended>>> GetUserTradesAsync(string symbol, int page, int limit, CancellationToken ct = default);

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the order is on</param>
        /// <param name="orderId">The id of the order to cancel</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the canceled order</returns>
        Task<WebCallResult<CoinExOrder>> CancelOrderAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Cancels all stop orders. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the orders are on</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Execution statut</returns>
        Task<WebCallResult> CancelAllStopOrdersAsync(string symbol, CancellationToken ct = default);

        /// <summary>
        /// Cancels all orders. Requires API credentials
        /// </summary>
        /// <param name="symbol">The symbol the orders are on</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Execution statut</returns>
        Task<WebCallResult> CancelAllOrdersAsync(string symbol, CancellationToken ct = default);
    }
}
