﻿using Newtonsoft.Json;
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
using CryptoExchange.Net.Clients;
using System.Net.Http;
using CryptoExchange.Net.Converters.JsonNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoinEx.Net.Interfaces.Clients;

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
            Assert.That(authProvider.ApiKey == "TestKey");
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
        public void CheckSignatureExample1()
        {
            var authProvider = new CoinExV2AuthenticationProvider(
                new ApiCredentials("XXXXXXXXXX", "XXXXXXXXXX")
                );
            var client = (RestApiClient)new CoinExRestClient().SpotApiV2;

            CryptoExchange.Net.Testing.TestHelpers.CheckSignature(
                client,
                authProvider,
                HttpMethod.Get,
                "/v2/spot/pending-order",
                (uriParams, bodyParams, headers) =>
                {
                    return headers["X-COINEX-SIGN"].ToString();
                },
                "444976F4F21D422AB7091B47D9201EB02A6614FF1F4C7B9F6CA57BFF632030A5",
                new Dictionary<string, object>
                {
                    { "market", "BTCUSDT" },
                    { "market_type", "SPOT" },
                    { "side", "buy" },
                    { "page", "1" },
                    { "limit", "10" },
                },
                time: DateTimeConverter.ConvertFromMilliseconds(1700490703564),
                disableOrdering: true);
        }

        [Test]
        public void CheckSignatureExample2()
        {
            var authProvider = new CoinExV2AuthenticationProvider(
                new ApiCredentials("XXXXXXXXXX", "XXXXXXXXXX")
                );
            var client = (RestApiClient)new CoinExRestClient().SpotApiV2;

            CryptoExchange.Net.Testing.TestHelpers.CheckSignature(
                client,
                authProvider,
                HttpMethod.Post,
                "/v2/spot/pending-order",
                (uriParams, bodyParams, headers) =>
                {
                    return headers["X-COINEX-SIGN"].ToString();
                },
                "5128936CEDB75A512991A47BCCB0A78F6D9F6F540C07A17AE68D31F16E06A17F",
                new Dictionary<string, object>
                {
                    { "market", "BTCUSDT" },
                    { "market_type", "SPOT" },
                    { "side", "buy" },
                    { "page", "1" },
                    { "limit", "10" },
                },
                time: DateTimeConverter.ConvertFromMilliseconds(1700490703564),
                disableOrdering: true);
        }

        [Test]
        public void CheckInterfaces()
        {
            CryptoExchange.Net.Testing.TestHelpers.CheckForMissingRestInterfaces<CoinExRestClient>();
            CryptoExchange.Net.Testing.TestHelpers.CheckForMissingSocketInterfaces<CoinExSocketClient>();
        }

        [Test]
        [TestCase(TradeEnvironmentNames.Live, "https://api.coinex.com")]
        [TestCase("", "https://api.coinex.com")]
        public void TestConstructorEnvironments(string environmentName, string expected)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "CoinEx:Environment:Name", environmentName },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddCoinEx(configuration.GetSection("CoinEx"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<ICoinExRestClient>();

            var address = client.SpotApiV2.BaseAddress;

            Assert.That(address, Is.EqualTo(expected));
        }

        [Test]
        public void TestConstructorNullEnvironment()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "CoinEx", null },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddCoinEx(configuration.GetSection("CoinEx"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<ICoinExRestClient>();

            var address = client.SpotApiV2.BaseAddress;

            Assert.That(address, Is.EqualTo("https://api.coinex.com"));
        }

        [Test]
        public void TestConstructorApiOverwriteEnvironment()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "CoinEx:Environment:Name", "test" },
                    { "CoinEx:Rest:Environment:Name", "live" },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddCoinEx(configuration.GetSection("CoinEx"));
            var provider = collection.BuildServiceProvider();

            var client = provider.GetRequiredService<ICoinExRestClient>();

            var address = client.SpotApiV2.BaseAddress;

            Assert.That(address, Is.EqualTo("https://api.coinex.com"));
        }

        [Test]
        public void TestConstructorConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ApiCredentials:Key", "123" },
                    { "ApiCredentials:Secret", "456" },
                    { "ApiCredentials:Memo", "000" },
                    { "Socket:ApiCredentials:Key", "456" },
                    { "Socket:ApiCredentials:Secret", "789" },
                    { "Socket:ApiCredentials:Memo", "xxx" },
                    { "Rest:OutputOriginalData", "true" },
                    { "Socket:OutputOriginalData", "false" },
                    { "Rest:Proxy:Host", "host" },
                    { "Rest:Proxy:Port", "80" },
                    { "Socket:Proxy:Host", "host2" },
                    { "Socket:Proxy:Port", "81" },
                }).Build();

            var collection = new ServiceCollection();
            collection.AddCoinEx(configuration);
            var provider = collection.BuildServiceProvider();

            var restClient = provider.GetRequiredService<ICoinExRestClient>();
            var socketClient = provider.GetRequiredService<ICoinExSocketClient>();

            Assert.That(((BaseApiClient)restClient.SpotApiV2).OutputOriginalData, Is.True);
            Assert.That(((BaseApiClient)socketClient.SpotApiV2).OutputOriginalData, Is.False);
            Assert.That(((BaseApiClient)restClient.SpotApiV2).AuthenticationProvider.ApiKey, Is.EqualTo("123"));
            Assert.That(((BaseApiClient)socketClient.SpotApiV2).AuthenticationProvider.ApiKey, Is.EqualTo("456"));
            Assert.That(((BaseApiClient)restClient.SpotApiV2).ClientOptions.Proxy.Host, Is.EqualTo("host"));
            Assert.That(((BaseApiClient)restClient.SpotApiV2).ClientOptions.Proxy.Port, Is.EqualTo(80));
            Assert.That(((BaseApiClient)socketClient.SpotApiV2).ClientOptions.Proxy.Host, Is.EqualTo("host2"));
            Assert.That(((BaseApiClient)socketClient.SpotApiV2).ClientOptions.Proxy.Port, Is.EqualTo(81));
        }
    }
}
