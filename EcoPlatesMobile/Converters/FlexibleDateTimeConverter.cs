using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcoPlatesMobile.Converters
{
    public class FlexibleDateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // 1) JSON null → C# null (if DateTime?); otherwise default(DateTime)
            if (reader.TokenType == JsonToken.Null)
            {
                if (objectType == typeof(DateTime?)) return null;
                return default(DateTime);
            }

            // 2) Empty string → same treatment
            if (reader.TokenType == JsonToken.String)
            {
                var str = (reader.Value ?? "").ToString();
                if (string.IsNullOrWhiteSpace(str))
                {
                    if (objectType == typeof(DateTime?)) return null;
                    return default(DateTime);
                }

                // 3) Try exact parse
                if (DateTime.TryParseExact(str, "yyyy-MM-dd HH:mm:ss",
                       null,
                       System.Globalization.DateTimeStyles.AssumeUniversal,
                       out var dt))
                {
                    return dt.ToUniversalTime();
                }

                // (you could add more formats here if needed)
            }

            // 4) Fallback for integers
            if (reader.TokenType == JsonToken.Integer)
            {
                var ms = Convert.ToInt64(reader.Value);
                return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
            }

            throw new JsonSerializationException($"Unable to convert value '{reader.Value}' to DateTime.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dt)
            {
                writer.WriteValue(dt.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }

}
