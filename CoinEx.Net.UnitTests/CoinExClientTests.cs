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
using CoinEx.Net.Enums;
using CoinEx.Net.Clients.Rest.Spot;
using System.Reflection;
using System.Diagnostics;
using CoinEx.Net.Clients.Socket;
using CoinEx.Net.Objects.Internal;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class CoinExClientTests
    {
        //[Test]
        //public async Task GetKlines_Should_RespondWithKlines()
        //{
        //    // arrange
        //    CoinExKline[] expected = new CoinExKline[] {
        //        new CoinExKline(),
        //        new CoinExKline(),
        //    };
        //    var objects = TestHelpers.PrepareClient(() => Construct(), CreateRequest(expected));

        //    // act
        //    var result = await objects.Client.GetKlinesAsync("ETHBTC", KlineInterval.FiveMinute);

        //    // assert
        //    Assert.AreEqual(true, result.Success);
        //    TestHelpers.PublicInstancePropertiesEqual(expected[0], result.Data.ToList()[0]);
        //    TestHelpers.PublicInstancePropertiesEqual(expected[1], result.Data.ToList()[1]);
        //}

        //[Test]
        //public async Task ReceivingCoinExError_Should_ReturnCoinExErrorAndNotSuccess()
        //{
        //    // arrange
        //    var response = JsonConvert.SerializeObject(new CoinExApiResult<object>() { Code = 101, Data = new object(), Message = "Some error" });
        //    var objects = TestHelpers.PrepareClient(() => Construct(), response);

        //    // act
        //    var result = await objects.Client.GetSymbolsAsync();

        //    // assert
        //    Assert.IsFalse(result.Success);
        //    Assert.IsNotNull(result.Error);
        //    Assert.IsTrue(result.Error.ToString().Contains("Some error"));
        //}

        //[Test]
        //public async Task ReceivingHttpError_Should_ReturnErrorAndNotSuccess()
        //{
        //    // arrange
        //    var objects = TestHelpers.PrepareClient(() => Construct(), "Error request", HttpStatusCode.BadRequest);

        //    // act
        //    var result = await objects.Client.GetSymbolsAsync();

        //    // assert
        //    Assert.IsFalse(result.Success);
        //    Assert.IsNotNull(result.Error);
        //    Assert.IsTrue(result.Error.ToString().Contains("Error request"));
        //}
      
        //[Test]
        //public async Task AuthenticatedRequests_Should_HaveAuthenticationHeader()
        //{
        //    // arrange
        //    var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
        //    {
        //        ApiCredentials = new ApiCredentials("test", "test")
        //    }), CreateRequest("{}"));

        //    // act
        //    var result = await objects.Client.GetBalancesAsync();

        //    // assert
        //    objects.Request.Verify(r => r.AddHeader("Authorization", It.IsAny<string>()));
        //}

        //[Test]
        //public async Task PostRequests_Should_HaveContentBody()
        //{
        //    // arrange
        //    var objects = TestHelpers.PrepareClient(() => Construct(new CoinExClientOptions()
        //    {
        //        ApiCredentials = new ApiCredentials("test", "test")
        //    }), CreateRequest("{}"));

        //    // act
        //    var result = await objects.Client.PlaceOrderAsync("BTCETH", OrderType.Limit, OrderSide.Buy, 1, 1);

        //    // assert
        //    objects.Request.Verify(r => r.SetContent(It.IsAny<string>(), It.IsAny<string>()));
        //}

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

        private CoinExClientSpot Construct(CoinExClientOptions options = null)
        {
            if (options != null)
                return new CoinExClientSpot(options);
            return new CoinExClientSpot();
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

        [Test]
        public void CheckRestInterfaces()
        {
            var assembly = Assembly.GetAssembly(typeof(CoinExClientSpot));
            var ignore = new string[] { "ICoinExClientSpot" };
            var clientInterfaces = assembly.GetTypes().Where(t => t.Name.StartsWith("ICoinExClient") && !ignore.Contains(t.Name));

            foreach (var clientInterface in clientInterfaces)
            {
                var implementation = assembly.GetTypes().Single(t => t.IsAssignableTo(clientInterface) && t != clientInterface);
                int methods = 0;
                foreach (var method in implementation.GetMethods().Where(m => m.ReturnType.IsAssignableTo(typeof(Task))))
                {
                    var interfaceMethod = clientInterface.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
                    Assert.NotNull(interfaceMethod);
                    methods++;
                }
                Debug.WriteLine($"{clientInterface.Name} {methods} methods validated");
            }
        }

        [Test]
        public void CheckSocketInterfaces()
        {
            var assembly = Assembly.GetAssembly(typeof(CoinExSocketClientSpot));
            var clientInterfaces = assembly.GetTypes().Where(t => t.Name.StartsWith("ICoinExSocketClient"));

            foreach (var clientInterface in clientInterfaces)
            {
                var implementation = assembly.GetTypes().Single(t => t.IsAssignableTo(clientInterface) && t != clientInterface);
                int methods = 0;
                foreach (var method in implementation.GetMethods().Where(m => m.ReturnType.IsAssignableTo(typeof(Task<CallResult<UpdateSubscription>>))))
                {
                    var interfaceMethod = clientInterface.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
                    Assert.NotNull(interfaceMethod);
                    methods++;
                }
                Debug.WriteLine($"{clientInterface.Name} {methods} methods validated");
            }
        }
    }
}
