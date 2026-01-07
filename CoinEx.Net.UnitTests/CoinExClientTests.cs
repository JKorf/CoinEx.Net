using NUnit.Framework;
using System.Collections.Generic;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CoinEx.Net.Clients;
using CryptoExchange.Net.Clients;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoinEx.Net.Interfaces.Clients;
using CryptoExchange.Net.Converters.SystemTextJson;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class CoinExClientTests
    {
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
                time: DateTimeConverter.ParseFromDouble(1700490704564),
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
                time: DateTimeConverter.ParseFromDouble(1700490704564),
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
