using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace CoinEx.Net.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<CoinExKey>();
            IConfiguration Configuration = builder.Build();
            var apiKey = Configuration.GetSection("CoinExKey").Get<CoinExKey>();
            CoinExClient client = new CoinExClient();
            client.SetApiCredentials(apiKey.APIKey, apiKey.APISecret);
            var withdrawalHistory = client.GetWithdrawalHistory();
            var depositHistory = client.GetDepositHistory();
            var marketInfo = client.GetMarketInfo();
            Console.WriteLine("Hello World!");
        }
    }
}
