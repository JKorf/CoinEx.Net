using CoinEx.Net.SymbolOrderBooks;
using CryptoExchange.Net.Objects;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class OrderBookTests
    {
        [TestCase]
        public async Task StartOrderBook_Should_BeSynced()
        {
            // arrange
            using var book = new CoinExSpotSymbolOrderBook("BTCUSDT");

            // act
            var result = await book.StartAsync();

            // assert
            Assert.That(book.Status == OrderBookStatus.Synced);
        }
    }
}
