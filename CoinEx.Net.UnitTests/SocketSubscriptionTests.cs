using CoinEx.Net.Clients;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Objects.Options;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class SocketSubscriptionTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public async Task ValidateSpotSubscriptions(bool useUpdatedDeserialization)
        {
            var logger = new LoggerFactory();
            logger.AddProvider(new TraceLoggerProvider());
            var client = new CoinExSocketClient(Options.Create(new CoinExSocketOptions
            {
                UseUpdatedDeserialization = useUpdatedDeserialization,
                OutputOriginalData = true,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456")
            }), logger);
            var tester = new SocketSubscriptionValidator<CoinExSocketClient>(client, "Subscriptions/SpotApi", "wss://socket.coinex.com", "data");
            await tester.ValidateAsync<CoinExTicker[]>((client, handler) => client.SpotApiV2.SubscribeToTickerUpdatesAsync(handler), "Tickers", nestedJsonProperty: "data.state_list", ignoreProperties: ["period"]);
            await tester.ValidateAsync<CoinExOrderBook>((client, handler) => client.SpotApiV2.SubscribeToOrderBookUpdatesAsync("ETHUSDT", 5, null, true, handler), "OrderBook", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExTrade[]>((client, handler) => client.SpotApiV2.SubscribeToTradeUpdatesAsync("ETHUSDT", handler), "Trades", nestedJsonProperty: "data.deal_list");
            await tester.ValidateAsync<CoinExIndexPriceUpdate>((client, handler) => client.SpotApiV2.SubscribeToIndexPriceUpdatesAsync("ETHUSDT", handler), "IndexPrice", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExBookPriceUpdate>((client, handler) => client.SpotApiV2.SubscribeToBookPriceUpdatesAsync("ETHUSDT", handler), "BookPrice", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExOrderUpdate>((client, handler) => client.SpotApiV2.SubscribeToOrderUpdatesAsync(handler), "Order", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExStopOrderUpdate>((client, handler) => client.SpotApiV2.SubscribeToStopOrderUpdatesAsync(handler), "StopOrder", nestedJsonProperty: "data", ignoreProperties: ["status"]);
            await tester.ValidateAsync<CoinExUserTrade>((client, handler) => client.SpotApiV2.SubscribeToUserTradeUpdatesAsync(handler), "UserTrade", nestedJsonProperty: "data", ignoreProperties: ["position_id"]);
            await tester.ValidateAsync<CoinExBalanceUpdate[]>((client, handler) => client.SpotApiV2.SubscribeToBalanceUpdatesAsync(handler), "Balance", nestedJsonProperty: "data.balance_list");
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task ValidateFuturesSubscriptions(bool useUpdatedDeserialization)
        {
            var client = new CoinExSocketClient(opts =>
            {
                opts.UseUpdatedDeserialization = useUpdatedDeserialization;
                opts.OutputOriginalData = true;
                opts.ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "456");
            });
            var tester = new SocketSubscriptionValidator<CoinExSocketClient>(client, "Subscriptions/FuturesApi", "wss://socket.coinex.com", "data");
            await tester.ValidateAsync<CoinExFuturesTickerUpdate[]>((client, handler) => client.FuturesApi.SubscribeToTickerUpdatesAsync(handler), "Tickers", nestedJsonProperty: "data.state_list", ignoreProperties: ["period"]);
            await tester.ValidateAsync<CoinExOrderBook>((client, handler) => client.FuturesApi.SubscribeToOrderBookUpdatesAsync("ETHUSDT", 5, null, true, handler), "OrderBook", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExTrade[]>((client, handler) => client.FuturesApi.SubscribeToTradeUpdatesAsync("ETHUSDT", handler), "Trades", nestedJsonProperty: "data.deal_list");
            await tester.ValidateAsync<CoinExIndexPriceUpdate>((client, handler) => client.FuturesApi.SubscribeToIndexPriceUpdatesAsync("ETHUSDT", handler), "IndexPrice", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExBookPriceUpdate>((client, handler) => client.FuturesApi.SubscribeToBookPriceUpdatesAsync("ETHUSDT", handler), "BookPrice", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExFuturesOrderUpdate>((client, handler) => client.FuturesApi.SubscribeToOrderUpdatesAsync(handler), "Order", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExStopOrderUpdate>((client, handler) => client.FuturesApi.SubscribeToStopOrderUpdatesAsync(handler), "StopOrder", nestedJsonProperty: "data", ignoreProperties: ["status"]);
            await tester.ValidateAsync<CoinExUserTrade>((client, handler) => client.FuturesApi.SubscribeToUserTradeUpdatesAsync(handler), "UserTrade", nestedJsonProperty: "data");
            await tester.ValidateAsync<CoinExFuturesBalance[]>((client, handler) => client.FuturesApi.SubscribeToBalanceUpdatesAsync(handler), "Balance", nestedJsonProperty: "data.balance_list");
            await tester.ValidateAsync<CoinExPositionUpdate>((client, handler) => client.FuturesApi.SubscribeToPositionUpdatesAsync(handler), "Position", nestedJsonProperty: "data");
            
        }
    }
}
