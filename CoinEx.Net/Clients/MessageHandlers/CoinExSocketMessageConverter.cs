using CoinEx.Net;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coinbase.Net.Clients.MessageHandlers
{
    internal class CoinExSocketMessageConverter : JsonSocketMessageHandler
    {
        private static readonly HashSet<string> _withoutSymbolTopics = new HashSet<string>
        {
            "state.update",
            "deals.update",
            "user_deals.update",
        };

        public override JsonSerializerOptions Options { get; } = SerializerOptions.WithConverters(CoinExExchange._serializerContext);

        protected override MessageEvaluator[] MessageEvaluators { get; } = [

            new MessageEvaluator {
                Priority = 1,
                Fields = [
                    new PropertyFieldReference("method") { Constraint = x => !_withoutSymbolTopics.Contains(x!) },
                    new PropertyFieldReference("market") { Depth = 2 }
                ],
                IdentifyMessageCallback = x => $"{x.FieldValue("method")}{x.FieldValue("market")}",
            },

            new MessageEvaluator {
                Priority = 2,
                Fields = [
                    new PropertyFieldReference("method") { Constraint = x => _withoutSymbolTopics.Contains(x!) },
                ],
                IdentifyMessageCallback = x => $"{x.FieldValue("method")}",
            },

            new MessageEvaluator {
                Priority = 3,
                ForceIfFound = true,
                Fields = [
                    new PropertyFieldReference("id") { Constraint = x => x != null },
                ],
                IdentifyMessageCallback = x => x.FieldValue("id"),
            },
        ];
    }
}
