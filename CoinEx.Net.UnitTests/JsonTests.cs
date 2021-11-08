using CoinEx.Net.Interfaces;
using CoinEx.Net.Interfaces.Clients.Rest.Spot;
using CoinEx.Net.Objects;
using CoinEx.Net.Testing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinEx.Net.UnitTests
{
    [TestFixture]
    public class JsonTests
    {
        private JsonToObjectComparer<ICoinExClientSpot> _comparer = new JsonToObjectComparer<ICoinExClientSpot>((json) => TestHelpers.CreateResponseClient(json, new CoinExClientSpotOptions()
        { ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "123"), OutputOriginalData = true }, System.Net.HttpStatusCode.OK));

        [Test]
        public async Task ValidateSpotAccountCalls()
        {   
            await _comparer.ProcessSubject("Spot/Account", c => c.Account,
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
            await _comparer.ProcessSubject("Spot/ExchangeData", c => c.ExchangeData,
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
            await _comparer.ProcessSubject("Spot/Trading", c => c.Trading,
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
