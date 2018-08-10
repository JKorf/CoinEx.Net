using CryptoExchange.Net.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using System.Collections.Generic;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class CoinExClientTests
    {
        [Test]
        public void GetMarketList_Should_RespondWithMarketList()
        {
            // arrange
            string[] expected = new string[] { "ETHBTC", "BTCBCH", "ETHBCH" };
            var client = TestHelpers.PrepareClient<CoinExClient>(CreateRequest(expected));

            // act
            var result = client.GetMarketList();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(expected, result.Data);
        }

        [Test]
        public void ReceivingCoinExError_Should_ReturnCoinExErrorAndNotSuccess()
        {
            // arrange
            var request = JsonConvert.SerializeObject(new CoinExApiResult<object>() { Code = 101, Data = new object(), Message = "Some error" });
            var client = TestHelpers.PrepareClient<CoinExClient>(request);

            // act
            var result = client.GetMarketList();

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("Some error"));
        }

        [Test]
        public void ReceivingServerError_Should_ReturnServerErrorAndNotSuccess()
        {
            // arrange
            var client = TestHelpers.PrepareExceptionClient<CoinExClient>("", "Unavailable", 504);

            // act
            var result = client.GetMarketList();

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("Unavailable"));
        }
        
        private string CreateRequest<T>(T obj)
        {
            return JsonConvert.SerializeObject(new CoinExApiResult<T>() { Code = 0, Data = obj, Message = "" });
        }
    }
}
