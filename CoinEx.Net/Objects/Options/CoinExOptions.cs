using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Options
{
    /// <summary>
    /// CoinEx options
    /// </summary>
    public class CoinExOptions
    {
        /// <summary>
        /// Rest client options
        /// </summary>
        public CoinExRestOptions Rest { get; set; } = new CoinExRestOptions();

        /// <summary>
        /// Socket client options
        /// </summary>
        public CoinExSocketOptions Socket { get; set; } = new CoinExSocketOptions();

        /// <summary>
        /// Trade environment. Contains info about URL's to use to connect to the API. Use `CoinExEnvironment` to swap environment, for example `Environment = CoinExEnvironment.Live`
        /// </summary>
        public CoinExEnvironment? Environment { get; set; }

        /// <summary>
        /// The api credentials used for signing requests.
        /// </summary>
        public ApiCredentials? ApiCredentials { get; set; }

        /// <summary>
        /// The DI service lifetime for the ICoinExSocketClient
        /// </summary>
        public ServiceLifetime? SocketClientLifeTime { get; set; }
    }
}
