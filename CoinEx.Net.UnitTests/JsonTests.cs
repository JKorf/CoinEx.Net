using CoinEx.Net.Interfaces;
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
        private JsonToObjectComparer<ICoinExClient> _comparer = new JsonToObjectComparer<ICoinExClient>((json) => TestHelpers.CreateResponseClient(json, new CoinExClientOptions()
        { ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "123"), OutputOriginalData = true }, System.Net.HttpStatusCode.OK));

        [Test]
        public async Task ValidateSpotCalls()
        {   
            await _comparer.ProcessSubject(c => c,
                useNestedJsonPropertyForCompare: new Dictionary<string, string> 
                { 

                },
                ignoreProperties: new Dictionary<string, List<string>>
                {
                    { "GetTradeHistoryAsync", new List<string> { "date" } },
                    { "GetOpenStopOrdersAsync", new List<string> { "account_id", "order_id", "maker_fee", "taker_fee", "state" } },
                },
                useNestedJsonPropertyForAllCompare: new List<string> { "data" },
                parametersToSetNull: new [] { "limit" }
                );
        }  
    }
}
