using CoinEx.Net;
using CoinEx.Net.Objects.Models.V2;
using CoinEx.Net.Objects.Sockets.V2;
using CryptoExchange.Net.Converters.MessageParsing.DynamicConverters;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters.SystemTextJson.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoinEx.Net.Clients.MessageHandlers
{
    internal class CoinExSocketSpotMessageHandler : JsonSocketMessageHandler
    {
        public override JsonSerializerOptions Options { get; } = SerializerOptions.WithConverters(CoinExExchange._serializerContext);

        public CoinExSocketSpotMessageHandler()
        {
            AddTopicMapping<CoinExSocketUpdate<CoinExTradeWrapper>>(x => x.Data.Symbol);
            AddTopicMapping<CoinExSocketUpdate<CoinExBookPriceUpdate>>(x => x.Data.Symbol);
            AddTopicMapping<CoinExSocketUpdate<CoinExOrderBook>>(x => x.Data.Symbol);
            AddTopicMapping<CoinExSocketUpdate<CoinExIndexPriceUpdate>>(x => x.Data.Symbol);
        }

        protected override MessageTypeDefinition[] TypeEvaluators { get; } = [

            new MessageTypeDefinition {
                Fields = [
                    new PropertyFieldReference("method"),
                ],
                TypeIdentifierCallback = x => $"{x.FieldValue("method")}",
            },

            new MessageTypeDefinition {
                ForceIfFound = true,
                Fields = [
                    new PropertyFieldReference("id").WithNotNullContstraint(),
                ],
                TypeIdentifierCallback = x => x.FieldValue("id")!,
            },
        ];
    }
}
