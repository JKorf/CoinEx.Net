using CoinEx.Net.Objects;
using CryptoExchange.Net;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Interfaces.Clients.SpotApi;
using CoinEx.Net.Clients.SpotApi;

namespace CoinEx.Net.Clients
{
    /// <inheritdoc cref="ICoinExSocketClient" />
    public class CoinExSocketClient : BaseSocketClient, ICoinExSocketClient
    {
        #region Api clients

        /// <inheritdoc />
        public ICoinExSocketClientSpotStreams SpotStreams { get; }

        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of CoinExSocketClient with default options
        /// </summary>
        public CoinExSocketClient() : this(CoinExSocketClientOptions.Default)
        {
        }

        /// <summary>
        /// Create a new instance of CoinExSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public CoinExSocketClient(CoinExSocketClientOptions options) : base("CoinEx", options)
        {
            SpotStreams = AddApiClient(new CoinExSocketClientSpotStreams(log, options));
        }
        #endregion

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(CoinExSocketClientOptions options)
        {
            CoinExSocketClientOptions.Default = options;
        }

    }
}
