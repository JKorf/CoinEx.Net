using CoinEx.Net.Objects;
using CryptoExchange.Net.Objects;

namespace CoinEx.Net
{
    /// <summary>
    /// CoinEx environments
    /// </summary>
    public class CoinExEnvironment : TradeEnvironment
    {
        /// <summary>
        /// Spot Rest client address
        /// </summary>
        public string RestBaseAddress { get; }

        /// <summary>
        /// Spot Socket client address
        /// </summary>
        public string SocketBaseAddress { get; }

        internal CoinExEnvironment(string name,
            string restBaseAddress,
            string socketBaseAddress) : base(name)
        {
            RestBaseAddress = restBaseAddress;
            SocketBaseAddress = socketBaseAddress;
        }

        /// <summary>
        /// Live environment
        /// </summary>
        public static CoinExEnvironment Live { get; }
            = new CoinExEnvironment(TradeEnvironmentNames.Live,
                                     CoinExApiAddresses.Default.RestClientAddress,
                                     CoinExApiAddresses.Default.SocketClientAddress);

        /// <summary>
        /// Create a custom environment
        /// </summary>
        /// <param name="name"></param>
        /// <param name="restAddress"></param>
        /// <param name="socketAddress"></param>
        /// <returns></returns>
        public static CoinExEnvironment CreateCustom(
                        string name,
                        string restAddress,
                        string socketAddress)
            => new CoinExEnvironment(name, restAddress, socketAddress);
    }
}
