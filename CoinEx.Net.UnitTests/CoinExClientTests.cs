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
using System.Reflection;
using System.Diagnostics;
using CoinEx.Net.Objects.Internal;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using CoinEx.Net.Clients;
using CoinEx.Net.ExtensionMethods;
using CryptoExchange.Net.Objects.Sockets;
using NUnit.Framework.Legacy;
using CoinEx.Net.Clients.SpotApiV1;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class CoinExClientTests
    {

        [TestCase()]
        public async Task ReceivingError_Should_ReturnErrorAndNotSuccess()
        {
            // arrange
            var client = TestHelpers.CreateClient();
            var resultObj = new CoinExApiResult()
            {
                Code = 400001,
                Message = "Error occured"
            };

            TestHelpers.SetResponse((CoinExRestClient)client, JsonConvert.SerializeObject(resultObj));

            // act
            var result = await client.SpotApi.ExchangeData.GetAssetsAsync();

            // assert
            ClassicAssert.IsFalse(result.Success);
            ClassicAssert.IsNotNull(result.Error);
            Assert.That(result.Error!.Code == 400001);
            Assert.That(result.Error.Message == "Error occured");
        }

        [TestCase()]
        public async Task ReceivingHttpErrorWithNoJson_Should_ReturnErrorAndNotSuccess()
        {
            // arrange
            var client = TestHelpers.CreateClient();
            TestHelpers.SetResponse((CoinExRestClient)client, "", System.Net.HttpStatusCode.BadRequest);

            // act
            var result = await client.SpotApi.ExchangeData.GetAssetsAsync();

            // assert
            ClassicAssert.IsFalse(result.Success);
            ClassicAssert.IsNotNull(result.Error);
        }

        [Test]
        public void ProvidingApiCredentials_Should_SaveApiCredentials()
        {
            // arrange
            // act
            var authProvider = new CoinExAuthenticationProvider(new ApiCredentials("TestKey", "TestSecret"), null);

            // assert
            Assert.That(authProvider.GetApiKey() == "TestKey");
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
            Assert.That(sign == output);
        }

        [Test]
        public void CheckRestInterfaces()
        {
            var assembly = Assembly.GetAssembly(typeof(CoinExRestClientSpotApi));
            var ignore = new string[] { "ICoinExClientSpot" };
            var clientInterfaces = assembly.GetTypes().Where(t => t.Name.StartsWith("ICoinExClient") && !ignore.Contains(t.Name));

            foreach (var clientInterface in clientInterfaces)
            {
                var implementation = assembly.GetTypes().Single(t => t.IsAssignableTo(clientInterface) && t != clientInterface);
                int methods = 0;
                foreach (var method in implementation.GetMethods().Where(m => m.ReturnType.IsAssignableTo(typeof(Task))))
                {
                    var interfaceMethod = clientInterface.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
                    ClassicAssert.NotNull(interfaceMethod, $"Missing interface for method {method.Name} in {implementation.Name} implementing interface {clientInterface.Name}");
                    methods++;
                }
                Debug.WriteLine($"{clientInterface.Name} {methods} methods validated");
            }
        }

        [Test]
        public void CheckSocketInterfaces()
        {
            var assembly = Assembly.GetAssembly(typeof(CoinExSocketClient));
            var clientInterfaces = assembly.GetTypes().Where(t => t.Name.StartsWith("ICoinExSocketClient"));

            foreach (var clientInterface in clientInterfaces)
            {
                var implementation = assembly.GetTypes().Single(t => t.IsAssignableTo(clientInterface) && t != clientInterface);
                int methods = 0;
                foreach (var method in implementation.GetMethods().Where(m => m.ReturnType.IsAssignableTo(typeof(Task<CallResult<UpdateSubscription>>))))
                {
                    var interfaceMethod = clientInterface.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray());
                    ClassicAssert.NotNull(interfaceMethod, $"Missing interface for method {method.Name} in {implementation.Name} implementing interface {clientInterface.Name}");
                    methods++;
                }
                Debug.WriteLine($"{clientInterface.Name} {methods} methods validated");
            }
        }
    }
}
