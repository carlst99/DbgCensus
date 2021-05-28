using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json
{
    public class BooleanJsonConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.False)
                return false;
            else if (reader.TokenType == JsonTokenType.True)
                return true;
            else if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Could not convert token to boolean.");

            string? data = reader.GetString();
            if (string.IsNullOrEmpty(data))
                throw new JsonException("Could not convert token to boolean: token was null");

            if (bool.TryParse(data, out bool result))
                return result;
            else if (data == "1")
                return true;
            else if (data == "0")
                return false;
            else
                throw new JsonException("Could not convert token to boolean - invalid format: " + data);
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
    }
}
