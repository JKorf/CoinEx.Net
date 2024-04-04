using CoinEx.Net.Enums;
using CryptoExchange.Net.Objects;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net.Objects.Models;

namespace CoinEx.Net.Interfaces.Clients.SpotApiV1
{
    /// <summary>
    /// CoinEx trading endpoints, placing and mananging orders.
    /// </summary>
    public interface ICoinExRestClientSpotApiTrading
    {
        /// <summary>
        /// Places an order. This is a single method for multiple place order endpoints. The called endpoint depends on the provided order type.
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/031limit_order" /></para>
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/032market_order" /></para>
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/03111stop_limit_order" /></para>
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/033IOC_order" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to place the order for</param>
        /// <param name="side">Order side</param>
        /// <param name="type">Order type</param>
        /// <param name="quantity">The quantity of the order</param>
        /// <param name="price">The price of a single unit of the order</param>
        /// <param name="orderOption">Option for the order</param>
        /// <param name="stopPrice">The stop-price of a single unit of the order</param>
        /// <param name="immediateOrCancel">True if the order should be filled immediately up on placing, otherwise it will be canceled</param>
        /// <param name="clientOrderId">Client id which can be used to match the order</param>
        /// <param name="sourceId">User defined number</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order that was placed</returns>
        Task<WebCallResult<CoinExOrder>> PlaceOrderAsync(
            string symbol,
            OrderSide side,
            OrderType type,
            decimal quantity,

            decimal? price = null,
            decimal? stopPrice = null,
            bool? immediateOrCancel = null,
            OrderOption? orderOption = null,
            string? clientOrderId = null,
            string? sourceId = null,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of open orders for a symbol. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/034pending" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenOrdersAsync(string? symbol = null, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of open stop orders for a symbol. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/041stop_pending" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of open orders for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetOpenStopOrdersAsync(string symbol, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a list of executed orders for a symbol in the last 2 days. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/036finished" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve the open orders for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of executed orders for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrder>>> GetClosedOrdersAsync(string symbol, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves details of an order. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/037order_status" /></para>
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <param name="symbol">The symbol the order is for</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of the order</returns>
        Task<WebCallResult<CoinExOrder>> GetOrderAsync(string symbol, long orderId, CancellationToken ct = default);

        /// <summary>
        /// Retrieves execution details of a specific order. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/0311order_deals" /></para>
        /// </summary>
        /// <param name="orderId">The id of the order</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Details of an executed order</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrderTrade>>> GetOrderTradesAsync(long orderId, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Gets a list of trades you executed on a specific symbol. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/039user_deals" /></para>
        /// </summary>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="page">The page of the resulting list</param>
        /// <param name="limit">The number of results per page</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>List of trades for a symbol</returns>
        Task<WebCallResult<CoinExPagedResult<CoinExOrderTradeExtended>>> GetUserTradesAsync(string symbol, int? page = null, int? limit = null, CancellationToken ct = default);

        /// <summary>
        /// Cancels an order. Requires API credentials
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/035cancel" /></para>
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
        /// <para><a href="https://github.com/coinexcom/coinex_exchange_api/wiki/0315cancel_all" /></para>
        /// </summary>
        /// <param name="symbol">The symbol the orders are on</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Execution statut</returns>
        Task<WebCallResult> CancelAllOrdersAsync(string symbol, CancellationToken ct = default);
    }
}
