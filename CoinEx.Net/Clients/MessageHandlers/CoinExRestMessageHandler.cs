using CoinEx.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Converters.MessageParsing;
using CryptoExchange.Net.Converters.SystemTextJson;
using CryptoExchange.Net.Converters.SystemTextJson.MessageConverters;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Errors;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Coinbase.Net.Clients.MessageHandlers
{
    internal class CoinExRestMessageHandler : JsonRestMessageHandler
    {
        private readonly ErrorMapping _errorMapping;

        public override JsonSerializerOptions Options { get; } = SerializerOptions.WithConverters(CoinExExchange._serializerContext);

        public CoinExRestMessageHandler(ErrorMapping errorMapping)
        {
            _errorMapping = errorMapping;
        }

        public override async ValueTask<Error?> CheckForErrorResponse(RequestDefinition request, object? state, HttpResponseHeaders responseHeaders, Stream responseStream)
        {
            var (parseError, document) = await GetJsonDocument(responseStream, state).ConfigureAwait(false);
            if (parseError != null)
                return parseError;

            int? code = document!.RootElement.TryGetProperty("code", out var codeProp) ? codeProp.GetInt32() : null;
            if (code == 0)
                return null;

            var msg = document!.RootElement.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : null;
            return new ServerError(code!.Value, _errorMapping.GetErrorInfo(code.Value.ToString(), msg));
        }

        public override async ValueTask<Error> ParseErrorResponse(
            int httpStatusCode,
            object? state,
            HttpResponseHeaders responseHeaders,
            Stream responseStream)
        {
            if (httpStatusCode == 401 || httpStatusCode == 403)
                return new ServerError(new ErrorInfo(ErrorType.Unauthorized, "Unauthorized"));

            using var streamReader = new StreamReader(responseStream);
            return new ServerError(ErrorInfo.Unknown with { Message = await streamReader.ReadToEndAsync().ConfigureAwait(false) });
        }
    }
}
