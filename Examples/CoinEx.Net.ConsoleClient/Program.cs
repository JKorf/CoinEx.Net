using CryptoExchange.Net.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoinEx.Net.Examples
{
    class Program
    {
        static async void Main(string[] args)
        {
            string[] marketList = new string[0];
            using(var client = new CoinExClient())
            {
                var listResult = await client.GetSymbolsAsync();
                if (!listResult.Success)
                    Console.WriteLine("Failed to get market list: " + listResult.Error);
                else
                {
                    Console.WriteLine("Support market list: " + string.Join(", ", listResult.Data));
                    marketList = listResult.Data.ToArray();
                }
            }

            Console.WriteLine();
            string market = null;
            while (true)
            {
                Console.WriteLine("Enter market name to subscribe to state updates");
                market = Console.ReadLine();
                if (!marketList.Contains(market.ToUpper()))
                    Console.WriteLine("Unknown market, try again");
                else
                    break;
            }

            var socketClient = new CoinExSocketClient(new Objects.CoinExSocketClientOptions()
            {
                LogLevel = LogLevel.Information,
                LogWriters = new List<ILogger> { new ConsoleLogger() }
            });
            await socketClient.SubscribeToSymbolStateUpdatesAsync(market, data =>
            {
                Console.WriteLine($"Last price of {market}: {data.Data.Close}");
            });
            Console.ReadLine();
        }
    }
}
