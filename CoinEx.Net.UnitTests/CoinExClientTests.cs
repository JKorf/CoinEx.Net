using CryptoExchange.Net.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using CoinEx.Net.Objects;
using System.Collections.Generic;
using CryptoExchange.Net.Authentication;
using Moq;
using System.Linq;
using CryptoExchange.Net;
using System.Net;
using System;
using System.Threading.Tasks;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class CoinExClientTests
    {
        [Test]
        public async Task GetMarketList_Should_RespondWithMarketList()
        {
            // arrange
            string[] expected = new string[] { "ETHBTC", "BTCBCH", "ETHBCH" };
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = await objects.Client.GetSymbolsAsync();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(expected, result.Data);
        }

        [Test]
        public async Task GetMarketInfo_Should_RespondWithMarketInfo()
        {
            Dictionary<string, CoinExMarket> expected = new Dictionary<string, CoinExMarket>()
            {
                { "ETHBTC", new CoinExMarket() },
                { "ETHBCH", new CoinExMarket() },
            };
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
            }), CreateRequest(expected));

            // act
            var result = await objects.Client.GetMarketInfoAsync();

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public async Task GetMarketInfoBySymbol_Should_RespondWithMarketInfo()
        {
            
            Dictionary<string, CoinExMarket> expected = new Dictionary<string, CoinExMarket>()
            {
                { "ETHBTC", new CoinExMarket() },
            };
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
            }), CreateRequest(expected));

            // act
            var result = await objects.Client.GetMarketInfoAsync("ETHBTC");

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public async Task GetKlines_Should_RespondWithKlines()
        {
            // arrange
            CoinExKline[] expected = new CoinExKline[] {
                new CoinExKline(),
                new CoinExKline(),
            };
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = await objects.Client.GetKlinesAsync("ETHBTC", KlineInterval.FiveMinute);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected[0], result.Data.ToList()[0]);
            TestHelpers.PublicInstancePropertiesEqual(expected[1], result.Data.ToList()[1]);
        }

        [Test]
        public async Task ReceivingCoinExError_Should_ReturnCoinExErrorAndNotSuccess()
        {
            // arrange
            var response = JsonConvert.SerializeObject(new CoinExApiResult<object>() { Code = 101, Data = new object(), Message = "Some error" });
            var objects = TestHelpers.PrepareClient(() => Construct(), response);

            // act
            var result = await objects.Client.GetSymbolsAsync();

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.ToString().Contains("Some error"));
        }

        [Test]
        public async Task ReceivingHttpError_Should_ReturnErrorAndNotSuccess()
        {
            // arrange
            var objects = TestHelpers.PrepareClient(() => Construct(), "Error request", HttpStatusCode.BadRequest);

            // act
            var result = await objects.Client.GetSymbolsAsync();

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.ToString().Contains("Error request"));
        }

        [Test]
        public async Task GetLatestTransactions_Should_RespondWithTransactionList()
        {
            // arrange
            CoinExSymbolTrade[] expected = new CoinExSymbolTrade[] {
                new CoinExSymbolTrade() { Type = TransactionType.Buy },
                new CoinExSymbolTrade() { Type = TransactionType.Buy },
            };
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = await objects.Client.GetTradesHistoryAsync("ETHBTC");

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected[0], result.Data.ToList()[0]);
            TestHelpers.PublicInstancePropertiesEqual(expected[1], result.Data.ToList()[1]);
        }

        [Test]
        public async Task GetMarketDepth_Should_RespondWithMarketDepth()
        {
            // arrange
            CoinExOrderBook expected = new CoinExOrderBook();
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = await objects.Client.GetOrderBookAsync("ETHBTC", 1);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public async Task GetMarketState_Should_RespondWithMarketState()
        {
            // arrange
            CoinExSymbolState expected = new CoinExSymbolState();
            var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

            // act
            var result = await objects.Client.GetSymbolStateAsync("ETHBTC");

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public async Task GetMiningDifficulty_Should_RespondWithMiningDifficulty()
        {
            // arrange
            CoinExMiningDifficulty expected = new CoinExMiningDifficulty();
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = await objects.Client.GetMiningDifficultyAsync();

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public async Task GetOpenOrders_Should_RespondWithOpenOrders()
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
            var result = await objects.Client.GetOpenOrdersAsync("ETHBTC", 1, 10);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }

        [Test]
        public async Task GetBalances_Should_RespondWithBalances()
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
            var result = await objects.Client.GetBalancesAsync();

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }


        [Test]
        public async Task WithdrawalHistory_Should_RespondWithdrawalHistory()
        {
            // arrange
            CoinExPagedResult<CoinExWithdrawal> expected = new CoinExPagedResult<CoinExWithdrawal>
            {
                Data = new[] {
                 new CoinExWithdrawal(),
                 new CoinExWithdrawal()
                 }
            };
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = await objects.Client.GetWithdrawalHistoryAsync();

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected.Data.First(), result.Data.Data.First());
        }

        [Test]
        public async Task AuthenticatedRequests_Should_HaveAuthenticationHeader()
        {
            // arrange
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest("{}"));

            // act
            var result = await objects.Client.GetBalancesAsync();

            // assert
            objects.Request.Verify(r => r.AddHeader("Authorization", It.IsAny<string>()));
        }

        [Test]
        public async Task PostRequests_Should_HaveContentBody()
        {
            // arrange
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest("{}"));

            // act
            var result = await objects.Client.PlaceLimitOrderAsync("BTCETH", TransactionType.Buy, 1, 1);

            // assert
            objects.Request.Verify(r => r.SetContent(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task GetExecutedOrderDetails_Should_RespondWithExecutedOrderResults()
        {
            // arrange
            CoinExPagedResult<CoinExOrderTrade> expected = new CoinExPagedResult<CoinExOrderTrade>()
            {
                Count = 1,
                CurrentPage = 1,
                HasNext = false,
                Data = new CoinExOrderTrade[]
                {
                    new CoinExOrderTrade()
                }
            };
            var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
            {
                ApiCredentials = new ApiCredentials("test", "test")
            }), CreateRequest(expected));

            // act
            var result = await objects.Client.GetOrderTradesAsync(123, 1, 10);

            // assert
            Assert.AreEqual(true, result.Success);
            TestHelpers.PublicInstancePropertiesEqual(expected, result.Data);
        }
        
        [Test]
        public void ProvidingApiCredentials_Should_SaveApiCredentials()
        {
            // arrange
            // act
            var authProvider = new CoinExAuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"), null);

            // assert
            Assert.AreEqual(authProvider.Credentials.Key.GetString(), "TestKey");
            Assert.AreEqual(authProvider.Credentials.Secret.GetString(), "TestSecret");
        }

        [Test]
        [TestCase("TestStringToSign", "C351B1833970C30017EF9AE280A09570")]
        [TestCase("access_id=4DA36FFC61334695A66F8D29020EB589&amount=1.0&market=BTCBCH&price=680&tonce=1513746038205&type=buy&secret_key=B51068CF10B34E7789C374AB932696A05E0A629BE7BFC62F", "610AB90A1D31D45901D173E4F59C9384")]
        public void SigningString_Should_GiveCorrectSignResult(string input, string output)
        {
            // arrange
            var authProvider = new CoinExAuthenticationProvider(new ApiCredentials("4DA36FFC61334695A66F8D29020EB589", "B51068CF10B34E7789C374AB932696A05E0A629BE7BFC62F"), null);

            // act
            var sign = authProvider.Sign(input);

            // assert
            Assert.AreEqual(sign, output);
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

        [TestCase("BTCUSDT", true)]
        [TestCase("NANOUSDTA", true)]
        [TestCase("NANOBTC", true)]
        [TestCase("ETHBTC", true)]
        [TestCase("BEETC", true)]
        [TestCase("BETC", false)]
        [TestCase("BTC-USDT", false)]
        [TestCase("BTC-USD", false)]
        public void CheckValidCoinExSymbol(string symbol, bool isValid)
        {
            if (isValid)
                Assert.DoesNotThrow(symbol.ValidateCoinExSymbol);
            else
                Assert.Throws(typeof(ArgumentException), symbol.ValidateCoinExSymbol);
        }
    }
}
