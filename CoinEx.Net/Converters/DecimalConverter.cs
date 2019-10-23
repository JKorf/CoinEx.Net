using Newtonsoft.Json;
using System;
using System.Globalization;

namespace CoinEx.Net.Converters
{
    internal class DecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : 0m;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((decimal)value);
        }
    }
}
