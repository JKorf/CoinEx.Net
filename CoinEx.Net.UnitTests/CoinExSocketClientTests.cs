using CoinEx.Net.Objects;
using CoinEx.Net.Objects.Websocket;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class CoinExSocketClientTests
    {
        [Test]
        public void SubscribingWithNormalResponse_Should_Succeed()
        {
            // Arrange
            var client = TestHelpers.PrepareSocketClient(() => Construct());

            // Act
            var sendWait = TestHelpers.WaitForSend(client);
            var subTask = client.SubscribeToSymbolStateUpdatesAsync(data => { });
            if(!sendWait.Result)
                Assert.Fail("No sub response");

            InvokeSubResponse(client);
            subTask.Wait(5000);

            // Assert
            Assert.IsTrue(subTask.IsCompleted);
            Assert.IsTrue(subTask.Result.Success);
        }

        [Test]
        public void SubscribingWithErrorResponse_Should_Fail()
        {
            // Arrange
            var client = TestHelpers.PrepareSocketClient(() => Construct());

            // Act
            var subTask = client.SubscribeToSymbolStateUpdatesAsync(data => { });
            Thread.Sleep(10);
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(
                new CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>()
                {
                    Error = new CoinExSocketError() { Code = 1, Message = "Error" },
                    Id = 1,
                    Result = new CoinExSocketRequestResponseMessage() { Status = "error" }
                }));
            subTask.Wait();

            // Assert
            Assert.IsFalse(subTask.Result.Success);
        }

        [Test]
        public void SubscribingToMarketStateUpdates_Should_InvokeUpdateMethod()
        {
            // Arrange
            var sb = new StringBuilder();
            var testWriter = new StringWriter(sb);
            var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { testWriter }
            }));

            var expected = new Dictionary<string, CoinExSocketSymbolState> { { "ETHBTC", new CoinExSocketSymbolState() } };
            Dictionary<string, CoinExSocketSymbolState> actual = null;

            var sendWait = TestHelpers.WaitForSend(client);
            var subTask = client.SubscribeToSymbolStateUpdatesAsync(data =>
            {
                actual = data;
            });

            if (!sendWait.Result)
                Assert.Fail(sb.ToString());

            InvokeSubResponse(client);
            subTask.Wait();

            // Act
            InvokeSubUpdate(client, "state.update", expected);

            // Assert
            Assert.IsTrue(subTask.Result.Success);
            Assert.IsTrue(actual != null);
            TestHelpers.PublicInstancePropertiesEqual(expected, actual);
        }

        [Test]
        public void SubscribingToMarketTransactionUpdates_Should_InvokeUpdateMethod()
        {
            // Arrange
            var sb = new StringBuilder();
            var testWriter = new StringWriter(sb);
            var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { testWriter }
            }));

            var expected = new CoinExSocketSymbolTrade[] {
                new CoinExSocketSymbolTrade() { Type = TransactionType.Buy },
                new CoinExSocketSymbolTrade() {  Type = TransactionType.Sell } };
            CoinExSocketSymbolTrade[] actual = null;

            var sendWait = TestHelpers.WaitForSend(client);
            var subTask = client.SubscribeToSymbolTradeUpdatesAsync("ETHBTC", (market, data) =>
            {
                actual = data.ToArray();
            });

            if (!sendWait.Result)
                Assert.Fail(sb.ToString());

            InvokeSubResponse(client);
            subTask.Wait();

            // Act
            InvokeSubUpdate(client, "deals.update", "ETHBTC", expected );

            // Assert
            Assert.IsTrue(subTask.Result.Success);
            Assert.IsTrue(actual != null);
            TestHelpers.PublicInstancePropertiesEqual(expected, actual);
        }

        [Test]
        public void SubscribingToMarketTransactionUpdates_Should_InvokeUpdateMethodWithExtraReceivedParamter()
        {
            // Arrange
            var sb = new StringBuilder();
            var testWriter = new StringWriter(sb);
            var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { testWriter }
            }));

            var expected = new CoinExSocketSymbolTrade[] {
                new CoinExSocketSymbolTrade() { Type = TransactionType.Buy },
                new CoinExSocketSymbolTrade() {  Type = TransactionType.Sell } };
            CoinExSocketSymbolTrade[] actual = null;
            var sendWait = TestHelpers.WaitForSend(client);
            var subTask = client.SubscribeToSymbolTradeUpdatesAsync("ETHBTC", (market, data) =>
            {
                actual = data.ToArray();
            });

            if (!sendWait.Result)
                Assert.Fail(sb.ToString());

            InvokeSubResponse(client);
            subTask.Wait();

            // Act
            InvokeSubUpdate(client, "deals.update", "ETHBTC", expected);

            // Assert
            Assert.IsTrue(subTask.Result.Success);
            Assert.IsTrue(actual != null);
            TestHelpers.PublicInstancePropertiesEqual(expected, actual);
        }

        [Test]
        public void SubscribingToMarketDepthUpdates_Should_InvokeUpdateMethod()
        {
            // Arrange
            var sb = new StringBuilder();
            var testWriter = new StringWriter(sb);
            var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { testWriter }
            }));
            var expected = new CoinExSocketOrderBook()
            {
                Asks = new List<CoinExDepthEntry> { new CoinExDepthEntry() { Quantity = 0.1m, Price = 0.2m } },
                Bids = new List<CoinExDepthEntry> { new CoinExDepthEntry() { Quantity = 0.1m, Price = 0.2m } }
            };
            CoinExSocketOrderBook actual = null;

            var sendWait = TestHelpers.WaitForSend(client);
            var subTask = client.SubscribeToOrderBookUpdatesAsync("ETHBTC", 10, 1, (market, full, data) =>
            {
                actual = data;
            });
            if (!sendWait.Result)
                Assert.Fail(sb.ToString());

            InvokeSubResponse(client);
            subTask.Wait();

            // Act
            InvokeSubUpdate(client, "depth.update", true, expected, "ETHBTC");
            

            // Assert
            Assert.IsTrue(subTask.Result.Success);
            Assert.IsTrue(actual != null);
            TestHelpers.PublicInstancePropertiesEqual(expected, actual);
        }

        [Test]
        public void SubscribingToMarketKlineUpdates_Should_InvokeUpdateMethod()
        {
            // Arrange
            var sb = new StringBuilder();
            var testWriter = new StringWriter(sb);
            var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            {
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { testWriter }
            }));

            var expected = new CoinExKline[]
            {
                new CoinExKline(),
                new CoinExKline(),
            };
            CoinExKline[] actual = null;
            var sendWait = TestHelpers.WaitForSend(client);
            var subTask = client.SubscribeToKlineUpdatesAsync("ETHBTC", KlineInterval.FiveMinute, (market, data) =>
            {
                actual = data.ToArray();
            });
            if (!sendWait.Result)
                Assert.Fail(sb.ToString());

            InvokeSubResponse(client);
            subTask.Wait();

            // Act
            InvokeSubUpdate(client, "kline.update", expected);

            // Assert
            Assert.IsTrue(subTask.Result.Success);
            Assert.IsTrue(actual != null);
            TestHelpers.PublicInstancePropertiesEqual(expected, actual);
        }

        [Test]
        public void SubscribingToBalanceUpdates_Should_InvokeUpdateMethod()
        {
            // Arrange
            var sb = new StringBuilder();
            var testWriter = new StringWriter(sb);
            var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("TestKey", "test"),
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { testWriter }
            }));
            var expected = new Dictionary<string, CoinExBalance>
            {
                { "ETHBTC",  new CoinExBalance() },
                { "ETHBCH",  new CoinExBalance() }
            };

            Dictionary<string, CoinExBalance> actual = null;
            var sendWait = TestHelpers.WaitForSend(client);
            var subTask = client.SubscribeToBalanceUpdatesAsync((data) =>
            {
                actual = data;
            });
            if (!sendWait.Result)
                Assert.Fail(sb.ToString());

            InvokeSubResponse(client);
            Thread.Sleep(10);
            InvokeSubResponse(client, true);
            subTask.Wait();

            // Act
            InvokeSubUpdate(client, "asset.update", expected);

            // Assert
            Assert.IsTrue(subTask.Result.Success);
            Assert.IsTrue(actual != null);
            TestHelpers.PublicInstancePropertiesEqual(expected, actual);
        }

        [Test]
        public void SubscribingToOrderUpdates_Should_InvokeUpdateMethod()
        {
            // Arrange
            var sb = new StringBuilder();
            var testWriter = new StringWriter(sb);
            var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("TestKey", "test"),
                LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { testWriter }
            }));

            var expected = new CoinExSocketOrder();
            CoinExSocketOrder actual = null;
            var sendWait = TestHelpers.WaitForSend(client);
            var subTask = client.SubscribeToOrderUpdatesAsync(new[] { "ETHBTC", "ETHBCH" }, (updateType, data) =>
            {
                actual = data;
            });

            if(!sendWait.Result)
                Assert.Fail(sb.ToString());

            InvokeSubResponse(client);
            Thread.Sleep(10);
            InvokeSubResponse(client, true);
            subTask.Wait();

            // Act
            InvokeSubUpdate(client, "order.update", 1, expected);

            // Assert
            Assert.IsTrue(subTask.Result.Success);
            Assert.IsTrue(actual != null);
            TestHelpers.PublicInstancePropertiesEqual(expected, actual);
        }

        [Test]
        public void SubscribingToAuthenticatedStream_Should_SendAuthentication()
        {
            // TODO
        }

        [Test]
        public void LosingConnectionAfterSubscribing_Should_BeReconnected()
        {
            // Arrange
            //var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            //{
            //    ReconnectionInterval = TimeSpan.FromMilliseconds(100),
            //    SubscriptionResponseTimeout = TimeSpan.FromMilliseconds(100)
            //}));
            //var sendWait = TestHelpers.WaitForSend(client);
            //var subTask = client.SubscribeToMarketStateUpdatesAsync(data => { });
            //if (!sendWait.Result)
            //    Assert.Fail("No sub request send");

            //InvokeSubResponse(client);
            //subTask.Wait();

            //// Act
            //var conWait = TestHelpers.WaitForConnect(client);
            //TestHelpers.CloseWebsocket(client);

            //// Assert
            //Assert.IsTrue(conWait.Result);
        }

        [Test]
        public void LosingConnectionDuringResubscribing_Should_BeReconnected()
        {
            //// Arrange
            //var sb = new StringBuilder();
            //var testWriter = new StringWriter(sb);
            //var client = TestHelpers.PrepareSocketClient(() => Construct(new CoinExSocketClientOptions()
            //{
            //    ReconnectionInterval = TimeSpan.FromMilliseconds(100),
            //    SubscriptionResponseTimeout = TimeSpan.FromMilliseconds(500),
            //    LogVerbosity = CryptoExchange.Net.Logging.LogVerbosity.Debug,
            //    LogWriters = new List<TextWriter> { testWriter }
            //}));
            //var sendWait = TestHelpers.WaitForSend(client);
            //var subTask = client.SubscribeToMarketStateUpdatesAsync(data => { });
            //if (!sendWait.Result)
            //    Assert.Fail(sb.ToString());

            //InvokeSubResponse(client);
            //subTask.Wait();
            //if(!subTask.Result.Success)
            //    Assert.Fail(sb.ToString());

            //// Act
            //// DC1
            //var conWait = TestHelpers.WaitForConnect(client);
            //var resubSendWait = TestHelpers.WaitForSend(client);
            //TestHelpers.CloseWebsocket(client);
            //var reconResult = conWait.Result;
            //var resubResult = resubSendWait.Result;
            //if(!reconResult)
            //    Assert.Fail(sb.ToString());
            //if (!resubResult)
            //    Assert.Fail(sb.ToString());

            //// DC2
            //conWait = TestHelpers.WaitForConnect(client);
            //resubSendWait = TestHelpers.WaitForSend(client);
            //TestHelpers.CloseWebsocket(client);
            //reconResult = conWait.Result;
            //resubResult = resubSendWait.Result;
            //if (!reconResult)
            //    Assert.Fail(sb.ToString());
            //if (!resubResult)
            //    Assert.Fail(sb.ToString());

            //// Assert
            //Assert.Pass();
        }

        private void InvokeSubResponse(CoinExSocketClient client, bool previousId = false)
        {
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(
                new CoinExSocketRequestResponse<CoinExSocketRequestResponseMessage>()
                {
                    Error = null,
                    Id = previousId ? CoinExSocketClient.LastId - 1: CoinExSocketClient.LastId,
                    Result = new CoinExSocketRequestResponseMessage() { Status = "success" }
                }));
        }

        private void InvokeSubUpdate(CoinExSocketClient client, string method, params object[] parameters)
        {
            TestHelpers.InvokeWebsocket(client, JsonConvert.SerializeObject(new CoinExSocketResponse()
            {
                Id = null,
                Method = method,
                Parameters = parameters
            },
            new TimestampSecondsConverter()));
        }

        private CoinExSocketClient Construct(CoinExSocketClientOptions options = null)
        {
            if (options != null)
                return new CoinExSocketClient(options);
            return new CoinExSocketClient();
        }
    }
}
