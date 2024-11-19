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
        /// ctor for DI, use <see cref="CreateCustom"/> for creating a custom environment
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public CoinExEnvironment() : base(TradeEnvironmentNames.Live)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        { }

        /// <summary>
        /// Get the CoinEx environment by name
        /// </summary>
        public static CoinExEnvironment? GetEnvironmentByName(string? name)
         => name switch
         {
             TradeEnvironmentNames.Live => Live,
             "" => Live,
             null => Live,
             _ => default
         };

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
