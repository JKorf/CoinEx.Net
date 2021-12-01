using CoinEx.Net.Interfaces;
using CoinEx.Net.Objects;
using CoinEx.Net.Testing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces;
using CoinEx.Net.Interfaces.Clients;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class JsonTests
    {
        private JsonToObjectComparer<ICoinExClient> _comparer = new JsonToObjectComparer<ICoinExClient>((json) => TestHelpers.CreateResponseClient(json, new CoinExClientOptions()
        { 
            ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "123"), 
            OutputOriginalData = true, 
            SpotApiOptions = new CryptoExchange.Net.Objects.RestApiClientOptions
            {
                RateLimiters = new List<IRateLimiter>()
            }
        }, System.Net.HttpStatusCode.OK));

        [Test]
        public async Task ValidateSpotAccountCalls()
        {   
            await _comparer.ProcessSubject("Spot/Account", c => c.SpotApi.Account,
                useNestedJsonPropertyForCompare: new Dictionary<string, string> 
                { 

                },
                ignoreProperties: new Dictionary<string, List<string>>
                {
                },
                useNestedJsonPropertyForAllCompare: new List<string> { "data" },
                parametersToSetNull: new [] { "limit" }
                );
        }

        [Test]
        public async Task ValidateSpotExchangeDataCalls()
        {
            await _comparer.ProcessSubject("Spot/ExchangeData", c => c.SpotApi.ExchangeData,
                useNestedJsonPropertyForCompare: new Dictionary<string, string>
                {

                },
                ignoreProperties: new Dictionary<string, List<string>>
                {
                    { "GetTradeHistoryAsync", new List<string> { "date" } },
                },
                useNestedJsonPropertyForAllCompare: new List<string> { "data" },
                parametersToSetNull: new[] { "limit" }
                );
        }

        [Test]
        public async Task ValidateSpotTradingCalls()
        {
            await _comparer.ProcessSubject("Spot/Trading", c => c.SpotApi.Trading,
                useNestedJsonPropertyForCompare: new Dictionary<string, string>
                {

                },
                ignoreProperties: new Dictionary<string, List<string>>
                {
                    { "GetOpenStopOrdersAsync", new List<string> { "account_id", "order_id", "maker_fee", "taker_fee", "state" } },
                },
                useNestedJsonPropertyForAllCompare: new List<string> { "data" },
                parametersToSetNull: new[] { "limit" }
                );
        }
    }
}
