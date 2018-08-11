using CryptoExchange.Net.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using System.Collections.Generic;
using CryptoExchange.Net.Authentication;

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
            var client = TestHelpers.PrepareClient<CoinExClient>(() => Construct(), CreateRequest(expected));

            // act
            var result = client.GetMarketList();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(expected, result.Data);
        }

        [Test]
        public void GetKlines_Should_RespondWithKlines()
        {
            // arrange
            CoinExKline[] expected = new CoinExKline[] {
                new CoinExKline(),
                new CoinExKline(),
            };
            var client = TestHelpers.PrepareClient<CoinExClient>(() => Construct(), CreateRequest(expected));

            // act
            var result = client.GetKlines("ETHBTC", KlineInterval.FiveMinute);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void ReceivingCoinExError_Should_ReturnCoinExErrorAndNotSuccess()
        {
            // arrange
            var request = JsonConvert.SerializeObject(new CoinExApiResult<object>() { Code = 101, Data = new object(), Message = "Some error" });
            var client = TestHelpers.PrepareClient<CoinExClient>(() => Construct(), request);

            // act
            var result = client.GetMarketList();

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("Some error"));
        }

        [Test]
        public void GetLatestTransactions_Should_RespondWithTransactionList()
        {
            // arrange
            CoinExMarketTransaction[] expected = new CoinExMarketTransaction[] {
                new CoinExMarketTransaction() { Type = TransactionType.Buy },
                new CoinExMarketTransaction() { Type = TransactionType.Buy },
            };
            var client = TestHelpers.PrepareClient<CoinExClient>(() => Construct(), CreateRequest(expected));

            // act
            var result = client.GetLatestTransactions("ETHBTC");

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetMarketDepth_Should_RespondWithMarketDepth()
        {
            // arrange
            CoinExMarketDepth expected = new CoinExMarketDepth();
            var client = TestHelpers.PrepareClient<CoinExClient>(() => Construct(), CreateRequest(expected));

            // act
            var result = client.GetMarketDepth("ETHBTC", 1);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetMarketState_Should_RespondWithMarketState()
        {
            // arrange
            CoinExMarketState expected = new CoinExMarketState();
            var client = TestHelpers.PrepareClient<CoinExClient>(() => Construct(), CreateRequest(expected));

            // act
            var result = client.GetMarketState("ETHBTC");

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetMiningDifficulty_Should_RespondWithMiningDifficulty()
        {
            // arrange
            CoinExMiningDifficulty expected = new CoinExMiningDifficulty();
            var client = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = client.GetMiningDifficulty();

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetOpenOrders_Should_RespondWithOpenOrders()
        {
            // arrange
            CoinExPagedResult<CoinExOrder> expected = new CoinExPagedResult<CoinExOrder>()
            {
                Count = 1,
                CurrentPage = 1,
                HasNext = false,
                Data = new CoinExOrder[]
                {
                    new CoinExOrder() { Type = TransactionType.Buy }
                }
            };
            var client = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = client.GetOpenOrders("ETHBTC", 1, 10);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetBalances_Should_RespondWithBalances()
        {
            // arrange
            Dictionary<string, CoinExBalance> expected = new Dictionary<string, CoinExBalance>()
            {
                { "ETHBTC", new CoinExBalance() },
                { "ETHBCH", new CoinExBalance() },
            };
            var client = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = client.GetBalances();

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetExecutedOrderDetails_Should_RespondWithExecutedOrderResults()
        {
            // arrange
            CoinExPagedResult<CoinExOrderTransaction> expected = new CoinExPagedResult<CoinExOrderTransaction>()
            {
                Count = 1,
                CurrentPage = 1,
                HasNext = false,
                Data = new CoinExOrderTransaction[]
                {
                    new CoinExOrderTransaction()
                }
            };
            var client = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = client.GetExecutedOrderDetails(123, 1, 10);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
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

        private CoinExClient Construct(CoinExClientOptions options = null)
        {
            if (options != null)
                return new CoinExClient(options);
            return new CoinExClient();
        }
    }
}
