using CoinEx.Net.Enums;

namespace CoinEx.Net.Objects.Models.Socket
{
    /// <summary>
    /// Order update
    /// </summary>
    public record CoinExSocketOrderUpdate
    {
        /// <summary>
        /// The type of update
        /// </summary>
        public UpdateType UpdateType { get; set; }
        /// <summary>
        /// The order info
        /// </summary>
        public CoinExSocketOrder Order { get; set; } = default!;
    }
}
