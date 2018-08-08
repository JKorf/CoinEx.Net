using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CoinEx.Net.Converters
{
    public class DecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = reader.Value.ToString();
            if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                return result;
            return 0m;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
