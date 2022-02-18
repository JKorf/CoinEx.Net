using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects.Models;
using CoinEx.Net.Objects.Models.Socket;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CoinEx.Net.UnitTests
{
    internal class JsonSocketTests
    {
        [Test]
        public async Task ValidateTickerUpdateStreamJson()
        {
            await TestFileToObject<Dictionary<string, CoinExSocketSymbolState>>(@"JsonResponses\Spot\Socket\TickerUpdate.txt");
        }

        [Test]
        public async Task ValidateTradeUpdateStreamJson()
        {
            await TestFileToObject<IEnumerable<CoinExSocketSymbolTrade>>(@"JsonResponses\Spot\Socket\TradeUpdate.txt");
        }

        [Test]
        public async Task ValidateBalanceUpdateStreamJson()
        {
            await TestFileToObject<Dictionary<string, CoinExBalance>>(@"JsonResponses\Spot\Socket\BalanceUpdate.txt");
        }

        [Test]
        public async Task ValidateOrderUpdateStreamJson()
        {
            await TestFileToObject<CoinExSocketOrder>(@"JsonResponses\Spot\Socket\OrderUpdate.txt", new List<string> { "account", "option" });
        }

        private static async Task TestFileToObject<T>(string filePath, List<string> ignoreProperties = null)
        {
            var listener = new EnumValueTraceListener();
            Trace.Listeners.Add(listener);
            var path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string json;
            try
            {
                var file = File.OpenRead(Path.Combine(path, filePath));
                using var reader = new StreamReader(file);
                json = await reader.ReadToEndAsync();
            }
            catch (FileNotFoundException)
            {
                throw;
            }

            var result = JsonConvert.DeserializeObject<T>(json);
            JsonToObjectComparer<ICoinExSocketClient>.ProcessData("", result, json, ignoreProperties: new Dictionary<string, List<string>>
            {
                { "", ignoreProperties ?? new List<string>() }
            });
            Trace.Listeners.Remove(listener);
        }
    }

    internal class EnumValueTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            if (message.Contains("Cannot map"))
                throw new Exception("Enum value error: " + message);
        }

        public override void WriteLine(string message)
        {
            if (message.Contains("Cannot map"))
                throw new Exception("Enum value error: " + message);
        }
    }
}
