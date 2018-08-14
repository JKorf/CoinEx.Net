using CryptoExchange.Net.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using System.Collections.Generic;
using CryptoExchange.Net.Authentication;
using Moq;
using System.Linq;
using CryptoExchange.Net;

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
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = objects.Client.GetMarketList();

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
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = objects.Client.GetKlines("ETHBTC", KlineInterval.FiveMinute);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void ReceivingCoinExError_Should_ReturnCoinExErrorAndNotSuccess()
        {
            // arrange
            var response = JsonConvert.SerializeObject(new CoinExApiResult<object>() { Code = 101, Data = new object(), Message = "Some error" });
            var objects = TestHelpers.PrepareClient(() => Construct(), response);

            // act
            var result = objects.Client.GetMarketList();

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
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = objects.Client.GetLatestTransactions("ETHBTC");

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetMarketDepth_Should_RespondWithMarketDepth()
        {
            // arrange
            CoinExMarketDepth expected = new CoinExMarketDepth();
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = objects.Client.GetMarketDepth("ETHBTC", 1);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetMarketState_Should_RespondWithMarketState()
        {
            // arrange
            CoinExMarketState expected = new CoinExMarketState();
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = objects.Client.GetMarketState("ETHBTC");

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void GetMiningDifficulty_Should_RespondWithMiningDifficulty()
        {
            // arrange
            CoinExMiningDifficulty expected = new CoinExMiningDifficulty();
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = objects.Client.GetMiningDifficulty();

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
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = objects.Client.GetOpenOrders("ETHBTC", 1, 10);

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
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = objects.Client.GetBalances();

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public void AuthenticatedRequests_Should_HaveAuthenticationHeader()
        {
            // arrange
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest("{}"));

            // act
            var result = objects.Client.GetBalances();

            // assert
            Assert.IsTrue(objects.Request.Object.Headers.AllKeys.Any(k => k == "authorization"));
        }

        [Test]
        public void PostRequests_Should_HaveContentBody()
        {
            // arrange
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest("{}"));

            // act
            var result = objects.Client.PlaceLimitOrder("BTCETH", TransactionType.Buy, 1, 1);

            // assert
            objects.RequestStream.Verify(r => r.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()));
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
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = objects.Client.GetExecutedOrderDetails(123, 1, 10);

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

        [Test]
        public void ProvidingApiCredentials_Should_SaveApiCredentials()
        {
            // arrange
            // act
            var authProvider = new CoinExAuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"));

            // assert
            Assert.AreEqual(authProvider.Credentials.Key.GetString(), "TestKey");
            Assert.AreEqual(authProvider.Credentials.Secret.GetString(), "TestSecret");
        }

        [Test]
        [TestCase("TestStringToSign", "C351B1833970C30017EF9AE280A09570")]
        [TestCase("access_id=4DA36FFC61334695A66F8D29020EB589&amount=1.0&market=BTCBCH&price=680&tonce=1513746038205&type=buy&secret_key=B51068CF10B34E7789C374AB932696A05E0A629BE7BFC62F", "C6F0DDA352101C2258F992A277397F4A")]
        public void SigningString_Should_GiveCorrectSignResult(string input, string output)
        {
            // arrange
            var authProvider = new CoinExAuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"));

            // act
            var sign = authProvider.Sign("TestStringToSign");

            // assert
            Assert.AreEqual(sign, "C351B1833970C30017EF9AE280A09570");
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
