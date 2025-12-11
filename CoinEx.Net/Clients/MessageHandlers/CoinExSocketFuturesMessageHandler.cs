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
    internal class CoinExSocketFuturesMessageHandler : JsonSocketMessageHandler
    {
        public override JsonSerializerOptions Options { get; } = SerializerOptions.WithConverters(CoinExExchange._serializerContext);

        public override string? GetTopicFilter(object deserializedObject)
        {
            if (deserializedObject is CoinExSocketUpdate<CoinExTradeWrapper> tradeUpdate)
                return tradeUpdate.Data.Symbol;

            if (deserializedObject is CoinExSocketUpdate<CoinExBookPriceUpdate> bookPriceUpdate)
                return bookPriceUpdate.Data.Symbol;

            if (deserializedObject is CoinExSocketUpdate<CoinExOrderBook> bookUpdate)
                return bookUpdate.Data.Symbol;

            if (deserializedObject is CoinExSocketUpdate<CoinExIndexPriceUpdate> indexPriceUpdate)
                return indexPriceUpdate.Data.Symbol;

            return null;
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
                    new PropertyFieldReference("id").WithNotNullConstraint(),
                ],
                TypeIdentifierCallback = x => x.FieldValue("id")!,
            },
        ];
    }
}
